using System;
using System.Numerics;

namespace Atlasiv.Core.Numerics;

/// <summary>
/// Exponential, natural logarithm and square root for <see cref="Q32"/>, computed with
/// integer arithmetic only, so every machine gives the same bits (Pass 1 spec section 8).
/// Library math functions are never used: they can differ between platforms and runtimes.
/// </summary>
/// <remarks>
/// Accuracy, in raw units of 2^-32, checked against a 160-bit reference in the tests:
/// <list type="bullet">
/// <item><see cref="Sqrt"/> is correctly rounded (error at most 1/2).</item>
/// <item><see cref="Log"/> is within 1 of the correctly rounded value.</item>
/// <item><see cref="Exp"/> is within 1 of the correctly rounded value, plus a relative
/// 2^-56 for large results. For inputs at or below 0 (results up to 1) it is within 1.</item>
/// </list>
/// Internally exp works in Q2.62 and log in Q0.64, well beyond the 32 bits returned.
/// </remarks>
public static class FixedMath
{
    private const int ExpFractionBits = 62;
    private const int LogFractionBits = 64;

    // Taylor terms for exp on [0, ln 2): the first omitted term is below 2^-66.
    private const int ExpTerms = 18;

    // atanh series terms for s below 1/3: the first omitted term is below 2^-67.
    private const int LogTerms = 19;

    // Inputs at or above 22 overflow (exp 22 > 2^31); inputs below -24 round to 0 (exp -24 < 2^-34).
    private const long ExpOverflowRaw = 22L << Q32.FractionBits;
    private const long ExpZeroRaw = -24L << Q32.FractionBits;

    private static readonly UInt128 ExpOne = UInt128.One << ExpFractionBits;

    // ln 2 rounded half to even to 96 and 64 fractional bits:
    // 0.B17217F7 D1CF79AB C9E3B398 03F2F6AF... (hexadecimal).
    private static readonly Int128 Ln2Q96 = new(0xB172_17F7UL, 0xD1CF_79AB_C9E3_B398UL);
    private static readonly Int128 Ln2Q64 = 0xB172_17F7_D1CF_79ACUL;

    // 2^64 / (2n + 1), rounded half to even, for the atanh series.
    private static readonly UInt128[] OddReciprocals = BuildOddReciprocals();

    /// <summary>e raised to <paramref name="x"/>.</summary>
    /// <exception cref="SimulationOverflowException">The result is above <see cref="Q32.MaxValue"/> (x above about 21.4876).</exception>
    public static Q32 Exp(Q32 x)
    {
        if (x.Raw >= ExpOverflowRaw)
        {
            throw new SimulationOverflowException("FixedMath.Exp overflowed: the input " + x + " is above about 21.4876.");
        }

        if (x.Raw < ExpZeroRaw)
        {
            return Q32.Zero;
        }

        // x = k ln2 + r with 0 <= r < ln 2, worked out at 96 fractional bits, then r rounded to Q2.62.
        var x96 = (Int128)x.Raw << (96 - Q32.FractionBits);
        var k = FloorDivide(x96, Ln2Q96);
        var r96 = x96 - (k * Ln2Q96);
        var r = WideMath.ShiftRightHalfToEven((UInt128)r96, 96 - ExpFractionBits);

        // exp x = exp(r) * 2^k; exp(r) is in [1, 2) at 62 fractional bits.
        var expR = ExpQ62(r);
        var shift = ExpFractionBits - Q32.FractionBits - (int)k;
        // Shifts are never overflow-checked. The guard above keeps k at 31 or less, so a left
        // shift is by at most 1 bit (exp(r) < 2^63 becomes < 2^64) and ToInt64 catches the overflow.
        var raw = shift >= 0 ? WideMath.ShiftRightHalfToEven(expR, shift) : expR << -shift;
        return Q32.FromRaw(WideMath.ToInt64((Int128)raw, "FixedMath.Exp"));
    }

    /// <summary>The natural logarithm of <paramref name="x"/>.</summary>
    /// <exception cref="SimulationDomainException"><paramref name="x"/> is zero or negative.</exception>
    public static Q32 Log(Q32 x)
    {
        if (x.Raw <= 0)
        {
            throw new SimulationDomainException(nameof(x), x, "The logarithm needs a positive value.");
        }

        // x = m * 2^e with m in [1, 2), held at 62 fractional bits.
        var top = 63 - BitOperations.LeadingZeroCount((ulong)x.Raw);
        var m = (UInt128)(ulong)x.Raw << (ExpFractionBits - top);
        var e = top - Q32.FractionBits;

        // ln m = 2 atanh(s) with s = (m - 1) / (m + 1) in [0, 1/3), at 64 fractional bits.
        var s = WideMath.DivideHalfToEven((m - ExpOne) << LogFractionBits, m + ExpOne);
        var z = WideMath.ShiftRightHalfToEven(s * s, LogFractionBits);
        var sum = OddReciprocals[LogTerms];
        for (var n = LogTerms - 1; n >= 0; n--)
        {
            sum = OddReciprocals[n] + WideMath.ShiftRightHalfToEven(z * sum, LogFractionBits);
        }

        var lnM = WideMath.ShiftRightHalfToEven(s * sum, LogFractionBits - 1);
        var total = (Int128)lnM + (e * Ln2Q64);
        return Q32.FromRaw((long)WideMath.ShiftRightHalfToEven(total, LogFractionBits - Q32.FractionBits));
    }

    /// <summary>The square root of <paramref name="x"/>, correctly rounded.</summary>
    /// <exception cref="SimulationDomainException"><paramref name="x"/> is negative.</exception>
    public static Q32 Sqrt(Q32 x)
    {
        if (x.Raw < 0)
        {
            throw new SimulationDomainException(nameof(x), x, "The square root needs a non-negative value.");
        }

        // sqrt(raw / 2^32) * 2^32 = sqrt(raw * 2^32).
        var n = (UInt128)(ulong)x.Raw << Q32.FractionBits;
        var (root, remainder) = IntegerSqrt(n);

        // Round to nearest: n > (root + 1/2)^2 exactly when n - root^2 > root. Ties cannot occur.
        return Q32.FromRaw((long)(remainder > root ? root + 1 : root));
    }

    /// <summary>
    /// exp(-<paramref name="mu"/>) at 64 fractional bits (2^64 stands for 1), for a
    /// non-negative <paramref name="mu"/>. Absolute error about 2^-62, so the result keeps
    /// far more precision than a Q32 value when it is close to 0 or 1. Used by Poisson
    /// draws (ENG-009).
    /// </summary>
    internal static UInt128 ExpNegativeQ64(Q32 mu)
    {
        if (mu.Raw <= 0)
        {
            return mu.Raw == 0
                ? UInt128.One << 64
                : throw new SimulationDomainException(nameof(mu), mu, "ExpNegativeQ64 needs a non-negative input.");
        }

        // mu = k ln2 - r with k = ceil(mu / ln 2) and 0 <= r < ln 2, worked out at 96 fractional bits.
        var mu96 = (Int128)mu.Raw << (96 - Q32.FractionBits); // below 2^127, so the shift cannot wrap
        var k = -FloorDivide(-mu96, Ln2Q96);

        // exp(-mu) = exp(r) * 2^-k is below 2^(1 - k); from k = 67 on that is under half of 2^-64.
        if (k >= 67)
        {
            return UInt128.Zero;
        }

        var r96 = (k * Ln2Q96) - mu96;
        var expR = ExpQ62(WideMath.ShiftRightHalfToEven((UInt128)r96, 96 - ExpFractionBits));

        // exp(r) at 62 fractional bits times 2^-k, written at 64 fractional bits: shift by k - 2.
        var shift = (int)k - 2;
        return shift >= 0 ? WideMath.ShiftRightHalfToEven(expR, shift) : expR << -shift;
    }

    /// <summary>exp(r) for r in [0, ln 2), both at 62 fractional bits, by Horner's rule on the Taylor series.</summary>
    private static UInt128 ExpQ62(UInt128 r)
    {
        // exp r = 1 + r(1 + r/2(1 + r/3(...))): each step is t = 1 + (r / n) t.
        // r is below 2^62, so r / n is a fast 64-bit division; t stays below 2^63.
        var r64 = (ulong)r;
        var t = ExpOne;
        for (var n = ExpTerms; n >= 1; n--)
        {
            var quotient = r64 / (ulong)n;
            var remainder = r64 - (quotient * (ulong)n);
            var rOverN = WideMath.RoundsUpHalfToEven(remainder, (ulong)n, (quotient & 1) == 1) ? quotient + 1 : quotient;
            t = ExpOne + WideMath.ShiftRightHalfToEven((UInt128)rOverN * t, ExpFractionBits);
        }

        return t;
    }

    /// <summary>Floor of the exact quotient for a positive divisor.</summary>
    private static Int128 FloorDivide(Int128 numerator, Int128 divisor)
    {
        var quotient = numerator / divisor;
        return numerator < 0 && quotient * divisor != numerator ? quotient - 1 : quotient;
    }

    /// <summary>Floor square root and remainder, digit by digit.</summary>
    private static (UInt128 Root, UInt128 Remainder) IntegerSqrt(UInt128 n)
    {
        var root = UInt128.Zero;
        var bit = UInt128.One << 126;
        while (bit > n)
        {
            bit >>= 2;
        }

        while (bit != 0)
        {
            if (n >= root + bit)
            {
                n -= root + bit;
                root = (root >> 1) + bit;
            }
            else
            {
                root >>= 1;
            }

            bit >>= 2;
        }

        return (root, n);
    }

    private static UInt128[] BuildOddReciprocals()
    {
        var table = new UInt128[LogTerms + 1];
        for (var n = 0; n <= LogTerms; n++)
        {
            table[n] = WideMath.DivideHalfToEven(UInt128.One << LogFractionBits, (UInt128)(uint)((2 * n) + 1));
        }

        return table;
    }
}
