using System;
using System.Collections.Generic;
using System.Linq;
using Atlasiv.Core.Events;
using Atlasiv.Core.Integration;
using Atlasiv.Core.Randomness;
using Atlasiv.Core.Replay;
using Atlasiv.Core.Scheduling;
using Atlasiv.Core.Time;
using Xunit;
using static Atlasiv.Core.Tests.Scheduling.RunHarness;

namespace Atlasiv.Core.Tests.Scheduling;

/// <summary>
/// Substeps inside a run (spec section 10, clarifications 3 and 13): a system in a turn-end stage
/// splits the time it covers, each substep has its own random streams, and asking for more than
/// the budget is logged in the timeline.
/// </summary>
public sealed class SubstepsInRunTests
{
    private static readonly NaturalPeriod Yearly = NaturalPeriod.TurnOf(TurnStep.OneYear);

    [Fact]
    public void A_turn_end_system_gets_the_drivers_substeps_each_with_its_own_draw_address()
    {
        var seen = new List<Substep>();
        var system = new ScriptedSystem(7, ResolutionStage.TurnEnd(3), Yearly, (context, covered) =>
            seen.AddRange(context.Substeps(covered, 4)));
        var run = Start(TurnStep.OneYear, system);

        run.RunTurns(1);

        var year = Span(Date(-9999, 1, 1), Date(-9998, 1, 1));
        Assert.Equal(SubstepDriver.Split(year, 4), seen.Select(s => s.Span));
        Assert.Equal([0, 1, 2, 3], seen.Select(s => s.Index));
        Assert.Equal(
            Enumerable.Range(0, 4).Select(k => new DrawAddress(Seed, 1, TurnSlot.TurnEnd, k, 7, 42)),
            seen.Select(s => s.Draws(42)));
        Assert.Equal(4, seen.Select(s => s.Draws(42).Draw(0)).Distinct().Count());
    }

    [Fact]
    public void Asking_for_more_substeps_than_the_budget_is_logged_in_the_timeline()
    {
        var system = new ScriptedSystem(7, ResolutionStage.TurnEnd(3), Yearly, (context, covered) =>
            context.Substeps(covered, 6));
        var run = Start(TurnStep.OneYear, system);

        var timeline = run.RunTurns(2);

        var turnEnds = timeline.TurnEnds.ToList();
        Assert.Equal(
            [new SubstepEscalation(ResolutionStage.TurnEnd(3), 7, Span(Date(-9999, 1, 1), Date(-9998, 1, 1)), 6, 4)],
            turnEnds[0].SubstepEscalations);
        Assert.Equal(
            [new SubstepEscalation(ResolutionStage.TurnEnd(3), 7, Span(Date(-9998, 1, 1), Date(-9997, 1, 1)), 6, 4)],
            turnEnds[1].SubstepEscalations);
        Assert.All(timeline.Phases, p => Assert.Empty(p.SubstepEscalations));
    }

    [Fact]
    public void Substeps_within_the_budget_are_not_logged()
    {
        var system = new ScriptedSystem(7, ResolutionStage.TurnEnd(3), Yearly, (context, covered) =>
            context.Substeps(covered, 4));
        var run = Start(TurnStep.OneYear, system);

        var timeline = run.RunTurns(1);

        Assert.All(timeline.Entries, e => Assert.Empty(e.SubstepEscalations));
    }

    [Fact]
    public void The_budget_is_configurable()
    {
        var system = new ScriptedSystem(7, ResolutionStage.TurnEnd(3), Yearly, (context, covered) =>
            context.Substeps(covered, 6));
        var run = new SimulationRun(new SimulationSetup(
            Seed, new ScriptedLeadingEdge(TurnStep.OneYear), ReferenceUndertaking.Uncalibrated, [system], new InputLog(), SubstepBudget: 8));

        var timeline = run.RunTurns(1);

        Assert.All(timeline.Entries, e => Assert.Empty(e.SubstepEscalations));
    }

    [Fact]
    public void A_budget_below_1_is_refused()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new SimulationRun(new SimulationSetup(
            Seed, new ScriptedLeadingEdge(TurnStep.OneYear), ReferenceUndertaking.Uncalibrated, [], new InputLog(), SubstepBudget: 0)));
    }

    [Fact]
    public void Substeps_are_refused_outside_a_turn_end_stage()
    {
        // Spec section 10: substeps are used at the turn-end tick.
        Exception? refused = null;
        var system = new ScriptedSystem(7, ResolutionStage.Phase(6), Yearly, (context, covered) =>
            refused = Record.Exception(() => context.Substeps(covered, 2)));
        var run = Start(TurnStep.OneYear, system);

        run.RunTurns(1);

        Assert.IsType<InvalidOperationException>(refused);
    }

    [Fact]
    public void Part_of_the_covered_time_can_be_split_but_not_time_outside_it()
    {
        IReadOnlyList<Substep>? secondHalf = null;
        Exception? outside = null;
        var system = new ScriptedSystem(7, ResolutionStage.TurnEnd(3), Yearly, (context, covered) =>
        {
            secondHalf = context.Substeps(Span(Date(-9999, 7, 1), covered.End), 2);
            outside = Record.Exception(() => context.Substeps(Span(Date(-9998, 1, 1), Date(-9997, 1, 1)), 2));
        });
        var run = Start(TurnStep.OneYear, system);

        run.RunTurns(1);

        Assert.Equal(SubstepDriver.Split(Span(Date(-9999, 7, 1), Date(-9998, 1, 1)), 2), secondHalf!.Select(s => s.Span));
        Assert.IsType<ArgumentException>(outside);
    }

    [Fact]
    public void Substeps_are_refused_while_resolving_an_event()
    {
        // A system splits the time its own run covers; resolving an event covers no time.
        Exception? refused = null;
        var system = new ScriptedSystem(
            7,
            ResolutionStage.TurnEnd(3),
            Yearly,
            (context, _) => context.Schedule(context.TurnSpan.End, ResolutionStage.TurnEnd(5), 0, InterruptionClass.Informational, new Note("split")),
            (_, context) => refused = Record.Exception(() => context.Substeps(context.TurnSpan, 2)));
        var run = Start(TurnStep.OneYear, system);

        run.RunTurns(1);

        Assert.IsType<InvalidOperationException>(refused);
    }

    [Fact]
    public void A_context_kept_after_its_call_cannot_ask_for_substeps()
    {
        SystemContext? kept = null;
        var system = new ScriptedSystem(7, ResolutionStage.TurnEnd(3), Yearly, (context, _) => kept = context);
        var run = Start(TurnStep.OneYear, system);
        run.RunTurns(1);

        Assert.Throws<InvalidOperationException>(() => kept!.Substeps(Span(Date(-9999, 1, 1), Date(-9998, 1, 1)), 2));
    }
}
