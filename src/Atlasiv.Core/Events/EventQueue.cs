using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Atlasiv.Core.Events;

/// <summary>
/// The deterministic event queue: a binary min-heap keyed by the full ordering tuple
/// (timestamp, stage, priority, stable ID) (Pass 1 spec section 5). Events leave in ascending
/// key order, so the same events give the same resolution order however they were inserted.
/// </summary>
/// <remarks>
/// <para>
/// No two events in the queue share a stable ID: <see cref="Enqueue"/> refuses a second one.
/// Because the ID is the key's last field, no two queued events ever compare equal, and the
/// heap's order is total. The queue only knows the IDs it currently holds; making each ID once
/// is the job of <see cref="EventIdSequence"/>.
/// </para>
/// <para>
/// Enqueue and dequeue take O(log n) time. The ID check uses a hash set for membership only;
/// nothing ever iterates it (hard rule 5). Not safe to share between threads.
/// </para>
/// </remarks>
/// <typeparam name="TPayload">What the events carry.</typeparam>
[SuppressMessage(
    "Naming",
    "CA1711:Identifiers should not have incorrect suffix",
    Justification = "\"Event queue\" is the spec's name for this (section 5), and it is a queue.")]
public sealed class EventQueue<TPayload>
{
    private readonly List<SimulationEvent<TPayload>> _heap = [];
    private readonly HashSet<StableEventId> _queuedIds = [];

    /// <summary>The number of events waiting.</summary>
    public int Count => _heap.Count;

    /// <summary>Adds an event.</summary>
    /// <exception cref="ArgumentNullException"><paramref name="simulationEvent"/> is null.</exception>
    /// <exception cref="InvalidOperationException">An event with the same stable ID is already queued.</exception>
    public void Enqueue(SimulationEvent<TPayload> simulationEvent)
    {
        ArgumentNullException.ThrowIfNull(simulationEvent);
        if (!_queuedIds.Add(simulationEvent.Key.Id))
        {
            throw new InvalidOperationException(
                "An event with stable ID (" + simulationEvent.Key.Id + ") is already queued. Stable IDs must be unique; "
                + "each system should take its IDs from one EventIdSequence per phase or turn end.");
        }

        _heap.Add(simulationEvent);
        SiftUp(_heap.Count - 1);
    }

    /// <summary>Shows the next event to resolve without removing it.</summary>
    /// <returns>False if the queue is empty.</returns>
    public bool TryPeek([MaybeNullWhen(false)] out SimulationEvent<TPayload> next)
    {
        if (_heap.Count == 0)
        {
            next = null;
            return false;
        }

        next = _heap[0];
        return true;
    }

    /// <summary>Removes and returns the next event to resolve.</summary>
    /// <returns>False if the queue is empty.</returns>
    public bool TryDequeue([MaybeNullWhen(false)] out SimulationEvent<TPayload> next)
    {
        if (_heap.Count == 0)
        {
            next = null;
            return false;
        }

        next = _heap[0];
        var lastIndex = _heap.Count - 1;
        _heap[0] = _heap[lastIndex];
        _heap.RemoveAt(lastIndex);
        if (_heap.Count > 0)
        {
            SiftDown(0);
        }

        _queuedIds.Remove(next.Key.Id);
        return true;
    }

    /// <summary>Removes and returns the next event to resolve.</summary>
    /// <exception cref="InvalidOperationException">The queue is empty.</exception>
    public SimulationEvent<TPayload> Dequeue() =>
        TryDequeue(out var next) ? next : throw new InvalidOperationException("The event queue is empty.");

    private bool Before(int a, int b) => _heap[a].Key.CompareTo(_heap[b].Key) < 0;

    private void Swap(int a, int b) => (_heap[a], _heap[b]) = (_heap[b], _heap[a]);

    private void SiftUp(int index)
    {
        while (index > 0)
        {
            var parent = (index - 1) / 2;
            if (!Before(index, parent))
            {
                return;
            }

            Swap(index, parent);
            index = parent;
        }
    }

    private void SiftDown(int index)
    {
        var count = _heap.Count;
        while (true)
        {
            var left = (2 * index) + 1;
            if (left >= count)
            {
                return;
            }

            var right = left + 1;
            var earliest = right < count && Before(right, left) ? right : left;
            if (!Before(earliest, index))
            {
                return;
            }

            Swap(index, earliest);
            index = earliest;
        }
    }
}
