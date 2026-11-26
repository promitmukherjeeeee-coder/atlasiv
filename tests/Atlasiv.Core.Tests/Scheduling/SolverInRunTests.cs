using System;
using System.Linq;
using Atlasiv.Core.Integration;
using Atlasiv.Core.Numerics;
using Atlasiv.Core.Scheduling;
using Atlasiv.Core.Time;
using Xunit;
using static Atlasiv.Core.Tests.Scheduling.RunHarness;

namespace Atlasiv.Core.Tests.Scheduling;

/// <summary>
/// The solver interface inside a run (spec section 10): a solver that falls back is logged in the
/// timeline, so it is never silent; one that converges is not.
/// </summary>
public sealed class SolverInRunTests
{
    private static readonly NaturalPeriod EveryPhase = NaturalPeriod.PhaseOf(TurnStep.OneMonth);

    [Fact]
    public void A_fallback_is_logged_in_the_entry_where_the_system_called_the_solver()
    {
        var settings = new SolverSettings(3, Q32.Zero);
        SolverResult<Q32>? result = null;
        var system = new ScriptedSystem(5, ResolutionStage.Phase(9), EveryPhase, (context, _) =>
        {
            if (context.Slot == TurnSlot.Phase(2))
            {
                result = context.Solve(new NeverConverges(), settings);
            }
        });
        var run = Start(TurnStep.OneMonth, system);

        var timeline = run.RunTurns(1);

        Assert.Equal(SolverOutcome.FellBack, result!.Outcome);
        Assert.Equal(NeverConverges.FallbackState, result.State);
        var entries = timeline.Entries.ToList();
        Assert.Equal([new SolverFallback(ResolutionStage.Phase(9), 5, settings, Q32.One)], entries[1].SolverFallbacks);
        Assert.All(entries.Where((_, i) => i != 1), e => Assert.Empty(e.SolverFallbacks));
    }

    [Fact]
    public void A_converged_solve_is_not_logged()
    {
        var system = new ScriptedSystem(5, ResolutionStage.TurnEnd(4), EveryPhase, (context, _) =>
            context.Solve(new ConvergesAtOnce(), SolverSettings.Uncalibrated));
        var run = Start(TurnStep.OneMonth, system);

        var timeline = run.RunTurns(1);

        Assert.All(timeline.Entries, e => Assert.Empty(e.SolverFallbacks));
    }

    [Fact]
    public void A_context_kept_after_its_call_cannot_run_the_solver()
    {
        SystemContext? kept = null;
        var system = new ScriptedSystem(5, ResolutionStage.TurnEnd(4), EveryPhase, (context, _) => kept = context);
        var run = Start(TurnStep.OneMonth, system);
        run.RunTurns(1);

        Assert.Throws<InvalidOperationException>(() => kept!.Solve(new ConvergesAtOnce(), SolverSettings.Uncalibrated));
    }

    /// <summary>Adds 1 every iteration, so the residual is always 1.</summary>
    private sealed class NeverConverges : IIterativeProblem<Q32>
    {
        public static readonly Q32 FallbackState = Q32.FromInt(-7);

        public Q32 Initial => Q32.Zero;

        public Q32 Iterate(Q32 current) => current + Q32.One;

        public Q32 Residual(Q32 before, Q32 after) => after - before;

        public Q32 Fallback(Q32 last) => FallbackState;
    }

    /// <summary>Stays where it starts, so the first residual is 0.</summary>
    private sealed class ConvergesAtOnce : IIterativeProblem<Q32>
    {
        public Q32 Initial => Q32.One;

        public Q32 Iterate(Q32 current) => current;

        public Q32 Residual(Q32 before, Q32 after) => after - before;

        public Q32 Fallback(Q32 last) => throw new InvalidOperationException("A converged solve never falls back.");
    }
}
