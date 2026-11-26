using System;

namespace Atlasiv.Core.Time;

/// <summary>
/// Where in a turn something happens: one of the turn's phases, or the turn end. The turn end
/// has its own fixed slot after every phase (spec clarification 3), in both random-draw keys
/// and stable event IDs, so turn-end draws and events can never repeat a phase's.
/// </summary>
/// <remarks>
/// The default value has <see cref="Value"/> 0, which is not a valid slot;
/// <see cref="Randomness.DrawAddress"/> and <see cref="Events.StableEventId"/> refuse it.
/// Named <c>DrawSlot</c> until J5, when event IDs began to share it (ENG-012).
/// </remarks>
public readonly record struct TurnSlot
{
    /// <summary>The slot number used for the turn end: the largest 32-bit value, above every phase number.</summary>
    public const uint TurnEndValue = uint.MaxValue;

    private TurnSlot(uint value) => Value = value;

    /// <summary>The turn-end slot.</summary>
    public static TurnSlot TurnEnd => new(TurnEndValue);

    /// <summary>The slot number as it enters random-draw keys and event IDs: the phase number, or <see cref="TurnEndValue"/>.</summary>
    public uint Value { get; }

    /// <summary>True for the turn-end slot.</summary>
    public bool IsTurnEnd => Value == TurnEndValue;

    /// <summary>The slot of phase <paramref name="number"/> of a turn. Phases count from 1 (spec clarification 16).</summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="number"/> is below 1.</exception>
    public static TurnSlot Phase(int number)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(number, 1);
        return new TurnSlot((uint)number);
    }

    /// <inheritdoc/>
    public override string ToString() => IsTurnEnd ? "turn end" : "phase " + Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
}
