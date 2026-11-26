using System;
using System.Collections.Generic;
using System.Linq;
using Atlasiv.Core.Events;
using Atlasiv.Core.Scheduling;
using Atlasiv.Core.Time;
using FsCheck;
using FsCheck.Fluent;
using Xunit;

namespace Atlasiv.Core.Tests.Events;

/// <summary>
/// The event queue: a binary heap keyed by the full ordering tuple, resolving events in the
/// same order however they were inserted (spec sections 5 and 13).
/// </summary>
public sealed class EventQueueTests
{
    private static readonly Instant T = new(500);

    private static SimulationEvent<string> Event(
        long seconds,
        ulong sequence,
        InterruptionClass interruption = InterruptionClass.Informational,
        string payload = "") =>
        new(
            new EventKey(new Instant(seconds), ResolutionStage.Phase(4), 0, new StableEventId(1, 3, TurnSlot.Phase(2), sequence)),
            interruption,
            payload);

    private static SimulationEvent<int> Wrap(EventKey key, int payload) => new(key, InterruptionClass.Informational, payload);

    private static List<EventKey> Drain<T>(EventQueue<T> queue)
    {
        var order = new List<EventKey>();
        while (queue.TryDequeue(out var next))
        {
            order.Add(next.Key);
        }

        return order;
    }

    // ---- Same events, any insertion order, same resolution order ----

    [Fact]
    public void The_same_events_inserted_in_shuffled_orders_always_resolve_in_the_same_order()
    {
        var cases =
            from keys in EventGen.KeysWithDistinctIds()
            from first in Gen.Shuffle(keys)
            from second in Gen.Shuffle(keys)
            select (Keys: keys, First: first, Second: second);

        EventGen.Check(Prop.ForAll(cases.ToArbitrary(), c =>
        {
            var a = new EventQueue<int>();
            var b = new EventQueue<int>();
            foreach (var key in c.First)
            {
                a.Enqueue(Wrap(key, 0));
            }

            foreach (var key in c.Second)
            {
                b.Enqueue(Wrap(key, 0));
            }

            var expected = c.Keys.ToArray();
            Array.Sort(expected);
            return Drain(a).SequenceEqual(expected) && Drain(b).SequenceEqual(expected);
        }));
    }

    [Fact]
    public void Each_dequeue_returns_the_earliest_event_still_queued_when_inserts_and_dequeues_interleave()
    {
        // Each operation is either an insert (a key) or a dequeue (null). The model is a plain
        // list searched for its smallest key.
        var operation = Gen.Frequency(
            (3, EventGen.Key().Select(k => (EventKey?)k)),
            (1, Gen.Constant((EventKey?)null)));

        EventGen.Check(Prop.ForAll(Gen.ListOf(operation).ToArbitrary(), operations =>
        {
            var queue = new EventQueue<int>();
            var model = new List<EventKey>();
            foreach (var op in operations)
            {
                if (op is { } key)
                {
                    if (model.Exists(k => k.Id == key.Id))
                    {
                        continue;
                    }

                    queue.Enqueue(Wrap(key, 0));
                    model.Add(key);
                }
                else if (model.Count == 0)
                {
                    if (queue.TryDequeue(out _))
                    {
                        return false;
                    }
                }
                else
                {
                    var earliest = model.Min();
                    model.Remove(earliest);
                    if (queue.Dequeue().Key != earliest)
                    {
                        return false;
                    }
                }

                if (queue.Count != model.Count)
                {
                    return false;
                }
            }

            return true;
        }));
    }

    [Fact]
    public void Events_resolve_in_timestamp_order_with_ties_broken_by_the_rest_of_the_key()
    {
        var queue = new EventQueue<string>();
        queue.Enqueue(Event(seconds: 30, sequence: 0, payload: "c"));
        queue.Enqueue(Event(seconds: 10, sequence: 5, payload: "b"));
        queue.Enqueue(Event(seconds: 10, sequence: 2, payload: "a"));
        queue.Enqueue(Event(seconds: 40, sequence: 1, payload: "d"));

        var payloads = new List<string>();
        while (queue.TryDequeue(out var next))
        {
            payloads.Add(next.Payload);
        }

        Assert.Equal(["a", "b", "c", "d"], payloads);
    }

    // ---- No two queued events share a stable ID ----

    [Fact]
    public void An_event_whose_stable_id_is_already_queued_is_refused()
    {
        var queue = new EventQueue<string>();
        queue.Enqueue(Event(seconds: 10, sequence: 7));

        var error = Assert.Throws<InvalidOperationException>(() => queue.Enqueue(Event(seconds: 99, sequence: 7)));
        Assert.Contains("#7", error.Message, StringComparison.Ordinal);
        Assert.Equal(1, queue.Count);
    }

    [Fact]
    public void A_stable_id_can_be_queued_again_once_its_event_has_left_the_queue()
    {
        var queue = new EventQueue<string>();
        queue.Enqueue(Event(seconds: 10, sequence: 7));
        queue.Dequeue();

        queue.Enqueue(Event(seconds: 20, sequence: 7));

        Assert.Equal(1, queue.Count);
    }

    // ---- The interruption class travels with the event ----

    [Fact]
    public void Each_event_carries_its_interruption_class_through_the_queue()
    {
        var queue = new EventQueue<string>();
        queue.Enqueue(Event(seconds: 10, sequence: 0, InterruptionClass.Informational, "harvest report"));
        queue.Enqueue(Event(seconds: 20, sequence: 1, InterruptionClass.DecisionCritical, "war declared"));

        var first = queue.Dequeue();
        var second = queue.Dequeue();

        Assert.Equal(("harvest report", InterruptionClass.Informational), (first.Payload, first.Interruption));
        Assert.Equal(("war declared", InterruptionClass.DecisionCritical), (second.Payload, second.Interruption));
    }

    [Fact]
    public void An_event_must_have_a_defined_interruption_class()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Event(seconds: 0, sequence: 0, default(InterruptionClass)));
        Assert.Throws<ArgumentOutOfRangeException>(() => Event(seconds: 0, sequence: 0, (InterruptionClass)3));
    }

    [Fact]
    public void An_event_must_have_a_valid_stage_and_stable_id()
    {
        var id = new StableEventId(1, 1, TurnSlot.Phase(1), 0);
        Assert.Throws<ArgumentException>(() =>
            new SimulationEvent<string>(new EventKey(T, default, 0, id), InterruptionClass.Informational, ""));
        Assert.Throws<ArgumentException>(() =>
            new SimulationEvent<string>(new EventKey(T, ResolutionStage.Phase(1), 0, default), InterruptionClass.Informational, ""));
    }

    // ---- Empty queue and peeking ----

    [Fact]
    public void An_empty_queue_has_nothing_to_peek_at_or_dequeue()
    {
        var queue = new EventQueue<string>();

        Assert.Equal(0, queue.Count);
        Assert.False(queue.TryPeek(out _));
        Assert.False(queue.TryDequeue(out _));
        Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
    }

    [Fact]
    public void Peeking_shows_the_next_event_without_removing_it()
    {
        var queue = new EventQueue<string>();
        queue.Enqueue(Event(seconds: 20, sequence: 0, payload: "later"));
        queue.Enqueue(Event(seconds: 10, sequence: 1, payload: "sooner"));

        Assert.True(queue.TryPeek(out var next));
        Assert.Equal("sooner", next.Payload);
        Assert.Equal(2, queue.Count);
        Assert.Equal("sooner", queue.Dequeue().Payload);
    }
}
