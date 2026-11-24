using System;
using System.Numerics;
using Atlasiv.Core.Numerics;
using FsCheck;
using FsCheck.Fluent;
using Xunit;

namespace Atlasiv.Core.Tests.Numerics;

/// <summary>
/// Integer exp, log and sqrt on Q32. The published vectors below are the true values
/// rounded half to even to Q32, computed independently with 70-digit decimal arithmetic.
/// The properties compare against <see cref="ReferenceMath"/> on random inputs.
/// </summary>
/// <remarks>
/// Stated errors, in raw units of 2^-32:
/// sqrt is correctly rounded (error at most 1/2);
/// log is within 1 of the correctly rounded value;
/// exp is within 1 of the correctly rounded value, plus 2^-56 relative for large results
/// (so within 1 for every result below 2^24).
/// </remarks>
public sealed class FixedMathTests
{
    private const long One = 1L << 32;

    // ---- Published test vectors ----

    [Theory]
    [InlineData(0L, 0L)]
    [InlineData(4294967296L, 4294967296L)]              // sqrt 1 = 1
    [InlineData(8589934592L, 6074001000L)]              // sqrt 2 = 1.41421356238...
    [InlineData(17179869184L, 8589934592L)]             // sqrt 4 = 2
    [InlineData(1073741824L, 2147483648L)]              // sqrt 0.25 = 0.5
    [InlineData(2147483648L, 3037000500L)]              // sqrt 0.5
    [InlineData(12884901888L, 7439101574L)]             // sqrt 3
    [InlineData(42949672960L, 13581879131L)]            // sqrt 10
    [InlineData(1L, 65536L)]                            // sqrt 2^-32 = 2^-16
    [InlineData(2L, 92682L)]
    [InlineData(long.MaxValue, 199032864766430L)]       // sqrt of the largest value
    [InlineData(4294967296000000L, 4294967296000L)]     // sqrt 1e6 = 1000
    public void Sqrt_vectors(long input, long expected)
    {
        Assert.Equal(expected, FixedMath.Sqrt(Q32.FromRaw(input)).Raw);
    }

    [Theory]
    [InlineData(0L, 4294967296L)]                       // exp 0 = 1
    [InlineData(4294967296L, 11674931555L)]             // exp 1 = e
    [InlineData(-4294967296L, 1580030169L)]             // exp -1
    [InlineData(2147483648L, 7081203938L)]              // exp 0.5
    [InlineData(-2147483648L, 2605029347L)]             // exp -0.5
    [InlineData(8589934592L, 31735754293L)]             // exp 2
    [InlineData(42949672960L, 94602950235157L)]         // exp 10
    [InlineData(-42949672960L, 194991L)]                // exp -10
    [InlineData(85899345920L, 2083768647442498562L)]    // exp 20
    [InlineData(-85899345920L, 9L)]                     // exp -20
    [InlineData(-94489280512L, 1L)]                     // exp -22
    [InlineData(-96636764160L, 1L)]                     // exp -22.5
    [InlineData(-98784247808L, 0L)]                     // exp -23 rounds to 0
    [InlineData(4295L, 4294971591L)]                    // exp 1e-6
    [InlineData(-4295L, 4294963001L)]                   // exp -1e-6
    [InlineData(1L, 4294967297L)]                       // exp of the smallest step
    [InlineData(-1L, 4294967295L)]
    [InlineData(long.MinValue, 0L)]                     // exp of the most negative value
    public void Exp_vectors(long input, long expected)
    {
        Assert.Equal(expected, FixedMath.Exp(Q32.FromRaw(input)).Raw);
    }

    /// <summary>Large results: the stated bound allows 2^-56 relative error.</summary>
    [Theory]
    [InlineData(90194313216L, 5664270449055626585L)]    // exp 21
    [InlineData(92255897518L, 9153882479645939135L)]    // exp 21.48, close to the largest value
    public void Exp_vectors_with_large_results(long input, long expected)
    {
        var actual = FixedMath.Exp(Q32.FromRaw(input)).Raw;
        Assert.InRange(actual - expected, -(1 + (expected >> 56)), 1 + (expected >> 56));
    }

    [Theory]
    [InlineData(4294967296L, 0L)]                       // ln 1 = 0
    [InlineData(8589934592L, 2977044472L)]              // ln 2
    [InlineData(2147483648L, -2977044472L)]             // ln 0.5
    [InlineData(11674931555L, 4294967296L)]             // ln e (as Q32) = 1
    [InlineData(42949672960L, 9889527671L)]             // ln 10
    [InlineData(1L, -95265423098L)]                     // ln 2^-32
    [InlineData(long.MaxValue, 92288378626L)]           // ln of the largest value
    [InlineData(12884901888L, 4718503851L)]             // ln 3
    [InlineData(429496730L, -9889527667L)]              // ln 0.1 (as Q32)
    [InlineData(4294967296000000L, 59337166024L)]       // ln 1e6
    [InlineData(4294967297L, 1L)]                       // ln(1 + 2^-32)
    [InlineData(4294967295L, -1L)]                      // ln(1 - 2^-32)
    public void Log_vectors(long input, long expected)
    {
        Assert.Equal(expected, FixedMath.Log(Q32.FromRaw(input)).Raw);
    }

    // ---- Domain and range errors ----

    [Fact]
    public void Exp_above_about_21_49_overflows()
    {
        Assert.Throws<SimulationOverflowException>(() => FixedMath.Exp(Q32.FromInt(22)));
        Assert.Throws<SimulationOverflowException>(() => FixedMath.Exp(Q32.MaxValue));
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(-1L)]
    [InlineData(long.MinValue)]
    public void Log_of_zero_or_a_negative_value_is_an_error(long raw)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FixedMath.Log(Q32.FromRaw(raw)));
    }

    [Fact]
    public void Sqrt_of_a_negative_value_is_an_error()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FixedMath.Sqrt(Q32.FromRaw(-1)));
    }

    // ---- Properties against the high-precision reference ----

    [Fact]
    public void Sqrt_is_correctly_rounded() =>
        NumericGen.Check(Prop.ForAll(Arb.From(NumericGen.NonNegativeRaw()), raw =>
            FixedMath.Sqrt(Q32.FromRaw(raw)).Raw == ReferenceMath.Sqrt(raw)));

    [Fact]
    public void Log_is_within_one_unit_of_the_true_value() =>
        NumericGen.Check(Prop.ForAll(Arb.From(NumericGen.NonNegativeRaw().Where(r => r > 0)), raw =>
            BigInteger.Abs(FixedMath.Log(Q32.FromRaw(raw)).Raw - ReferenceMath.Log(raw)) <= 1));

    [Fact]
    public void Exp_is_within_the_stated_error_of_the_true_value() =>
        NumericGen.Check(Prop.ForAll(Arb.From(ExpInput), raw =>
        {
            if (raw < -30 * One)
            {
                return FixedMath.Exp(Q32.FromRaw(raw)) == Q32.Zero; // exp(-30) is below 2^-43
            }

            var expected = raw >= 22 * One ? BigInteger.One << 64 : ReferenceMath.Exp(raw);
            if (expected > long.MaxValue)
            {
                return NumericGen.RawOrOverflow(() => FixedMath.Exp(Q32.FromRaw(raw))) is null;
            }

            return BigInteger.Abs(FixedMath.Exp(Q32.FromRaw(raw)).Raw - expected) <= 1 + (expected >> 56);
        }));

    [Fact]
    public void Exp_log_and_sqrt_never_decrease() =>
        NumericGen.Check(Prop.ForAll(
            Arb.From(from a in ExpInput from b in ExpInput select (Low: Math.Min(a, b), High: Math.Max(a, b))),
            p =>
            {
                var exp = NumericGen.RawOrOverflow(() => FixedMath.Exp(Q32.FromRaw(p.High))) is null
                    || FixedMath.Exp(Q32.FromRaw(p.Low)) <= FixedMath.Exp(Q32.FromRaw(p.High));
                var positive = p.Low > 0;
                var log = !positive || FixedMath.Log(Q32.FromRaw(p.Low)) <= FixedMath.Log(Q32.FromRaw(p.High));
                var sqrt = p.Low < 0 || FixedMath.Sqrt(Q32.FromRaw(p.Low)) <= FixedMath.Sqrt(Q32.FromRaw(p.High));
                return exp && log && sqrt;
            }));

    /// <summary>
    /// Stated error: exp(log x) is within x * 2^-31 + 2 raw units of x
    /// (log's error of at most about 2^-32 becomes a relative error after exp).
    /// </summary>
    [Fact]
    public void Exp_of_log_returns_within_the_stated_error() =>
        NumericGen.Check(Prop.ForAll(Arb.From(NumericGen.NonNegativeRaw().Where(r => r > 0)), raw =>
        {
            var bound = (raw >> 31) + 2;
            var roundTrip = NumericGen.RawOrOverflow(() => FixedMath.Exp(FixedMath.Log(Q32.FromRaw(raw))));
            return roundTrip is null
                ? raw > long.MaxValue - bound // only within the error bound of the largest value
                : Math.Abs(roundTrip.Value - raw) <= bound;
        }));

    /// <summary>
    /// Stated error: log(exp y) is within 2 + 2^32 / exp(y)_raw raw units of y
    /// (exp's half-unit rounding, divided by the size of exp y). Checked for y from -10 to 21.
    /// </summary>
    [Fact]
    public void Log_of_exp_returns_within_the_stated_error() =>
        NumericGen.Check(Prop.ForAll(
            Arb.From(ExpInput.Where(r => r >= -10 * One && r <= 21 * One)),
            raw =>
            {
                var e = FixedMath.Exp(Q32.FromRaw(raw)).Raw;
                return Math.Abs(FixedMath.Log(Q32.FromRaw(e)).Raw - raw) <= 2 + (One / e);
            }));

    /// <summary>Stated error: sqrt(x)^2 is within sqrt(x) + 2 raw units of x.</summary>
    [Fact]
    public void Sqrt_squared_returns_within_the_stated_error() =>
        NumericGen.Check(Prop.ForAll(Arb.From(NumericGen.NonNegativeRaw()), raw =>
        {
            var s = FixedMath.Sqrt(Q32.FromRaw(raw));
            var square = NumericGen.RawOrOverflow(() => s * s);
            return square is null
                ? raw > long.MaxValue - (1L << 33) // only the very top of the range can round past the maximum
                : Math.Abs(square.Value - raw) <= (s.Raw >> 32) + 2;
        }));

    /// <summary>Inputs for exp: whole parts from -24 to 21 with any fraction, plus small values.</summary>
    private static Gen<long> ExpInput =>
        Gen.OneOf(
            from whole in Gen.Choose(-24, 21)
            from fraction in Gen.Choose(int.MinValue, int.MaxValue)
            select ((long)whole << 32) + unchecked((uint)fraction),
            NumericGen.Raw(34),
            Gen.Elements(0L, 1L, -1L, long.MinValue, long.MaxValue));
}
