using System;
using System.Numerics;
using Atlasiv.Core.Numerics;
using FsCheck;
using FsCheck.Fluent;
using Xunit;

namespace Atlasiv.Core.Tests.Numerics;

/// <summary>
/// Q32 arithmetic: published test vectors worked out by hand, overflow cases, and
/// FsCheck properties checked against an exact BigInteger reference.
/// </summary>
public sealed class Q32ArithmeticTests
{
    private const long One = 1L << 32;
    private const long Half = One / 2;

    // ---- Published test vectors (raw in, raw out) ----

    [Theory]
    [InlineData(One + Half, 2 * One + One / 4, 3 * One + 3 * One / 4)] // 1.5 + 2.25 = 3.75
    [InlineData(-One, One / 4, -3 * One / 4)]                          // -1 + 0.25 = -0.75
    [InlineData(long.MaxValue, long.MinValue, -1L)]                    // largest + most negative
    public void Addition_vectors(long a, long b, long expected)
    {
        Assert.Equal(expected, (Q32.FromRaw(a) + Q32.FromRaw(b)).Raw);
    }

    [Theory]
    [InlineData(3 * One, One + Half, One + Half)]       // 3 - 1.5 = 1.5
    [InlineData(0L, 1L, -1L)]                          // 0 - epsilon
    public void Subtraction_vectors(long a, long b, long expected)
    {
        Assert.Equal(expected, (Q32.FromRaw(a) - Q32.FromRaw(b)).Raw);
    }

    [Theory]
    [InlineData(Half, Half, One / 4)]                  // 0.5 * 0.5 = 0.25
    [InlineData(3 * One, -(One + Half), -(4 * One + Half))] // 3 * -1.5 = -4.5
    [InlineData(1L, Half, 0L)]                         // 0.5 raw units: ties to even, 0
    [InlineData(3L, Half, 2L)]                         // 1.5 raw units: ties to even, 2
    [InlineData(5L, Half, 2L)]                         // 2.5 raw units: ties to even, 2
    [InlineData(-3L, Half, -2L)]                       // -1.5 raw units: ties to even, -2
    [InlineData(1L, 1L, 0L)]                           // 2^-64 rounds to 0
    [InlineData(-(1L << 48), 1L << 47, long.MinValue)] // -2^16 * 2^15 = -2^31, the most negative value
    public void Multiplication_vectors(long a, long b, long expected)
    {
        Assert.Equal(expected, (Q32.FromRaw(a) * Q32.FromRaw(b)).Raw);
    }

    [Theory]
    [InlineData(One, 3 * One, 1_431_655_765L)]         // 1 / 3, rounds down
    [InlineData(2 * One, 3 * One, 2_863_311_531L)]     // 2 / 3, rounds up
    [InlineData(-One, 4 * One, -One / 4)]              // -1 / 4
    [InlineData(1L, 2 * One, 0L)]                      // 0.5 raw units: ties to even, 0
    [InlineData(3L, 2 * One, 2L)]                      // 1.5 raw units: ties to even, 2
    [InlineData(7 * One, Half, 14 * One)]              // 7 / 0.5 = 14
    public void Division_vectors(long a, long b, long expected)
    {
        Assert.Equal(expected, (Q32.FromRaw(a) / Q32.FromRaw(b)).Raw);
    }

    // ---- Overflow is a simulation error, never a wrap ----

    [Fact]
    public void Overflow_cases_raise_a_simulation_error()
    {
        Assert.Throws<SimulationOverflowException>(() => Q32.MaxValue + Q32.Epsilon);
        Assert.Throws<SimulationOverflowException>(() => Q32.MinValue - Q32.Epsilon);
        Assert.Throws<SimulationOverflowException>(() => -Q32.MinValue);
        Assert.Throws<SimulationOverflowException>(() => Q32.FromInt(1 << 16) * Q32.FromInt(1 << 15));
        Assert.Throws<SimulationOverflowException>(() => Q32.One / Q32.Epsilon);
    }

    [Fact]
    public void Dividing_by_zero_is_an_error()
    {
        Assert.Throws<DivideByZeroException>(() => Q32.One / Q32.Zero);
    }

    [Fact]
    public void Negation_flips_the_sign_exactly()
    {
        Assert.Equal(-One - Half, (-Q32.FromRaw(One + Half)).Raw);
        Assert.Equal(-long.MaxValue, (-Q32.MaxValue).Raw);
    }

    // ---- FsCheck properties ----

    [Fact]
    public void Addition_is_commutative_including_overflow() =>
        Check(Prop.ForAll(NumericGen.Pair(), p =>
            NumericGen.RawOrOverflow(() => p.A + p.B) == NumericGen.RawOrOverflow(() => p.B + p.A)));

    [Fact]
    public void Multiplication_is_commutative_including_overflow() =>
        Check(Prop.ForAll(NumericGen.Pair(), p =>
            NumericGen.RawOrOverflow(() => p.A * p.B) == NumericGen.RawOrOverflow(() => p.B * p.A)));

    [Fact]
    public void Addition_and_subtraction_are_exact_inverses() =>
        Check(Prop.ForAll(NumericGen.Pair(62, 62), p => (p.A + p.B - p.B) == p.A));

    [Fact]
    public void Addition_matches_the_exact_reference() =>
        Check(Prop.ForAll(NumericGen.Pair(), p =>
            NumericGen.RawOrOverflow(() => p.A + p.B)
            == NumericGen.InRange((BigInteger)p.A.Raw + p.B.Raw)));

    [Fact]
    public void Multiplication_matches_the_exact_reference_rounded_half_to_even() =>
        Check(Prop.ForAll(NumericGen.Pair(), p =>
            NumericGen.RawOrOverflow(() => p.A * p.B)
            == NumericGen.InRange(NumericGen.DivideHalfToEven((BigInteger)p.A.Raw * p.B.Raw, BigInteger.One << 32))));

    [Fact]
    public void Division_matches_the_exact_reference_rounded_half_to_even() =>
        Check(Prop.ForAll(NumericGen.Pair(), p =>
            p.B == Q32.Zero
            || NumericGen.RawOrOverflow(() => p.A / p.B)
               == NumericGen.InRange(NumericGen.DivideHalfToEven((BigInteger)p.A.Raw << 32, p.B.Raw))));

    /// <summary>
    /// Stated error: (a * b) / b differs from a by at most 0.5 / |b| + 0.5 raw units
    /// (one half-unit rounding in each step, the first scaled by 1 / b).
    /// </summary>
    [Fact]
    public void Multiply_then_divide_returns_within_the_stated_error() =>
        Check(Prop.ForAll(NumericGen.Pair(47, 47), p =>
        {
            if (p.B == Q32.Zero)
            {
                return true;
            }

            var diff = BigInteger.Abs((BigInteger)(p.A * p.B / p.B).Raw - p.A.Raw);
            var b = BigInteger.Abs(p.B.Raw);
            return 2 * diff * b <= (BigInteger.One << 32) + b;
        }));

    /// <summary>
    /// Stated error: (a / b) * b differs from a by at most 0.5 * |b| + 0.5 raw units.
    /// </summary>
    [Fact]
    public void Divide_then_multiply_returns_within_the_stated_error() =>
        Check(Prop.ForAll(NumericGen.Pair(47, 47), p =>
        {
            if (BigInteger.Abs(p.B.Raw) < One)
            {
                return true; // |b| < 1 can push a / b out of range; covered by the vectors
            }

            var diff = BigInteger.Abs((BigInteger)(p.A / p.B * p.B).Raw - p.A.Raw);
            return 2 * diff * One <= BigInteger.Abs(p.B.Raw) + One;
        }));

    private static void Check(Property property) => NumericGen.Check(property);
}
