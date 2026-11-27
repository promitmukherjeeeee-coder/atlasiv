using System;
using Atlasiv.Core.Time;

namespace Atlasiv.Core.Randomness;

/// <summary>
/// The address of one random stream: world seed, turn, phase or turn-end slot, substep,
/// system and entity (spec section 9 and clarification 3, ADR-006). Each draw from the
/// stream is a pure function of this address and its draw index, so draws never depend on
/// iteration order, on other entities, or on how many draws other streams made.
/// </summary>
/// <remarks>
/// <para>
/// Layout (ENG-008). The address and draw index fill Philox4x64-10's 128-bit key and
/// 256-bit counter with no hashing, one field per word, so distinct addresses always give
/// distinct generator inputs:
/// </para>
/// <list type="table">
/// <item><term>key word 0</term><description>world seed</description></item>
/// <item><term>key word 1</term><description>system ID in the low 32 bits; the high 32 bits are zero (reserved)</description></item>
/// <item><term>counter word 0</term><description>draw index divided by 4 (the block number)</description></item>
/// <item><term>counter word 1</term><description>entity ID</description></item>
/// <item><term>counter word 2</term><description>turn</description></item>
/// <item><term>counter word 3</term><description>slot in the high 32 bits, substep in the low 32 bits</description></item>
/// </list>
/// <para>
/// Each Philox call yields four 64-bit words; draw index <c>i</c> is word <c>i mod 4</c> of
/// block <c>i / 4</c>.
/// </para>
/// <para>
/// The default value (turn 0, slot 0) is not a valid address and cannot draw, nor can a
/// copy of it made with <c>with</c> that leaves either field unset.
/// </para>
/// </remarks>
public readonly record struct DrawAddress
{
    private readonly long _turn;
    private readonly TurnSlot _slot;
    private readonly int _substep;

    /// <summary>Creates a stream address.</summary>
    /// <param name="worldSeed">The campaign's world seed.</param>
    /// <param name="turn">The turn, counting from 1 at the epoch (spec clarification 17).</param>
    /// <param name="slot">The phase, or the turn end.</param>
    /// <param name="substep">The substep within the stage, counting from 0; 0 when the stage is not split.</param>
    /// <param name="systemId">The drawing system's ID.</param>
    /// <param name="entityId">The entity the draw is for (a placeholder 64-bit ID until Pass 2).</param>
    /// <exception cref="ArgumentOutOfRangeException">The turn is below 1, the slot is the default value, or the substep is negative.</exception>
    public DrawAddress(ulong worldSeed, long turn, TurnSlot slot, int substep, uint systemId, ulong entityId)
    {
        WorldSeed = worldSeed;
        Turn = turn;
        Slot = slot;
        Substep = substep;
        SystemId = systemId;
        EntityId = entityId;
    }

    /// <summary>The campaign's world seed.</summary>
    public ulong WorldSeed { get; init; }

    /// <summary>The turn, counting from 1.</summary>
    public long Turn
    {
        get => _turn;
        init
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, 1);
            _turn = value;
        }
    }

    /// <summary>The phase, or the turn end.</summary>
    public TurnSlot Slot
    {
        get => _slot;
        init
        {
            ArgumentOutOfRangeException.ThrowIfZero(value.Value);
            _slot = value;
        }
    }

    /// <summary>The substep, counting from 0.</summary>
    public int Substep
    {
        get => _substep;
        init
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            _substep = value;
        }
    }

    /// <summary>The drawing system's ID.</summary>
    public uint SystemId { get; init; }

    /// <summary>The entity the draw is for.</summary>
    public ulong EntityId { get; init; }

    /// <summary>Draw number <paramref name="drawIndex"/> of this stream: 64 uniformly distributed bits.</summary>
    /// <exception cref="InvalidOperationException">The address has no valid turn or slot (see <see cref="EnsureValid"/>).</exception>
    public ulong Draw(ulong drawIndex)
    {
        var input = GeneratorInput(drawIndex);
        return Philox4x64.Generate(input.Counter, input.Key0, input.Key1)[input.Word];
    }

    /// <summary>
    /// The Philox counter, key and output word that draw <paramref name="drawIndex"/> uses.
    /// Public so the layout can be checked and published; simulation code calls <see cref="Draw"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">The address has no valid turn or slot (see <see cref="EnsureValid"/>).</exception>
    public PhiloxInput GeneratorInput(ulong drawIndex)
    {
        EnsureValid();

        var counter = new Block4x64(
            drawIndex >> 2,
            EntityId,
            (ulong)_turn,
            ((ulong)_slot.Value << 32) | (uint)_substep);
        return new PhiloxInput(counter, WorldSeed, SystemId, (int)(drawIndex & 3));
    }

    /// <summary>
    /// Checks that the address can draw. The constructor and the property setters refuse bad
    /// values, but the default value, or a copy of it made with <c>with</c>, can still hold
    /// turn 0 or slot 0.
    /// </summary>
    /// <exception cref="InvalidOperationException">The turn is below 1 or the slot is the default value.</exception>
    public void EnsureValid()
    {
        if (_turn < 1 || _slot.Value == 0)
        {
            throw new InvalidOperationException(
                "This DrawAddress is not valid: it holds turn " + _turn.ToString(System.Globalization.CultureInfo.InvariantCulture)
                + " and slot value " + _slot.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)
                + ", but turns and phases count from 1. Build addresses with the constructor.");
        }
    }
}
