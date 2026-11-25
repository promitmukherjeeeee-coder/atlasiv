using System;
using System.Globalization;

namespace Atlasiv.Core.Numerics;

/// <summary>
/// 128-bit integer helpers shared by the numerics types. Every result is exact or
/// rounded half to even; nothing wraps.
/// </summary>
internal static class WideMath
{
    /// <summary>
    /// The one half-to-even rule used everywhere: after a floor or truncating division
    /// left <paramref name="remainder"/> out of <paramref name="divisor"/>, should the
    /// quotient go up by one? Yes if more than half is left, or exactly half and the
    /// quotient is odd.
    /// </summary>
    public static bool RoundsUpHalfToEven(UInt128 remainder, UInt128 divisor, bool quotientIsOdd)
    {
        var rest = divisor - remainder;
        return remainder > rest || (remainder == rest && quotientIsOdd);
    }

    /// <summary>Divides and rounds the exact quotient half to even.</summary>
    /// <exception cref="DivideByZeroException">The divisor is zero.</exception>
    /// <exception cref="SimulationOverflowException">The quotient is 2^127 (only Int128.MinValue / -1).</exception>
    public static Int128 DivideHalfToEven(Int128 numerator, Int128 divisor)
    {
        if (divisor == 0)
        {
            throw new DivideByZeroException();
        }

        var negative = (numerator < 0) != (divisor < 0);
        var magnitude = DivideHalfToEven(Magnitude(numerator), Magnitude(divisor));
        if (negative && magnitude == (UInt128)Int128.MaxValue + 1)
        {
            return Int128.MinValue;
        }

        if (magnitude > (UInt128)Int128.MaxValue)
        {
            throw new SimulationOverflowException("WideMath.DivideHalfToEven overflowed the 128-bit range.");
        }

        return negative ? -(Int128)magnitude : (Int128)magnitude;
    }

    /// <summary>Divides two non-negative values and rounds the exact quotient half to even.</summary>
    public static UInt128 DivideHalfToEven(UInt128 numerator, UInt128 divisor)
    {
        var quotient = numerator / divisor;
        var remainder = numerator % divisor;
        return RoundsUpHalfToEven(remainder, divisor, UInt128.IsOddInteger(quotient)) ? quotient + 1 : quotient;
    }

    /// <summary>
    /// Divides by 2^<paramref name="shift"/> and rounds half to even. Works for negative
    /// values too, because the arithmetic shift floors and the remainder is then never negative.
    /// </summary>
    /// <param name="value">The value to divide.</param>
    /// <param name="shift">0 to 127. C# masks larger shift counts silently, so they are refused.</param>
    public static Int128 ShiftRightHalfToEven(Int128 value, int shift)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(shift);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(shift, 127);
        if (shift == 0)
        {
            return value;
        }

        var floor = value >> shift;
        var remainder = (UInt128)(value - (floor << shift));
        return RoundsUpHalfToEven(remainder, UInt128.One << shift, Int128.IsOddInteger(floor)) ? floor + 1 : floor;
    }

    /// <summary>The same as <see cref="ShiftRightHalfToEven(Int128, int)"/> for non-negative values.</summary>
    /// <param name="value">The value to divide.</param>
    /// <param name="shift">0 or more; 128 or more gives 0, since every bit is shifted out and less than half is left.</param>
    public static UInt128 ShiftRightHalfToEven(UInt128 value, int shift)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(shift);
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
        return RoundsUpHalfToEven(remainder, UInt128.One << shift, UInt128.IsOddInteger(floor)) ? floor + 1 : floor;
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
}
