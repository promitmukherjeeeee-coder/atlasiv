using Atlasiv.Core.Time;

namespace Atlasiv.Core.Scheduling;

/// <summary>One run of one system, as the timeline records it.</summary>
/// <param name="Stage">The stage it ran in.</param>
/// <param name="SystemId">The system.</param>
/// <param name="Covered">The time the run covered: everything since the system's last run.</param>
public sealed record SystemRun(ResolutionStage Stage, uint SystemId, TimeInterval Covered);
