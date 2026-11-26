using System;

namespace Atlasiv.Core.Events;

/// <summary>
/// An event on the event queue: its ordering key, its interruption class and what it carries
/// (Pass 1 spec section 5). Events are created only when due.
/// </summary>
/// <remarks>
/// The interruption class travels with the event but is not part of the ordering key: whether
/// an event stops Advance Until never changes when it resolves.
/// </remarks>
/// <typeparam name="TPayload">What the event carries. Pass 1 uses dummy payloads; real event types arrive with their systems.</typeparam>
public sealed record SimulationEvent<TPayload>
{
    /// <summary>Creates an event.</summary>
    /// <param name="key">The ordering key: timestamp, stage, priority and stable ID.</param>
    /// <param name="interruption">Whether the event resolves on its own or halts Advance Until.</param>
    /// <param name="payload">What the event carries.</param>
    /// <exception cref="ArgumentException">The key has no valid stage or stable ID.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The interruption class is not one of the defined values.</exception>
    public SimulationEvent(EventKey key, InterruptionClass interruption, TPayload payload)
    {
        if (!key.IsValid)
        {
            throw new ArgumentException(
                "An event needs a valid stage and stable ID; build them with ResolutionStage and StableEventId (or EventIdSequence). Got " + key + ".",
                nameof(key));
        }

        if (interruption is not (InterruptionClass.Informational or InterruptionClass.DecisionCritical))
        {
            throw new ArgumentOutOfRangeException(nameof(interruption), interruption, "An event must be informational or decision-critical.");
        }

        Key = key;
        Interruption = interruption;
        Payload = payload;
    }

    /// <summary>The ordering key.</summary>
    public EventKey Key { get; }

    /// <summary>Whether the event resolves on its own or halts Advance Until.</summary>
    public InterruptionClass Interruption { get; }

    /// <summary>What the event carries.</summary>
    public TPayload Payload { get; }
}
