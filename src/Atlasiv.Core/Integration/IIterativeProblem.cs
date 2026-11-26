using Atlasiv.Core.Numerics;

namespace Atlasiv.Core.Integration;

/// <summary>
/// A problem an explicit solver works on by repeated iteration, such as a circular dependency
/// between systems (spec section 10). The real problems and solvers come in Pass 3.
/// </summary>
/// <typeparam name="TState">What each iteration improves.</typeparam>
public interface IIterativeProblem<TState>
{
    /// <summary>Where the first iteration starts.</summary>
    TState Initial { get; }

    /// <summary>One iteration: the next state from the current one.</summary>
    TState Iterate(TState current);

    /// <summary>How far apart two successive states are; never negative. The solver converges when it is within the tolerance.</summary>
    Q32 Residual(TState before, TState after);

    /// <summary>The state to use when the solver reaches its iteration cap without converging.</summary>
    /// <param name="last">The last state the iterations reached.</param>
    TState Fallback(TState last);
}
