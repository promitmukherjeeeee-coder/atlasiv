using Atlasiv.Core.Randomness;
using Xunit;

namespace Atlasiv.Core.Tests.Randomness;

/// <summary>
/// The core generator is Philox4x64-10 (Salmon, Moraes, Dror and Shaw, "Parallel random
/// numbers: as easy as 1, 2, 3", SC 2011). These are the published known-answer vectors
/// from the reference implementation, Random123, file tests/kat_vectors (fetched
/// 25 Nov 2026 from github.com/DEShawResearch/random123). They come from outside this
/// project, so a match proves the generator is the standard one, bit for bit.
/// </summary>
public sealed class PhiloxTests
{
    [Theory]
    // counter (4 words), key (2 words), expected output (4 words)
    [InlineData(
        0x0000000000000000UL, 0x0000000000000000UL, 0x0000000000000000UL, 0x0000000000000000UL,
        0x0000000000000000UL, 0x0000000000000000UL,
        0x16554d9eca36314cUL, 0xdb20fe9d672d0fdcUL, 0xd7e772cee186176bUL, 0x7e68b68aec7ba23bUL)]
    [InlineData(
        0xffffffffffffffffUL, 0xffffffffffffffffUL, 0xffffffffffffffffUL, 0xffffffffffffffffUL,
        0xffffffffffffffffUL, 0xffffffffffffffffUL,
        0x87b092c3013fe90bUL, 0x438c3c67be8d0224UL, 0x9cc7d7c69cd777b6UL, 0xa09caebf594f0ba0UL)]
    [InlineData(
        0x243f6a8885a308d3UL, 0x13198a2e03707344UL, 0xa4093822299f31d0UL, 0x082efa98ec4e6c89UL,
        0x452821e638d01377UL, 0xbe5466cf34e90c6cUL,
        0xa528f45403e61d95UL, 0x38c72dbd566e9788UL, 0xa5a1610e72fd18b5UL, 0x57bd43b5e52b7fe6UL)]
    public void Philox4x64_10_matches_the_published_Random123_vectors(
        ulong c0, ulong c1, ulong c2, ulong c3, ulong k0, ulong k1,
        ulong e0, ulong e1, ulong e2, ulong e3)
    {
        var output = Philox4x64.Generate(new Block4x64(c0, c1, c2, c3), k0, k1);

        Assert.Equal(new Block4x64(e0, e1, e2, e3), output);
    }
}
