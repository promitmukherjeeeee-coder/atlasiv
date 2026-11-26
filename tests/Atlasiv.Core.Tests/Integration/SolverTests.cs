using System;
using Atlasiv.Core.Integration;
using Atlasiv.Core.Numerics;
using Xunit;

namespace Atlasiv.Core.Tests.Integration;

/// <summary>
/// The solver interface: an iteration cap, a tolerance and a fallback (spec section 10). The real
/// solvers come in Pass 3; these tests use a problem whose every step is known by hand: x halves each
/// iteration from 1, and the residual is the change, so iteration i leaves x = 2^-i and residual 2^-i.
/// </summary>
public sealed class SolverTests
{
    private static readonly Q32 OneOver1024 = Q32.FromRaw(1L << 22);

    [Fact]
    public void A_problem_converges_on_the_first_iteration_whose_residual_is_within_the_tolerance()
    {
        var result = IterativeSolver.Solve(new Halving(), new SolverSettings(50, OneOver1024));

        Assert.Equal(SolverOutcome.Converged, result.Outcome);
        Assert.Equal(10, result.Iterations);
        Assert.Equal(OneOver1024, result.Residual);
        Assert.Equal(OneOver1024, result.State);
    }

    [Fact]
    public void Converging_on_the_last_allowed_iteration_still_counts_as_converged()
    {
        var result = IterativeSolver.Solve(new Halving(), new SolverSettings(10, OneOver1024));

        Assert.Equal(SolverOutcome.Converged, result.Outcome);
        Assert.Equal(10, result.Iterations);
    }

    [Fact]
    public void Reaching_the_cap_without_converging_returns_the_fallback_and_says_so()
    {
        var problem = new Halving();

        var result = IterativeSolver.Solve(problem, new SolverSettings(5, OneOver1024));

        Assert.Equal(SolverOutcome.FellBack, result.Outcome);
        Assert.Equal(5, result.Iterations);
        Assert.Equal(Q32.FromRaw(1L << 27), result.Residual);
        Assert.Equal(Q32.FromRaw(1L << 27), problem.FallbackGiven);
        Assert.Equal(Halving.FallbackState, result.State);
    }

    [Fact]
    public void A_negative_residual_is_refused()
    {
        Assert.Throws<SimulationDomainException>(() =>
            IterativeSolver.Solve(new Halving(residualSign: -1), SolverSettings.Uncalibrated));
    }

    [Fact]
    public void An_iteration_cap_below_1_or_a_negative_tolerance_is_refused()
    {
        Assert.Throws<SimulationDomainException>(() => new SolverSettings(0, OneOver1024));
        Assert.Throws<SimulationDomainException>(() => new SolverSettings(10, -Q32.Epsilon));
    }

    [Fact]
    public void The_uncalibrated_settings_are_the_named_placeholders()
    {
        Assert.Equal(SolverSettings.PlaceholderIterationCap, SolverSettings.Uncalibrated.IterationCap);
        Assert.Equal(SolverSettings.PlaceholderTolerance, SolverSettings.Uncalibrated.Tolerance);
    }

    /// <summary>x halves each iteration from 1; the residual is how much it changed.</summary>
    private sealed class Halving(int residualSign = 1) : IIterativeProblem<Q32>
    {
        public static readonly Q32 FallbackState = Q32.FromInt(-1);

        public Q32? FallbackGiven { get; private set; }

        public Q32 Initial => Q32.One;

        public Q32 Iterate(Q32 current) => Q32.FromRaw(current.Raw / 2);

        public Q32 Residual(Q32 before, Q32 after) => Q32.FromRaw(residualSign * (before.Raw - after.Raw));

        public Q32 Fallback(Q32 last)
        {
            FallbackGiven = last;
            return FallbackState;
        }
    }
}
