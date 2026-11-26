using System.Collections.Generic;
using Atlasiv.Core.Replay;
using Atlasiv.Core.Time;

namespace Atlasiv.Core.Scheduling;

/// <summary>Everything a simulation run starts from.</summary>
/// <param name="WorldSeed">The seed every random draw is keyed by.</param>
/// <param name="LeadingEdge">Where the step controller reads the leading edge's strategic time (scripted in Pass 1).</param>
/// <param name="Undertaking">The reference undertaking the leading edge is timed on.</param>
/// <param name="Systems">The registered systems, in any order; each must have its own ID.</param>
/// <param name="InputLog">The orders for each phase.</param>
public sealed record SimulationSetup(
    ulong WorldSeed,
    ILeadingEdge LeadingEdge,
    ReferenceUndertaking Undertaking,
    IReadOnlyList<ISimulationSystem> Systems,
    InputLog InputLog);
