using Atlasiv.Core.Events;
using Atlasiv.Core.Time;

namespace Atlasiv.Core.Scheduling;

/// <summary>
/// The payload of the informational event that announces a step switch, with its preview (spec
/// section 4, clarification 10). The clock (system 0) creates it at the boundary where the
/// switch is decided.
/// </summary>
/// <param name="Transition">The switch and its preview.</param>
public sealed record EraTransitionAnnounced(EraTransition Transition) : EventPayload;
