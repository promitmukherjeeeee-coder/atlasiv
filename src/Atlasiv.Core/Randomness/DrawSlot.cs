using System;

namespace Atlasiv.Core.Randomness;

/// <summary>
/// Where in a turn a draw is made: one of the turn's phases, or the turn end. The turn end
/// has its own fixed slot after every phase (spec clarification 3), so turn-end draws can
/// never repeat a phase's draws.
/// </summary>
/// <remarks>
/// The default value has <see cref="Value"/> 0, which is not a valid slot; a
/// <see cref="DrawAddress"/> refuses it.
/// </remarks>
public readonly record struct DrawSlot
{
    /// <summary>The slot number used for the turn end: the largest 32-bit value, above every phase number.</summary>
    public const uint TurnEndValue = uint.MaxValue;

    private DrawSlot(uint value) => Value = value;

    /// <summary>The turn-end slot.</summary>
    public static DrawSlot TurnEnd => new(TurnEndValue);

    /// <summary>The slot number as it enters the generator: the phase number, or <see cref="TurnEndValue"/>.</summary>
    public uint Value { get; }

    /// <summary>True for the turn-end slot.</summary>
    public bool IsTurnEnd => Value == TurnEndValue;

    /// <summary>The slot of phase <paramref name="number"/> of a turn. Phases count from 1 (spec clarification 16).</summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="number"/> is below 1.</exception>
    public static DrawSlot Phase(int number)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(number, 1);
        return new DrawSlot((uint)number);
    }

    /// <inheritdoc/>
    public override string ToString() => IsTurnEnd ? "turn end" : "phase " + Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
}
