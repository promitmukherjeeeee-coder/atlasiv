using System.Numerics;

namespace Atlasiv.Core.Tests.Numerics;

/// <summary>
/// Slow, high-precision reference versions of sqrt, exp and log, used only by tests.
/// They work in BigInteger fixed point with <see cref="Bits"/> fractional bits, by a
/// different method from the library, and return the true value rounded half to even
/// to a Q32 raw value. Their own error (about 2^-150) is far below one Q32 unit.
/// </summary>
internal static class ReferenceMath
{
    private const int Bits = 160;
    private static readonly BigInteger One = BigInteger.One << Bits;
    private static readonly BigInteger Ln2 = ComputeLn2();

    /// <summary>sqrt(raw / 2^32), correctly rounded, as a Q32 raw value.</summary>
    public static BigInteger Sqrt(long raw)
    {
        var n = (BigInteger)raw << 32;
        var s = IntegerSqrt(n);
        return n - (s * s) > s ? s + 1 : s; // n > (s + 1/2)^2 exactly when n - s^2 > s
    }

    /// <summary>exp(raw / 2^32), correctly rounded, as a Q32 raw value (may exceed 64 bits).</summary>
    public static BigInteger Exp(long raw) => Exp(raw, 32);

    /// <summary>exp(raw / 2^32), correctly rounded to <paramref name="outputBits"/> fractional bits.</summary>
    public static BigInteger Exp(long raw, int outputBits)
    {
        var x = (BigInteger)raw << (Bits - 32);
        var k = NumericGen.DivideHalfToEven(x, Ln2);    // x = k ln2 + r, |r| <= ln2 / 2
        var r = x - (k * Ln2);
        var sum = One;
        var term = One;
        for (var n = 1; !term.IsZero; n++)
        {
            term = term * r / One / n;
            sum += term;
        }

        // value = sum * 2^k / 2^Bits; with outputBits fractional bits that is sum * 2^(k + outputBits - Bits).
        var shift = (int)k + outputBits - Bits;
        return shift >= 0 ? sum << shift : NumericGen.DivideHalfToEven(sum, BigInteger.One << -shift);
    }

    /// <summary>ln(raw / 2^32), correctly rounded, as a Q32 raw value. Requires raw &gt; 0.</summary>
    public static BigInteger Log(long raw)
    {
        // raw = m * 2^e with m in [1, 2); ln(raw / 2^32) = ln m + (e - 32) ln 2.
        var e = (int)BigInteger.Log2(raw);
        var m = ((BigInteger)raw << Bits) >> e;
        var lnM = 2 * Atanh(((m - One) * One) / (m + One));
        var total = lnM + ((e - 32) * Ln2);
        return NumericGen.DivideHalfToEven(total, BigInteger.One << (Bits - 32));
    }

    /// <summary>1 - exp(-h) for a Q32 hazard raw value h &gt;= 0, correctly rounded.</summary>
    public static BigInteger OneMinusExpNeg(long hazardRaw) => (BigInteger.One << 32) - Exp(-hazardRaw);

    private static BigInteger Atanh(BigInteger s)
    {
        // atanh(s) = s + s^3/3 + s^5/5 + ...
        var s2 = s * s / One;
        var power = s;
        var sum = BigInteger.Zero;
        for (var n = 1; !power.IsZero; n += 2)
        {
            sum += power / n;
            power = power * s2 / One;
        }

        return sum;
    }

    private static BigInteger ComputeLn2()
    {
        // ln 2 = sum over k >= 1 of 1 / (k 2^k), with extra guard bits.
        var guard = One << 16;
        var sum = BigInteger.Zero;
        for (var k = 1; k < Bits + 40; k++)
        {
            sum += guard / (k * (BigInteger.One << k));
        }

        return sum >> 16;
    }

    private static BigInteger IntegerSqrt(BigInteger n)
    {
        if (n.IsZero)
        {
            return n;
        }

        var x = BigInteger.One << (int)((BigInteger.Log2(n) / 2) + 1);
        while (true)
        {
            var y = (x + (n / x)) / 2;
            if (y >= x)
            {
                return x;
            }

            x = y;
        }
    }
}
