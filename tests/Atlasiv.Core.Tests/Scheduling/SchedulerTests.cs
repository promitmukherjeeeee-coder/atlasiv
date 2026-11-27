using System;
using System.Collections.Generic;
using System.Linq;
using Atlasiv.Core.Replay;
using Atlasiv.Core.Scheduling;
using Atlasiv.Core.Time;
using Xunit;
using static Atlasiv.Core.Tests.Scheduling.RunHarness;

namespace Atlasiv.Core.Tests.Scheduling;

/// <summary>
/// Which systems run, in which order, and over what time (Pass 1 spec section 6, clarification
/// 8, and the owner's 26 Nov 2026 ruling that a phase-stage system runs in the phase that reaches
/// its boundary). Expected dates are worked out by hand from the spec; dates are astronomical
/// year-month-day (year -9,999 = 10,000 BCE).
/// </summary>
public sealed class SchedulerTests
{
    private static readonly NaturalPeriod EveryPhase = NaturalPeriod.PhaseOf(TurnStep.OneMonth);
    private static readonly NaturalPeriod Yearly = NaturalPeriod.TurnOf(TurnStep.OneYear);

    [Fact]
    public void Phase_stages_run_1_to_11_and_turn_end_stages_1_to_10_whatever_the_system_IDs()
    {
        // IDs run against the stage order, so ID order cannot be what puts the stages in order.
        var systems = new List<ISimulationSystem>();
        for (var n = 1; n <= 11; n++)
        {
            systems.Add(new ScriptedSystem((uint)(100 - n), ResolutionStage.Phase(n), EveryPhase));
        }

        for (var n = 1; n <= 10; n++)
        {
            systems.Add(new ScriptedSystem((uint)(200 - n), ResolutionStage.TurnEnd(n), EveryPhase));
        }

        var run = Start(new ScriptedLeadingEdge(TurnStep.OneYear), new InputLog(), [.. systems]);

        var timeline = run.RunTurns(2);

        var phaseOrder = Enumerable.Range(1, 11).Select(ResolutionStage.Phase).ToList();
        var turnEndOrder = Enumerable.Range(1, 10).Select(ResolutionStage.TurnEnd).ToList();
        Assert.Equal(8, timeline.Phases.Count());
        Assert.All(timeline.Phases, p => Assert.Equal(phaseOrder, p.Runs.Select(r => r.Stage)));
        Assert.All(timeline.TurnEnds, t => Assert.Equal(turnEndOrder, t.Runs.Select(r => r.Stage)));
    }

    [Fact]
    public void Systems_in_one_stage_run_in_ascending_ID_order_whatever_order_they_were_registered_in()
    {
        var stage = ResolutionStage.Phase(6);
        var run = Start(
            TurnStep.OneYear,
            new ScriptedSystem(30, stage, EveryPhase),
            new ScriptedSystem(10, stage, EveryPhase),
            new ScriptedSystem(20, stage, EveryPhase));

        var timeline = run.RunTurns(1);

        Assert.All(timeline.Phases, p => Assert.Equal([10u, 20u, 30u], p.Runs.Select(r => r.SystemId)));
    }

    [Fact]
    public void A_system_runs_only_in_its_own_stage()
    {
        var run = Start(TurnStep.OneYear, new ScriptedSystem(1, ResolutionStage.TurnEnd(3), EveryPhase));

        var timeline = run.RunTurns(3);

        Assert.All(timeline.Phases, p => Assert.Empty(p.Runs));
        Assert.All(timeline.TurnEnds, t => Assert.Equal([ResolutionStage.TurnEnd(3)], t.Runs.Select(r => r.Stage)));
    }

    [Fact]
    public void A_yearly_turn_end_system_runs_every_12th_turn_on_the_1_month_step_covering_each_calendar_year()
    {
        var run = Start(TurnStep.OneMonth, new ScriptedSystem(1, ResolutionStage.TurnEnd(1), Yearly));

        var timeline = run.RunTurns(36);

        Assert.Equal(
            [(12L, Span(Date(-9999, 1, 1), Date(-9998, 1, 1))), (24L, Span(Date(-9998, 1, 1), Date(-9997, 1, 1))), (36L, Span(Date(-9997, 1, 1), Date(-9996, 1, 1)))],
            RunsOf(timeline, 1));
    }

    [Fact]
    public void A_yearly_turn_end_system_runs_once_per_turn_on_the_50_year_step_covering_the_whole_turn()
    {
        var run = Start(TurnStep.FiftyYears, new ScriptedSystem(1, ResolutionStage.TurnEnd(1), Yearly));

        var timeline = run.RunTurns(3);

        Assert.Equal(
            [(1L, Span(Date(-9999, 1, 1), Date(-9949, 1, 1))), (2L, Span(Date(-9949, 1, 1), Date(-9899, 1, 1))), (3L, Span(Date(-9899, 1, 1), Date(-9849, 1, 1)))],
            RunsOf(timeline, 1));
    }

    [Fact]
    public void A_system_whose_period_is_shorter_than_the_turn_runs_once_per_turn_end_not_once_per_period()
    {
        var monthly = NaturalPeriod.TurnOf(TurnStep.OneMonth);
        var run = Start(TurnStep.TenYears, new ScriptedSystem(1, ResolutionStage.TurnEnd(5), monthly));

        var timeline = run.RunTurns(2);

        Assert.Equal(
            [(1L, Span(Date(-9999, 1, 1), Date(-9989, 1, 1))), (2L, Span(Date(-9989, 1, 1), Date(-9979, 1, 1)))],
            RunsOf(timeline, 1));
    }

    [Fact]
    public void A_5_year_system_on_the_1_year_step_runs_on_the_1_January_boundaries_counted_from_the_epoch()
    {
        var fiveYearly = NaturalPeriod.TurnOf(TurnStep.FiveYears);
        var run = Start(TurnStep.OneYear, new ScriptedSystem(1, ResolutionStage.TurnEnd(1), fiveYearly));

        var timeline = run.RunTurns(11);

        Assert.Equal(
            [(5L, Span(Date(-9999, 1, 1), Date(-9994, 1, 1))), (10L, Span(Date(-9994, 1, 1), Date(-9989, 1, 1)))],
            RunsOf(timeline, 1));
    }

    [Fact]
    public void A_5_year_system_on_the_2_year_step_covers_6_years_then_4()
    {
        // Clarification 18: boundaries at years 5 and 10 are first reached by the turn ends after
        // years 6 (turn 3) and 10 (turn 5). Neither 6 nor 4 years is a legal step.
        var fiveYearly = NaturalPeriod.TurnOf(TurnStep.FiveYears);
        var run = Start(TurnStep.TwoYears, new ScriptedSystem(1, ResolutionStage.TurnEnd(1), fiveYearly));

        var timeline = run.RunTurns(5);

        Assert.Equal(
            [(3L, Span(Date(-9999, 1, 1), Date(-9993, 1, 1))), (5L, Span(Date(-9993, 1, 1), Date(-9989, 1, 1)))],
            RunsOf(timeline, 1));
    }

    [Fact]
    public void A_30_month_system_follows_its_alternating_1_January_and_1_July_boundaries()
    {
        // Boundaries at 1 July 9,998 BCE (30 months in) and 1 January 9,995 BCE (60 months in).
        var thirtyMonths = NaturalPeriod.PhaseOf(TurnStep.FiftyYears);
        var run = Start(TurnStep.SixMonths, new ScriptedSystem(1, ResolutionStage.TurnEnd(1), thirtyMonths));

        var timeline = run.RunTurns(10);

        Assert.Equal(
            [(5L, Span(Date(-9999, 1, 1), Date(-9997, 7, 1))), (10L, Span(Date(-9997, 7, 1), Date(-9994, 1, 1)))],
            RunsOf(timeline, 1));
    }

    [Fact]
    public void Due_stays_anchored_to_1_January_across_a_switch_from_the_1_year_to_the_6_month_step()
    {
        // 1-year turns until a faster candidate appears at the start of turn 21 (1 January 9,980 BCE).
        // That is the 10th faster reading, so 6-month turns begin at once with turn 30 (1 January 9,971 BCE).
        var edge = new ScriptedLeadingEdge(TurnStep.OneYear).Then(Date(-9979, 1, 1), TurnStep.SixMonths);
        var run = Start(edge, new InputLog(), new ScriptedSystem(1, ResolutionStage.TurnEnd(1), Yearly));

        var timeline = run.RunTurns(36);

        Assert.Equal(TurnStep.SixMonths, Assert.Single(timeline.EraTransitions).NewStep);
        var runs = RunsOf(timeline, 1);
        Assert.Equal(Enumerable.Range(1, 29).Select(t => (long)t), runs.Take(29).Select(r => r.Turn));
        Assert.Equal([31L, 33L, 35L], runs.Skip(29).Select(r => r.Turn));
        Assert.All(runs, r => Assert.True(IsWholeCalendarYear(r.Covered), r.Covered.ToString()));
        Assert.Equal(Span(Date(-9970, 1, 1), Date(-9969, 1, 1)), runs[29].Covered);
    }

    [Fact]
    public void Due_stays_anchored_to_the_calendar_when_a_slower_switch_waits_for_alignment()
    {
        // 1-month turns. A slower candidate (6 months) from 1 February 10,000 BCE: after 20 turns the
        // switch is announced at 1 September 9,999 BCE and waits for 1 January 9,998 BCE (turn 25).
        var edge = new ScriptedLeadingEdge(TurnStep.OneMonth).Then(Date(-9999, 2, 1), TurnStep.SixMonths);
        var quarterly = NaturalPeriod.PhaseOf(TurnStep.OneYear);
        var run = Start(edge, new InputLog(), new ScriptedSystem(1, ResolutionStage.TurnEnd(2), quarterly));

        var timeline = run.RunTurns(28);

        var transition = Assert.Single(timeline.EraTransitions);
        Assert.Equal(Date(-9998, 9, 1), transition.AnnouncedAt);
        Assert.Equal(Date(-9997, 1, 1), transition.EffectiveAt);

        // Every quarter on the 1-month turns; then each 6-month turn end passes one boundary and covers two quarters.
        var covered = RunsOf(timeline, 1).Select(r => r.Covered.End).ToList();
        Assert.Equal(
            [
                Date(-9999, 4, 1), Date(-9999, 7, 1), Date(-9999, 10, 1), Date(-9998, 1, 1),
                Date(-9998, 4, 1), Date(-9998, 7, 1), Date(-9998, 10, 1), Date(-9997, 1, 1),
                Date(-9997, 7, 1), Date(-9996, 1, 1), Date(-9996, 7, 1), Date(-9995, 1, 1),
            ],
            covered);
    }

    [Fact]
    public void Each_run_covers_exactly_the_time_since_the_previous_one_with_no_gap_or_overlap()
    {
        var edge = new ScriptedLeadingEdge(TurnStep.OneYear).Then(Date(-9979, 1, 1), TurnStep.SixMonths);
        var twoMonthly = NaturalPeriod.PhaseOf(TurnStep.SixMonths);
        var run = Start(edge, new InputLog(), new ScriptedSystem(1, ResolutionStage.TurnEnd(4), twoMonthly));

        var timeline = run.RunTurns(40);

        var runs = RunsOf(timeline, 1);
        Assert.Equal(Instant.Epoch, runs[0].Covered.Start);
        for (var i = 1; i < runs.Count; i++)
        {
            Assert.Equal(runs[i - 1].Covered.End, runs[i].Covered.Start);
        }

        Assert.Equal(timeline.TurnEnds.Last().At, runs[^1].Covered.End);
    }

    [Fact]
    public void A_yearly_phase_stage_system_runs_in_the_phase_that_ends_on_1_January_covering_the_calendar_year()
    {
        var run = Start(TurnStep.OneMonth, new ScriptedSystem(1, ResolutionStage.Phase(5), Yearly));

        var timeline = run.RunTurns(24);

        var runs = timeline.Phases.Where(p => p.Runs.Count > 0).ToList();
        Assert.Equal(2, runs.Count);
        Assert.All(runs, p => Assert.Equal(5u, p.Slot.Value));                    // the last phase, 25 to 31 December
        Assert.Equal(Span(Date(-9999, 12, 25), Date(-9998, 1, 1)), runs[0].Span);
        Assert.Equal(Span(Date(-9999, 1, 1), Date(-9998, 1, 1)), runs[0].Runs[0].Covered);
        Assert.Equal(Span(Date(-9998, 1, 1), Date(-9997, 1, 1)), runs[1].Runs[0].Covered);
    }

    [Fact]
    public void An_every_phase_system_runs_in_every_phase_on_every_step_covering_that_phase()
    {
        foreach (var step in StepTable.All)
        {
            var run = Start(step, new ScriptedSystem(1, ResolutionStage.Phase(6), EveryPhase));

            var timeline = run.RunTurns(2);

            Assert.All(timeline.Phases, p => Assert.Equal(p.Span, Assert.Single(p.Runs).Covered));
        }
    }

    [Fact]
    public void A_phase_stage_system_with_a_period_longer_than_the_phase_covers_several_phases_in_one_run()
    {
        // 6-month period on 1-year turns (3-month phases): runs in the phases ending 1 July and 1 January.
        var halfYearly = NaturalPeriod.TurnOf(TurnStep.SixMonths);
        var run = Start(TurnStep.OneYear, new ScriptedSystem(1, ResolutionStage.Phase(2), halfYearly));

        var timeline = run.RunTurns(1);

        Assert.Equal([2u, 4u], timeline.Phases.Where(p => p.Runs.Count > 0).Select(p => p.Slot.Value));
        Assert.Equal(
            [Span(Date(-9999, 1, 1), Date(-9999, 7, 1)), Span(Date(-9999, 7, 1), Date(-9998, 1, 1))],
            timeline.Phases.SelectMany(p => p.Runs).Select(r => r.Covered));
    }

    [Fact]
    public void Two_systems_cannot_share_an_ID()
    {
        Assert.Throws<ArgumentException>(() => Start(
            TurnStep.OneYear,
            new ScriptedSystem(7, ResolutionStage.Phase(1), EveryPhase),
            new ScriptedSystem(7, ResolutionStage.TurnEnd(1), Yearly)));
    }

    [Fact]
    public void System_ID_0_is_reserved_for_the_clock()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new SystemRegistration(0, ResolutionStage.Phase(1), EveryPhase));
    }

    [Fact]
    public void A_registration_needs_a_stage_and_a_period()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new SystemRegistration(1, default, EveryPhase));
        Assert.Throws<ArgumentOutOfRangeException>(() => new SystemRegistration(1, ResolutionStage.Phase(1), default));
    }

    private static List<(long Turn, TimeInterval Covered)> RunsOf(SimulationTimeline timeline, uint systemId) =>
        timeline.Entries
            .SelectMany(e => e.Runs.Where(r => r.SystemId == systemId).Select(r => (e.Turn, r.Covered)))
            .ToList();

    private static bool IsWholeCalendarYear(TimeInterval span)
    {
        var start = span.Start.Date;
        var end = span.End.Date;
        return start.Month == 1 && start.Day == 1 && end.Month == 1 && end.Day == 1 && end.Year == start.Year + 1
            && span.Start.SecondOfDay == 0 && span.End.SecondOfDay == 0;
    }
}
