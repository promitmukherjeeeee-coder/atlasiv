using Atlasiv.Core.Integration;
using Atlasiv.Core.Numerics;

namespace Atlasiv.Core.Scheduling;

/// <summary>
/// A solver reached its iteration cap without converging and used its fallback (spec section 10).
/// Logged in the timeline so that no failure to converge is silent.
/// </summary>
/// <param name="Stage">The stage being resolved when the system called the solver.</param>
/// <param name="SystemId">The system.</param>
/// <param name="Settings">The cap and tolerance it ran with.</param>
/// <param name="Residual">The last residual, which was above the tolerance.</param>
public sealed record SolverFallback(ResolutionStage Stage, uint SystemId, SolverSettings Settings, Q32 Residual);
