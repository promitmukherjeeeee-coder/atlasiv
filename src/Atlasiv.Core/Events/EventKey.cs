using System;
using System.Globalization;
using Atlasiv.Core.Scheduling;
using Atlasiv.Core.Time;

namespace Atlasiv.Core.Events;

/// <summary>
/// An event's ordering key: (timestamp, resolution stage, event priority, stable event ID)
/// (Pass 1 spec section 5). The event queue resolves events in ascending key order.
/// </summary>
/// <remarks>
/// <para>
/// Each field outranks every later one. At the same instant, turn-end stages come before phase
/// stages (<see cref="ResolutionStage"/>). A lower priority number resolves first (owner
/// ruling, 26 Nov 2026; ENG-012). The stable ID is unique, so two distinct events never
/// compare equal.
/// </para>
/// <para>
/// The default value has no valid stage or ID (<see cref="IsValid"/>); the queue refuses it.
/// </para>
/// </remarks>
/// <param name="Timestamp">When the event happens.</param>
/// <param name="Stage">The stage that resolves it.</param>
/// <param name="Priority">Breaks ties between events at the same instant and stage; lower numbers first.</param>
/// <param name="Id">The event's stable ID.</param>
public readonly record struct EventKey(Instant Timestamp, ResolutionStage Stage, int Priority, StableEventId Id)
    : IComparable<EventKey>
{
    /// <summary>True when both the stage and the stable ID are valid.</summary>
    public bool IsValid => Stage.IsValid && Id.IsValid;

    /// <summary>Orders keys by timestamp, then stage, then priority (lower first), then stable ID.</summary>
    public int CompareTo(EventKey other)
    {
        var c = Timestamp.CompareTo(other.Timestamp);
        if (c != 0)
        {
            return c;
        }

        c = Stage.CompareTo(other.Stage);
        if (c != 0)
        {
            return c;
        }

        c = Priority.CompareTo(other.Priority);
        return c != 0 ? c : Id.CompareTo(other.Id);
    }

    /// <summary>Whether <paramref name="left"/> resolves before <paramref name="right"/>.</summary>
    public static bool operator <(EventKey left, EventKey right) => left.CompareTo(right) < 0;

    /// <summary>Whether <paramref name="left"/> resolves after <paramref name="right"/>.</summary>
    public static bool operator >(EventKey left, EventKey right) => left.CompareTo(right) > 0;

    /// <summary>Whether <paramref name="left"/> is the same as or resolves before <paramref name="right"/>.</summary>
    public static bool operator <=(EventKey left, EventKey right) => left.CompareTo(right) <= 0;

    /// <summary>Whether <paramref name="left"/> is the same as or resolves after <paramref name="right"/>.</summary>
    public static bool operator >=(EventKey left, EventKey right) => left.CompareTo(right) >= 0;

    /// <summary>The key as text: timestamp (seconds from the epoch), stage, priority and ID.</summary>
    public override string ToString() =>
        string.Create(CultureInfo.InvariantCulture, $"[t={Timestamp.SecondsSinceEpoch}s, {Stage}, priority {Priority}, {Id}]");
}
