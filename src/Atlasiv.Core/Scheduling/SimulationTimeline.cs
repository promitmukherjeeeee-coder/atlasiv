using System.Collections.Generic;
using System.Linq;
using Atlasiv.Core.Events;
using Atlasiv.Core.Time;

namespace Atlasiv.Core.Scheduling;

/// <summary>The record of a stretch of a simulation run: one entry per phase and turn end, in the order they ran.</summary>
public sealed class SimulationTimeline
{
    internal SimulationTimeline(IReadOnlyList<TimelineEntry> entries, SimulationEvent<EventPayload>? stoppedBy)
    {
        Entries = entries;
        StoppedBy = stoppedBy;
    }

    /// <summary>Every phase and turn end, in order.</summary>
    public IReadOnlyList<TimelineEntry> Entries { get; }

    /// <summary>The phases only.</summary>
    public IEnumerable<TimelineEntry> Phases => Entries.Where(e => !e.IsTurnEnd);

    /// <summary>The turn ends only.</summary>
    public IEnumerable<TimelineEntry> TurnEnds => Entries.Where(e => e.IsTurnEnd);

    /// <summary>Every step switch announced, in order.</summary>
    public IEnumerable<EraTransition> EraTransitions =>
        Entries.Where(e => e.Announced is not null).Select(e => e.Announced!);

    /// <summary>The event that stopped the run early, or null if it ran every turn it was asked for.</summary>
    public SimulationEvent<EventPayload>? StoppedBy { get; }
}
