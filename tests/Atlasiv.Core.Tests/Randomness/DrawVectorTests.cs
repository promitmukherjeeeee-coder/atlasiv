using Atlasiv.Core.Randomness;
using Atlasiv.Core.Time;
using Xunit;

namespace Atlasiv.Core.Tests.Randomness;

/// <summary>
/// Published test vectors for keyed draws (ENG-008). The expected values were computed by a
/// separate Python implementation of Philox4x64-10 and the key layout, itself checked
/// against the Random123 vectors first. Any change to the generator or the layout changes
/// every replay hash, so these must only ever change by a deliberate, reviewed decision.
/// </summary>
public sealed class DrawVectorTests
{
    private const int TurnEnd = -1; // stands for TurnSlot.TurnEnd in the table below

    [Theory]
    // seed, turn, slot, substep, system, entity, draw index, expected draw
    [InlineData(0UL, 1L, 1, 0, 0U, 0UL, 0UL, 0x8b42ce270989c8fdUL)]
    [InlineData(0UL, 1L, 1, 0, 0U, 0UL, 1UL, 0x3a2f759ee499c30fUL)]
    [InlineData(0UL, 1L, 1, 0, 0U, 0UL, 2UL, 0x598c2e7e60798ef5UL)]
    [InlineData(0UL, 1L, 1, 0, 0U, 0UL, 3UL, 0x5d8933355f4de97aUL)]
    [InlineData(0UL, 1L, 1, 0, 0U, 0UL, 4UL, 0x7e7e2e2f043a1a79UL)]
    [InlineData(0x5EED000000000001UL, 12L, 3, 0, 7U, 42UL, 0UL, 0x9f25e9b5a0ebdfdeUL)]
    [InlineData(0x5EED000000000001UL, 12L, 3, 1, 7U, 42UL, 0UL, 0xbf157b65f5c5273aUL)]
    [InlineData(0x5EED000000000001UL, 12L, 20, 0, 7U, 42UL, 0UL, 0xd92e6fefaab0d665UL)]
    [InlineData(0x5EED000000000001UL, 12L, TurnEnd, 0, 7U, 42UL, 0UL, 0x397494161b9961feUL)]
    [InlineData(0x5EED000000000001UL, 12L, TurnEnd, 3, 7U, 42UL, 0UL, 0x666175948bdbda85UL)]
    [InlineData(0x5EED000000000001UL, 1000L, 5, 0, uint.MaxValue, ulong.MaxValue, ulong.MaxValue, 0x40fbaa7b6de70d42UL)]
    [InlineData(ulong.MaxValue, long.MaxValue, int.MaxValue, int.MaxValue, uint.MaxValue, ulong.MaxValue, 1234567UL, 0x1fa06cfde0fcce45UL)]
    public void Keyed_draws_match_the_published_vectors(
        ulong seed, long turn, int slot, int substep, uint system, ulong entity, ulong drawIndex, ulong expected)
    {
        var key = new DrawAddress(seed, turn, slot == TurnEnd ? TurnSlot.TurnEnd : TurnSlot.Phase(slot), substep, system, entity);

        Assert.Equal(expected, key.Draw(drawIndex));
    }
}
