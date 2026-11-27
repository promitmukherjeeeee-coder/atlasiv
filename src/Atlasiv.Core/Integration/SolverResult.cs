using Atlasiv.Core.Numerics;

namespace Atlasiv.Core.Integration;

/// <summary>How a solver finished.</summary>
public enum SolverOutcome
{
    /// <summary>A residual came within the tolerance.</summary>
    Converged = 1,

    /// <summary>The iteration cap was reached first, and the problem's fallback state was used.</summary>
    FellBack = 2,
}

/// <summary>What a solver returned and how it got there.</summary>
/// <typeparam name="TState">The problem's state.</typeparam>
/// <param name="State">The converged state, or the fallback state.</param>
/// <param name="Outcome">Whether it converged or fell back.</param>
/// <param name="Iterations">How many iterations ran.</param>
/// <param name="Residual">The last residual.</param>
public sealed record SolverResult<TState>(TState State, SolverOutcome Outcome, int Iterations, Q32 Residual);
