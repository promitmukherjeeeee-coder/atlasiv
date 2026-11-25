using System;

namespace Atlasiv.Core.Randomness;

/// <summary>
/// The Philox4x64-10 counter-based generator (Salmon, Moraes, Dror and Shaw, SC 2011),
/// exactly as in the reference implementation Random123. It maps a 256-bit counter and a
/// 128-bit key to 256 random bits. It keeps no state: the same counter and key always
/// give the same output, on every machine (Pass 1 spec section 9, ADR-006, ENG-008).
/// </summary>
/// <remarks>
/// For a fixed key the map from counter to output is a bijection, so two different
/// counters under the same key never give the same output block.
/// </remarks>
public static class Philox4x64
{
    /// <summary>The number of rounds: 10, the standard choice with a wide safety margin.</summary>
    public const int Rounds = 10;

    // Random123 constants: round multipliers and the Weyl key increments.
    private const ulong Multiplier0 = 0xD2E7_470E_E14C_6C93UL;
    private const ulong Multiplier1 = 0xCA5A_8263_9512_1157UL;
    private const ulong Weyl0 = 0x9E37_79B9_7F4A_7C15UL;
    private const ulong Weyl1 = 0xBB67_AE85_84CA_A73BUL;

    /// <summary>The output block for <paramref name="counter"/> under the key (<paramref name="key0"/>, <paramref name="key1"/>).</summary>
    public static Block4x64 Generate(Block4x64 counter, ulong key0, ulong key1)
    {
        var (c0, c1, c2, c3) = counter;

        // Philox is defined on wrapping 64-bit arithmetic; the wrap is intended here (ENG-003).
        unchecked
        {
            for (var round = 0; round < Rounds; round++)
            {
                if (round > 0)
                {
                    key0 += Weyl0;
                    key1 += Weyl1;
                }

                var product0 = (UInt128)Multiplier0 * c0;
                var product1 = (UInt128)Multiplier1 * c2;
                var hi0 = (ulong)(product0 >> 64);
                var hi1 = (ulong)(product1 >> 64);
                (c0, c1, c2, c3) = (hi1 ^ c1 ^ key0, (ulong)product1, hi0 ^ c3 ^ key1, (ulong)product0);
            }
        }

        return new Block4x64(c0, c1, c2, c3);
    }
}
