using System;
using System.Collections.Generic;
using Atlasiv.Core.Events;
using Atlasiv.Core.Numerics;
using Atlasiv.Core.Time;
using Xunit;

namespace Atlasiv.Core.Tests.Events;

/// <summary>
/// Stable event IDs: (creating system ID, turn, phase or turn-end slot, per-system sequence
/// number) (spec section 5, clarification 3).
/// </summary>
public sealed class StableEventIdTests
{
    // ---- The turn end has its own slot ----

    [Fact]
    public void Events_created_at_the_turn_end_get_ids_distinct_from_the_last_phase()
    {
        foreach (var step in StepTable.All)
        {
            var lastPhase = new EventIdSequence(systemId: 4, turn: 30, TurnSlot.Phase(StepTable.PhaseCount(step)));
            var turnEnd = new EventIdSequence(systemId: 4, turn: 30, TurnSlot.TurnEnd);

            var ids = new HashSet<StableEventId>();
            for (var i = 0; i < 100; i++)
            {
                Assert.True(ids.Add(lastPhase.Next()), $"{step}: last-phase ID {i} repeated");
                Assert.True(ids.Add(turnEnd.Next()), $"{step}: turn-end ID {i} repeated a last-phase ID");
            }
        }
    }

    [Fact]
    public void A_turn_end_id_sorts_after_every_phase_id_of_its_turn()
    {
        var turnEnd = new StableEventId(4, 30, TurnSlot.TurnEnd, 0);
        var lastPhaseLateEvent = new StableEventId(4, 30, TurnSlot.Phase(20), ulong.MaxValue);
        var nextTurn = new StableEventId(4, 31, TurnSlot.Phase(1), 0);

        Assert.True(lastPhaseLateEvent.CompareTo(turnEnd) < 0);
        Assert.True(turnEnd.CompareTo(nextTurn) < 0);
    }

    // ---- The sequence counter ----

    [Fact]
    public void A_sequence_numbers_its_events_from_zero_in_creation_order()
    {
        var sequence = new EventIdSequence(systemId: 9, turn: 2, TurnSlot.Phase(3));

        Assert.Equal(new StableEventId(9, 2, TurnSlot.Phase(3), 0), sequence.Next());
        Assert.Equal(new StableEventId(9, 2, TurnSlot.Phase(3), 1), sequence.Next());
        Assert.Equal(new StableEventId(9, 2, TurnSlot.Phase(3), 2), sequence.Next());
    }

    [Fact]
    public void Two_systems_in_the_same_phase_never_share_an_id()
    {
        var first = new EventIdSequence(systemId: 1, turn: 5, TurnSlot.Phase(2));
        var second = new EventIdSequence(systemId: 2, turn: 5, TurnSlot.Phase(2));

        Assert.NotEqual(first.Next(), second.Next());
    }

    [Fact]
    public void A_sequence_that_runs_out_of_numbers_is_an_overflow_error()
    {
        var sequence = new EventIdSequence(systemId: 1, turn: 1, TurnSlot.Phase(1), firstSequenceNumber: ulong.MaxValue);

        Assert.Equal(ulong.MaxValue, sequence.Next().Sequence);
        Assert.Throws<SimulationOverflowException>(() => sequence.Next());
    }

    // ---- Ordering: system, then turn, then slot, then sequence ----

    [Fact]
    public void Ids_order_by_system_then_turn_then_slot_then_sequence()
    {
        // Each ID differs from the next in one field, and that field outranks every later field.
        var ordered = new[]
        {
            new StableEventId(1, 9, TurnSlot.TurnEnd, ulong.MaxValue),
            new StableEventId(2, 1, TurnSlot.Phase(1), 0),
            new StableEventId(2, 2, TurnSlot.Phase(1), 0),
            new StableEventId(2, 2, TurnSlot.Phase(2), 0),
            new StableEventId(2, 2, TurnSlot.Phase(2), 1),
        };

        for (var i = 0; i + 1 < ordered.Length; i++)
        {
            Assert.True(ordered[i].CompareTo(ordered[i + 1]) < 0, $"ID {i} should sort before ID {i + 1}");
            Assert.True(ordered[i + 1].CompareTo(ordered[i]) > 0, $"ID {i + 1} should sort after ID {i}");
        }
    }

    // ---- Invalid IDs are refused ----

    [Fact]
    public void An_id_needs_a_turn_from_1_and_a_real_slot()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new StableEventId(1, 0, TurnSlot.Phase(1), 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new StableEventId(1, 1, default, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new EventIdSequence(1, 0, TurnSlot.Phase(1)));
        Assert.Throws<ArgumentOutOfRangeException>(() => new EventIdSequence(1, 1, default));
    }

    [Fact]
    public void The_default_id_is_not_valid()
    {
        Assert.False(default(StableEventId).IsValid);
        Assert.True(new StableEventId(0, 1, TurnSlot.Phase(1), 0).IsValid);
    }
}
