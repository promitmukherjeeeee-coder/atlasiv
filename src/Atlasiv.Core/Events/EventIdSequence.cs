using System;
using System.Globalization;
using Atlasiv.Core.Numerics;
using Atlasiv.Core.Time;

namespace Atlasiv.Core.Events;

/// <summary>
/// Hands out one system's stable event IDs within one phase or turn end, numbering them from
/// 0 in creation order (Pass 1 spec section 5). A system makes a new sequence for each slot
/// it runs in, and creates its events while iterating entities in ascending stable-ID order,
/// so the numbers are the same on every run.
/// </summary>
/// <remarks>
/// The sequence carries on across the substeps of a turn-end stage, so no two substeps share
/// an ID. It is not safe to share between threads; each system owns its own.
/// </remarks>
public sealed class EventIdSequence
{
    private ulong _next;
    private bool _exhausted;

    /// <summary>Starts a sequence at number 0.</summary>
    /// <param name="systemId">The creating system's ID.</param>
    /// <param name="turn">The current turn, counting from 1.</param>
    /// <param name="slot">The current phase, or the turn end.</param>
    /// <exception cref="ArgumentOutOfRangeException">The turn is below 1 or the slot is the default value.</exception>
    public EventIdSequence(uint systemId, long turn, TurnSlot slot)
        : this(systemId, turn, slot, 0)
    {
    }

    /// <summary>Starts a sequence at <paramref name="firstSequenceNumber"/>; lets tests reach the end of the range.</summary>
    internal EventIdSequence(uint systemId, long turn, TurnSlot slot, ulong firstSequenceNumber)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(turn, 1);
        ArgumentOutOfRangeException.ThrowIfZero(slot.Value, nameof(slot));
        SystemId = systemId;
        Turn = turn;
        Slot = slot;
        _next = firstSequenceNumber;
    }

    /// <summary>The creating system's ID.</summary>
    public uint SystemId { get; }

    /// <summary>The turn.</summary>
    public long Turn { get; }

    /// <summary>The phase, or the turn end.</summary>
    public TurnSlot Slot { get; }

    /// <summary>The next ID in the sequence.</summary>
    /// <exception cref="SimulationOverflowException">Every 64-bit sequence number has been used.</exception>
    public StableEventId Next()
    {
        if (_exhausted)
        {
            throw new SimulationOverflowException(string.Create(
                CultureInfo.InvariantCulture,
                $"EventIdSequence: system {SystemId} has used every 64-bit sequence number in turn {Turn}, {Slot}."));
        }

        var id = new StableEventId(SystemId, Turn, Slot, _next);
        if (_next == ulong.MaxValue)
        {
            _exhausted = true;
        }
        else
        {
            _next++;
        }

        return id;
    }
}
