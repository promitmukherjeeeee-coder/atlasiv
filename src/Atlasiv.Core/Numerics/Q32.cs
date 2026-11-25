using System;
using System.Globalization;

namespace Atlasiv.Core.Numerics;

/// <summary>
/// A Q32.32 fixed-point number: a signed 64-bit raw value read as raw / 2^32.
/// The default format for rates, probabilities and other dimensionless values
/// (Pass 1 spec section 8, ADR-004). Counts, money and mass are plain 64-bit integers
/// in base units, not Q32.
/// </summary>
/// <remarks>
/// The range is -2^31 to 2^31 - 2^-32, in steps of 2^-32 (about 2.3e-10).
/// Every operation is exact or rounds half to even, and any result outside the range
/// raises <see cref="SimulationOverflowException"/>.
/// </remarks>
public readonly struct Q32 : IEquatable<Q32>, IComparable<Q32>
{
    /// <summary>Number of fractional bits.</summary>
    public const int FractionBits = 32;

    private const long OneRaw = 1L << FractionBits;

    private Q32(long raw) => Raw = raw;

    /// <summary>Zero.</summary>
    public static Q32 Zero => default;

    /// <summary>One.</summary>
    public static Q32 One => new(OneRaw);

    /// <summary>The smallest positive value, 2^-32.</summary>
    public static Q32 Epsilon => new(1);

    /// <summary>The largest value, 2^31 - 2^-32.</summary>
    public static Q32 MaxValue => new(long.MaxValue);

    /// <summary>The most negative value, -2^31.</summary>
    public static Q32 MinValue => new(long.MinValue);

    /// <summary>The raw 64-bit value: the number times 2^32.</summary>
    public long Raw { get; }

    /// <summary>Wraps a raw value (the number times 2^32) without conversion.</summary>
    public static Q32 FromRaw(long raw) => new(raw);

    /// <summary>The whole number <paramref name="value"/>.</summary>
    /// <exception cref="SimulationOverflowException">The value is outside -2^31 to 2^31 - 1.</exception>
    public static Q32 FromInt(long value) =>
        new(WideMath.ToInt64((Int128)value << FractionBits, "Q32.FromInt"));

    /// <summary>The ratio <paramref name="numerator"/> / <paramref name="denominator"/>, rounded half to even.</summary>
    /// <exception cref="DivideByZeroException">The denominator is zero.</exception>
    /// <exception cref="SimulationOverflowException">The ratio is outside the Q32 range.</exception>
    public static Q32 FromRatio(long numerator, long denominator) =>
        new(WideMath.ToInt64(
            WideMath.DivideHalfToEven((Int128)numerator << FractionBits, denominator),
            "Q32.FromRatio"));

    /// <summary>Exact sum.</summary>
    /// <exception cref="SimulationOverflowException">The sum is outside the Q32 range.</exception>
    public static Q32 operator +(Q32 left, Q32 right) =>
        new(WideMath.ToInt64((Int128)left.Raw + right.Raw, "Q32 addition"));

    /// <summary>Exact difference.</summary>
    /// <exception cref="SimulationOverflowException">The difference is outside the Q32 range.</exception>
    public static Q32 operator -(Q32 left, Q32 right) =>
        new(WideMath.ToInt64((Int128)left.Raw - right.Raw, "Q32 subtraction"));

    /// <summary>Exact negation.</summary>
    /// <exception cref="SimulationOverflowException">The value is <see cref="MinValue"/>.</exception>
    public static Q32 operator -(Q32 value) =>
        new(WideMath.ToInt64(-(Int128)value.Raw, "Q32 negation"));

    /// <summary>Product, rounded half to even, using a 128-bit intermediate.</summary>
    /// <exception cref="SimulationOverflowException">The product is outside the Q32 range.</exception>
    public static Q32 operator *(Q32 left, Q32 right) =>
        new(WideMath.ToInt64(
            WideMath.ShiftRightHalfToEven((Int128)left.Raw * right.Raw, FractionBits),
            "Q32 multiplication"));

    /// <summary>Quotient, rounded half to even, using a 128-bit intermediate.</summary>
    /// <exception cref="DivideByZeroException">The divisor is zero.</exception>
    /// <exception cref="SimulationOverflowException">The quotient is outside the Q32 range.</exception>
    public static Q32 operator /(Q32 left, Q32 right) =>
        new(WideMath.ToInt64(
            WideMath.DivideHalfToEven((Int128)left.Raw << FractionBits, right.Raw),
            "Q32 division"));

    /// <summary>Named alternative to <c>+</c>.</summary>
    public static Q32 Add(Q32 left, Q32 right) => left + right;

    /// <summary>Named alternative to binary <c>-</c>.</summary>
    public static Q32 Subtract(Q32 left, Q32 right) => left - right;

    /// <summary>Named alternative to unary <c>-</c>.</summary>
    public static Q32 Negate(Q32 value) => -value;

    /// <summary>Named alternative to <c>*</c>.</summary>
    public static Q32 Multiply(Q32 left, Q32 right) => left * right;

    /// <summary>Named alternative to <c>/</c>.</summary>
    public static Q32 Divide(Q32 left, Q32 right) => left / right;

    /// <summary>The exact decimal value, using the invariant culture, with no trailing zeros.</summary>
    public override string ToString()
    {
        var magnitude = WideMath.Magnitude(Raw);
        var whole = magnitude >> FractionBits;
        var fraction = magnitude & (OneRaw - 1);
        var sign = Raw < 0 ? "-" : string.Empty;
        var wholeText = whole.ToString(CultureInfo.InvariantCulture);
        if (fraction == 0)
        {
            return sign + wholeText;
        }

        // fraction / 2^32 = fraction * 5^32 / 10^32, so 32 decimal digits are always exact.
        var digits = (fraction * Pow5To32).ToString("D32", CultureInfo.InvariantCulture).TrimEnd('0');
        return sign + wholeText + "." + digits;
    }

    /// <inheritdoc/>
    public bool Equals(Q32 other) => Raw == other.Raw;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Q32 other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => Raw.GetHashCode();

    /// <inheritdoc/>
    public int CompareTo(Q32 other) => Raw.CompareTo(other.Raw);

    /// <summary>True when both hold the same value.</summary>
    public static bool operator ==(Q32 left, Q32 right) => left.Raw == right.Raw;

    /// <summary>True when the values differ.</summary>
    public static bool operator !=(Q32 left, Q32 right) => left.Raw != right.Raw;

    /// <summary>Less than.</summary>
    public static bool operator <(Q32 left, Q32 right) => left.Raw < right.Raw;

    /// <summary>Greater than.</summary>
    public static bool operator >(Q32 left, Q32 right) => left.Raw > right.Raw;

    /// <summary>Less than or equal.</summary>
    public static bool operator <=(Q32 left, Q32 right) => left.Raw <= right.Raw;

    /// <summary>Greater than or equal.</summary>
    public static bool operator >=(Q32 left, Q32 right) => left.Raw >= right.Raw;

    // 5^32 = 5^16 * 5^16 = 23283064365386962890625.
    private static readonly UInt128 Pow5To32 = (UInt128)152_587_890_625UL * 152_587_890_625UL;
}
