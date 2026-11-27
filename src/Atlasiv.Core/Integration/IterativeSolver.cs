using System;
using Atlasiv.Core.Numerics;

namespace Atlasiv.Core.Integration;

/// <summary>
/// The solver interface's loop (spec section 10): iterate until a residual is within the tolerance
/// or the cap is reached, and then use the problem's fallback. Systems reach it through
/// <see cref="Scheduling.SystemContext.Solve"/>, which logs every fallback in the timeline, so a
/// solver that does not converge is never silent.
/// </summary>
internal static class IterativeSolver
{
    /// <exception cref="SimulationDomainException">The problem gave a negative residual.</exception>
    public static SolverResult<TState> Solve<TState>(IIterativeProblem<TState> problem, SolverSettings settings)
    {
        ArgumentNullException.ThrowIfNull(problem);
        ArgumentNullException.ThrowIfNull(settings);

        var state = problem.Initial;
        var residual = Q32.Zero;
        for (var i = 1; i <= settings.IterationCap; i++)
        {
            var next = problem.Iterate(state);
            residual = problem.Residual(state, next);
            if (residual < Q32.Zero)
            {
                throw new SimulationDomainException("The problem gave a negative residual (" + residual + ").");
            }

            state = next;
            if (residual <= settings.Tolerance)
            {
                return new SolverResult<TState>(state, SolverOutcome.Converged, i, residual);
            }
        }

        return new SolverResult<TState>(problem.Fallback(state), SolverOutcome.FellBack, settings.IterationCap, residual);
    }
}
