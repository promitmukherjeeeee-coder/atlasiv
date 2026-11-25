using System;
using Atlasiv.Core.Numerics;
using Xunit;

namespace Atlasiv.Core.Tests.Numerics;

/// <summary>
/// The Q32.32 fixed-point type: a signed 64-bit raw value read as raw / 2^32.
/// Expected raw values are worked out by hand from that definition.
/// </summary>
public sealed class Q32Tests
{
    private const long OneRaw = 1L << 32;

    [Theory]
    [InlineData(0, 0L)]
    [InlineData(1, OneRaw)]
    [InlineData(-1, -OneRaw)]
    [InlineData(2_147_483_647, 2_147_483_647L << 32)]
    [InlineData(-2_147_483_648, long.MinValue)]
    public void An_integer_becomes_that_many_units_of_2_to_the_32(long value, long expectedRaw)
    {
        Assert.Equal(expectedRaw, Q32.FromInt(value).Raw);
    }

    [Theory]
    [InlineData(2_147_483_648L)]
    [InlineData(-2_147_483_649L)]
    [InlineData(long.MaxValue)]
    public void An_integer_outside_the_range_is_an_overflow(long value)
    {
        Assert.Throws<SimulationOverflowException>(() => Q32.FromInt(value));
    }

    [Theory]
    [InlineData(1, 2, OneRaw / 2)]
    [InlineData(-3, 4, -(3 * OneRaw / 4))]
    [InlineData(1, 3, 1_431_655_765L)]   // 4294967296 / 3 = 1431655765.33, rounds down
    [InlineData(2, 3, 2_863_311_531L)]   // 8589934592 / 3 = 2863311530.67, rounds up
    [InlineData(1, 1L << 33, 0L)]        // exactly half a raw unit: ties to even (0)
    [InlineData(3, 1L << 33, 2L)]        // exactly 1.5 raw units: ties to even (2)
    [InlineData(-3, 1L << 33, -2L)]      // ties to even on the negative side too
    public void A_ratio_rounds_half_to_even(long numerator, long denominator, long expectedRaw)
    {
        Assert.Equal(expectedRaw, Q32.FromRatio(numerator, denominator).Raw);
    }

    [Fact]
    public void A_ratio_with_zero_denominator_is_an_error()
    {
        Assert.Throws<DivideByZeroException>(() => Q32.FromRatio(1, 0));
    }

    [Theory]
    [InlineData(0L, "0")]
    [InlineData(OneRaw, "1")]
    [InlineData(-OneRaw, "-1")]
    [InlineData(OneRaw / 2, "0.5")]
    [InlineData(-(OneRaw + OneRaw / 4), "-1.25")]
    [InlineData(1L, "0.00000000023283064365386962890625")]
    [InlineData(long.MaxValue, "2147483647.99999999976716935634613037109375")]
    [InlineData(long.MinValue, "-2147483648")]
    public void Text_shows_the_exact_decimal_value(long raw, string expected)
    {
        Assert.Equal(expected, Q32.FromRaw(raw).ToString());
    }
}
