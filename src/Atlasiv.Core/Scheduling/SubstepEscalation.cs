using Atlasiv.Core.Time;

namespace Atlasiv.Core.Scheduling;

/// <summary>
/// A system asked for more substeps than the budget (spec section 10, clarification 13). Logged
/// in the timeline for benchmarking; it changes nothing else.
/// </summary>
/// <param name="Stage">The turn-end stage the system was running in.</param>
/// <param name="SystemId">The system.</param>
/// <param name="Interval">The interval it split.</param>
/// <param name="Count">How many substeps it asked for.</param>
/// <param name="Budget">The run's substep budget.</param>
public sealed record SubstepEscalation(ResolutionStage Stage, uint SystemId, TimeInterval Interval, int Count, int Budget);
