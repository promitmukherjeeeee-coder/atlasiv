using System;
using System.Numerics;
using Atlasiv.Core.Numerics;
using FsCheck;
using FsCheck.Fluent;
using Xunit;

namespace Atlasiv.Core.Tests.Numerics;

/// <summary>
/// Rounding to whole base units: round-half-to-even by default, and the stochastic
/// rounding hook, which takes a 64-bit uniform draw that J2's random streams will supply.
/// </summary>
public sealed class RoundingTests
{
    private const long One = 1L << 32;
    private const long Quarter = One / 4;
    private const long Half = One / 2;

    // ---- Round half to even ----

    [Theory]
    [InlineData(Half, 0L)]
    [InlineData(One + Half, 2L)]
    [InlineData(2 * One + Half, 2L)]
    [InlineData(-Half, 0L)]
    [InlineData(-(One + Half), -2L)]
    [InlineData(-(2 * One + Half), -2L)]
    [InlineData(One + Quarter, 1L)]
    [InlineData(One + 3 * Quarter, 2L)]
    [InlineData(-(One + 3 * Quarter), -2L)]
    [InlineData(Half + 1, 1L)]                     // just above a half rounds up
    [InlineData(Half - 1, 0L)]                     // just below a half rounds down
    [InlineData(long.MaxValue, 2_147_483_648L)]
    [InlineData(long.MinValue, -2_147_483_648L)]
    public void A_fixed_point_value_rounds_half_to_even(long raw, long expected)
    {
        Assert.Equal(expected, Rounding.HalfToEven(Q32.FromRaw(raw)));
    }

    [Theory]
    [InlineData(7L, Half, 4L)]                     // 3.5 -> 4
    [InlineData(5L, Half, 2L)]                     // 2.5 -> 2
    [InlineData(-7L, Half, -4L)]                   // -3.5 -> -4
    [InlineData(1_000_000_000L, 1_431_655_765L, 333_333_333L)] // 1e9 * (1/3 as Q32) = 333333333.2557...
    [InlineData(8_000_000_000L, Quarter, 2_000_000_000L)]       // more than 2^31 people is fine
    [InlineData(long.MaxValue, One, long.MaxValue)]             // times exactly 1
    public void A_count_times_a_rate_rounds_half_to_even(long count, long factorRaw, long expected)
    {
        Assert.Equal(expected, Rounding.MultiplyHalfToEven(count, Q32.FromRaw(factorRaw)));
    }

    [Theory]
    [InlineData(10L, 3L, 4L, 8L)]                  // 7.5 -> 8
    [InlineData(10L, 1L, 4L, 2L)]                  // 2.5 -> 2
    [InlineData(-10L, 3L, 4L, -8L)]                // -7.5 -> -8
    [InlineData(long.MaxValue, long.MaxValue, long.MaxValue, long.MaxValue)] // needs the 128-bit intermediate
    public void Multiply_then_divide_rounds_half_to_even(long a, long b, long divisor, long expected)
    {
        Assert.Equal(expected, Rounding.MultiplyDivideHalfToEven(a, b, divisor));
    }

    [Fact]
    public void Whole_number_overflow_is_a_simulation_error()
    {
        Assert.Throws<SimulationOverflowException>(() => Rounding.MultiplyHalfToEven(long.MaxValue, Q32.FromInt(2)));
        Assert.Throws<SimulationOverflowException>(() => Rounding.MultiplyDivideHalfToEven(long.MaxValue, 2, 1));
        Assert.Throws<SimulationOverflowException>(() => Rounding.MultiplyStochastic(long.MaxValue, Q32.FromInt(2), 0));
        Assert.Throws<DivideByZeroException>(() => Rounding.MultiplyDivideHalfToEven(1, 1, 0));
    }

    [Fact]
    public void Multiply_then_divide_matches_the_exact_reference() =>
        NumericGen.Check(Prop.ForAll(
            Arb.From(from a in NumericGen.Raw() from b in NumericGen.Raw() from d in NumericGen.Raw() select (a, b, d)),
            t =>
            {
                if (t.d == 0)
                {
                    return true;
                }

                long? actual;
                try
                {
                    actual = Rounding.MultiplyDivideHalfToEven(t.a, t.b, t.d);
                }
                catch (SimulationOverflowException)
                {
                    actual = null;
                }

                return actual == NumericGen.InRange(NumericGen.DivideHalfToEven((BigInteger)t.a * t.b, t.d));
            }));

    // ---- Stochastic rounding hook ----

    // 2.25 rounds up to 3 exactly when the top 32 bits of the draw are below 0.25 * 2^32 = 2^30.
    [Theory]
    [InlineData(0UL, 3L)]
    [InlineData(((1UL << 30) - 1) << 32 | 0xFFFF_FFFFUL, 3L)]
    [InlineData((1UL << 30) << 32, 2L)]
    [InlineData(ulong.MaxValue, 2L)]
    public void A_positive_value_rounds_up_with_probability_equal_to_its_fraction(ulong uniform, long expected)
    {
        Assert.Equal(expected, Rounding.Stochastic(Q32.FromRaw(2 * One + Quarter), uniform));
    }

    // -2.25 = -3 + 0.75 rounds up to -2 exactly when the top 32 bits are below 0.75 * 2^32.
    [Theory]
    [InlineData(0UL, -2L)]
    [InlineData((3UL << 30) - 1 << 32, -2L)]
    [InlineData((3UL << 30) << 32, -3L)]
    public void A_negative_value_rounds_toward_the_next_integer_up_with_its_fraction(ulong uniform, long expected)
    {
        Assert.Equal(expected, Rounding.Stochastic(Q32.FromRaw(-(2 * One + Quarter)), uniform));
    }

    [Theory]
    [InlineData(0UL)]
    [InlineData(ulong.MaxValue)]
    public void A_whole_value_never_moves(ulong uniform)
    {
        Assert.Equal(5L, Rounding.Stochastic(Q32.FromInt(5), uniform));
        Assert.Equal(4_000_000_000L, Rounding.MultiplyStochastic(8_000_000_000L, Q32.FromRaw(Half), uniform));
    }

    [Theory]
    [InlineData(0UL, 3L)]                          // 10 * 0.25 = 2.5: draws below one half round up
    [InlineData((1UL << 31) - 1 << 32, 3L)]
    [InlineData((1UL << 31) << 32, 2L)]
    public void A_count_times_a_rate_rounds_stochastically_on_its_exact_fraction(ulong uniform, long expected)
    {
        Assert.Equal(expected, Rounding.MultiplyStochastic(10, Q32.FromRaw(Quarter), uniform));
    }

    /// <summary>
    /// For any value with fraction f (in units of 2^-32), exactly f of the 2^32 possible
    /// top-32-bit draws round up: the draw f - 1 rounds up, the draw f rounds down, and the
    /// result never decreases as the draw falls. So the expected result equals the value.
    /// </summary>
    [Fact]
    public void Stochastic_rounding_is_unbiased_at_its_threshold() =>
        NumericGen.Check(Prop.ForAll(
            Arb.From(from x in NumericGen.Q32() from u in NumericGen.Raw() select (x, u: unchecked((ulong)u))),
            t =>
            {
                var floor = (long)(new BigInteger(t.x.Raw) >> 32);
                var fraction = (ulong)(t.x.Raw - (floor << 32));
                var low = t.u & 0xFFFF_FFFFUL;
                var result = Rounding.Stochastic(t.x, t.u);
                var resultAtThreshold = Rounding.Stochastic(t.x, fraction << 32 | low);
                var resultBelow = fraction == 0 ? floor : Rounding.Stochastic(t.x, (fraction - 1) << 32 | low);
                return (result == floor || result == floor + 1)
                    && resultAtThreshold == floor
                    && resultBelow == (fraction == 0 ? floor : floor + 1);
            }));

    [Fact]
    public void Stochastic_and_half_to_even_never_differ_by_more_than_one() =>
        NumericGen.Check(Prop.ForAll(
            Arb.From(from c in NumericGen.Raw(40) from f in NumericGen.Raw(50) from u in NumericGen.Raw()
                     select (c, f: Q32.FromRaw(f), u: unchecked((ulong)u))),
            t => Math.Abs(Rounding.MultiplyStochastic(t.c, t.f, t.u) - Rounding.MultiplyHalfToEven(t.c, t.f)) <= 1));
}
