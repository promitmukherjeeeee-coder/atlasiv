using System;
using System.Globalization;
using Atlasiv.Core.Time;

namespace Atlasiv.Core.Events;

/// <summary>
/// An event's stable ID: the system that created it, the turn, the phase or turn-end slot it
/// was created in, and the system's sequence number within that slot (Pass 1 spec section 5,
/// clarification 3). Systems create events while iterating entities in ascending stable-ID
/// order, so the sequence numbers, and therefore the IDs, are the same on every run.
/// </summary>
/// <remarks>
/// <para>
/// IDs order by system, then turn, then slot (every phase before the turn end), then sequence
/// number: the fields in the order the spec lists them. This is the last field of the event
/// ordering key, so two distinct events never compare equal (<see cref="EventKey"/>).
/// </para>
/// <para>
/// Pass 2 replaces this with the permanent event identity, keeping the same ordering
/// guarantee. The default value (turn 0, slot 0) is not a valid ID (<see cref="IsValid"/>).
/// </para>
/// </remarks>
public readonly record struct StableEventId : IComparable<StableEventId>
{
    /// <summary>Creates an ID.</summary>
    /// <param name="systemId">The creating system's ID.</param>
    /// <param name="turn">The turn the event was created in, counting from 1 (spec clarification 17).</param>
    /// <param name="slot">The phase, or the turn end, the event was created in.</param>
    /// <param name="sequence">The creating system's sequence number within that slot, from 0.</param>
    /// <exception cref="ArgumentOutOfRangeException">The turn is below 1 or the slot is the default value.</exception>
    public StableEventId(uint systemId, long turn, TurnSlot slot, ulong sequence)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(turn, 1);
        ArgumentOutOfRangeException.ThrowIfZero(slot.Value, nameof(slot));
        SystemId = systemId;
        Turn = turn;
        Slot = slot;
        Sequence = sequence;
    }

    /// <summary>The creating system's ID.</summary>
    public uint SystemId { get; }

    /// <summary>The turn the event was created in, counting from 1.</summary>
    public long Turn { get; }

    /// <summary>The phase, or the turn end, the event was created in.</summary>
    public TurnSlot Slot { get; }

    /// <summary>The creating system's sequence number within the slot, from 0.</summary>
    public ulong Sequence { get; }

    /// <summary>False only for the default value, which no constructor can produce.</summary>
    public bool IsValid => Turn >= 1 && Slot.Value != 0;

    /// <summary>Orders IDs by system, then turn, then slot, then sequence number.</summary>
    public int CompareTo(StableEventId other)
    {
        var c = SystemId.CompareTo(other.SystemId);
        if (c != 0)
        {
            return c;
        }

        c = Turn.CompareTo(other.Turn);
        if (c != 0)
        {
            return c;
        }

        c = Slot.Value.CompareTo(other.Slot.Value);
        return c != 0 ? c : Sequence.CompareTo(other.Sequence);
    }

    /// <summary>Whether <paramref name="left"/> sorts before <paramref name="right"/>.</summary>
    public static bool operator <(StableEventId left, StableEventId right) => left.CompareTo(right) < 0;

    /// <summary>Whether <paramref name="left"/> sorts after <paramref name="right"/>.</summary>
    public static bool operator >(StableEventId left, StableEventId right) => left.CompareTo(right) > 0;

    /// <summary>Whether <paramref name="left"/> is the same as or sorts before <paramref name="right"/>.</summary>
    public static bool operator <=(StableEventId left, StableEventId right) => left.CompareTo(right) <= 0;

    /// <summary>Whether <paramref name="left"/> is the same as or sorts after <paramref name="right"/>.</summary>
    public static bool operator >=(StableEventId left, StableEventId right) => left.CompareTo(right) >= 0;

    /// <summary>The ID as text, such as "system 4, turn 30, phase 2, #7".</summary>
    public override string ToString() =>
        string.Create(CultureInfo.InvariantCulture, $"system {SystemId}, turn {Turn}, {Slot}, #{Sequence}");
}
