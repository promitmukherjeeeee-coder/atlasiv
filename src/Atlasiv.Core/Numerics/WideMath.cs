using System;
using System.Globalization;

namespace Atlasiv.Core.Numerics;

/// <summary>
/// 128-bit integer helpers shared by the numerics types. Every result is exact or
/// rounded half to even; nothing wraps.
/// </summary>
internal static class WideMath
{
    /// <summary>Divides and rounds the exact quotient half to even.</summary>
    /// <exception cref="DivideByZeroException">The divisor is zero.</exception>
    public static Int128 DivideHalfToEven(Int128 numerator, Int128 divisor)
    {
        if (divisor == 0)
        {
            throw new DivideByZeroException();
        }

        var negative = (numerator < 0) != (divisor < 0);
        var magnitude = DivideHalfToEven(Magnitude(numerator), Magnitude(divisor));
        return negative ? Negate(magnitude) : ToSigned(magnitude);
    }

    /// <summary>Divides two non-negative values and rounds the exact quotient half to even.</summary>
    public static UInt128 DivideHalfToEven(UInt128 numerator, UInt128 divisor)
    {
        var quotient = numerator / divisor;
        var remainder = numerator % divisor;
        var rest = divisor - remainder;
        if (remainder > rest || (remainder == rest && !UInt128.IsEvenInteger(quotient)))
        {
            quotient++;
        }

        return quotient;
    }

    /// <summary>
    /// Divides by 2^<paramref name="shift"/> and rounds half to even. Works for negative
    /// values too, because the arithmetic shift floors and the remainder is then never negative.
    /// </summary>
    public static Int128 ShiftRightHalfToEven(Int128 value, int shift)
    {
        if (shift == 0)
        {
            return value;
        }

        var floor = value >> shift;
        var remainder = (UInt128)(value - (floor << shift));
        var half = UInt128.One << (shift - 1);
        if (remainder > half || (remainder == half && !Int128.IsEvenInteger(floor)))
        {
            floor++;
        }

        return floor;
    }

    /// <summary>The same as <see cref="ShiftRightHalfToEven(Int128, int)"/> for non-negative values.</summary>
    public static UInt128 ShiftRightHalfToEven(UInt128 value, int shift)
    {
        if (shift == 0)
        {
            return value;
        }

        if (shift >= 128)
        {
            return UInt128.Zero;
        }

        var floor = value >> shift;
        var remainder = value - (floor << shift);
        var half = UInt128.One << (shift - 1);
        if (remainder > half || (remainder == half && !UInt128.IsEvenInteger(floor)))
        {
            floor++;
        }

        return floor;
    }

    /// <summary>The absolute value as an unsigned number, exact even for <see cref="Int128.MinValue"/>.</summary>
    public static UInt128 Magnitude(Int128 value) =>
        value < 0 ? (UInt128)(-(value + 1)) + 1 : (UInt128)value;

    /// <summary>Converts to a 64-bit raw value, or raises a simulation overflow naming the operation.</summary>
    public static long ToInt64(Int128 value, string operation)
    {
        if (value < long.MinValue || value > long.MaxValue)
        {
            throw new SimulationOverflowException(string.Create(
                CultureInfo.InvariantCulture,
                $"{operation} overflowed the 64-bit range: the exact result was {value}."));
        }

        return (long)value;
    }

    private static Int128 ToSigned(UInt128 magnitude) => (Int128)magnitude;

    private static Int128 Negate(UInt128 magnitude) =>
        magnitude == (UInt128)Int128.MaxValue + 1 ? Int128.MinValue : -(Int128)magnitude;
}
