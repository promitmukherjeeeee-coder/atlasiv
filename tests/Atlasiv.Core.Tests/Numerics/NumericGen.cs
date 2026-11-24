using System;
using System.Numerics;
using Atlasiv.Core.Numerics;
using FsCheck;
using FsCheck.Fluent;

namespace Atlasiv.Core.Tests.Numerics;

/// <summary>
/// Random inputs for the numerics properties. Magnitudes are spread on a log scale,
/// so tiny, ordinary and huge raw values are all drawn often.
/// </summary>
internal static class NumericGen
{
    /// <summary>Any 64-bit value whose magnitude is below 2^<paramref name="maxBits"/>, log-spread.</summary>
    public static Gen<long> Raw(int maxBits = 63) =>
        from hi in Gen.Choose(int.MinValue, int.MaxValue)
        from lo in Gen.Choose(int.MinValue, int.MaxValue)
        from shift in Gen.Choose(63 - maxBits, 63)
        select ((long)hi << 32 | unchecked((uint)lo)) >> shift; // reinterpret the bits on purpose

    /// <summary>A non-negative raw value below 2^<paramref name="maxBits"/>, log-spread.</summary>
    public static Gen<long> NonNegativeRaw(int maxBits = 63) =>
        Raw(maxBits + 1).Select(r => r < 0 ? ~r : r).Select(r => maxBits >= 63 ? r : r & ((1L << maxBits) - 1));

    /// <summary>A Q32 whose magnitude is below 2^(<paramref name="maxBits"/> - 32).</summary>
    public static Gen<Q32> Q32(int maxBits = 63) => Raw(maxBits).Select(Atlasiv.Core.Numerics.Q32.FromRaw);

    public static Arbitrary<Q32> AnyQ32 => Arb.From(Q32());

    public static Arbitrary<(Q32 A, Q32 B)> Pair(int maxBitsA = 63, int maxBitsB = 63) =>
        Arb.From(from a in Q32(maxBitsA) from b in Q32(maxBitsB) select (a, b));

    /// <summary>Number of random cases each numerics property checks.</summary>
    public const int Cases = 2000;

    /// <summary>Checks <paramref name="property"/> on <see cref="Cases"/> random inputs; throws on the first failure.</summary>
    public static void Check(Property property) =>
        FsCheck.Check.One(Config.QuickThrowOnFailure.WithMaxTest(Cases), property);

    /// <summary>Runs <paramref name="operation"/> and returns its raw result, or null if it overflowed.</summary>
    public static long? RawOrOverflow(Func<Q32> operation)
    {
        try
        {
            return operation().Raw;
        }
        catch (SimulationOverflowException)
        {
            return null;
        }
    }

    /// <summary>Exact division rounded half to even, done with BigInteger as an independent reference.</summary>
    public static BigInteger DivideHalfToEven(BigInteger numerator, BigInteger divisor)
    {
        var quotient = BigInteger.DivRem(numerator, divisor, out var remainder);
        var twice = BigInteger.Abs(remainder) * 2;
        var size = BigInteger.Abs(divisor);
        var away = numerator.Sign * divisor.Sign;
        if (twice > size || (twice == size && !quotient.IsEven))
        {
            quotient += away;
        }

        return quotient;
    }

    /// <summary>The reference result as a raw value, or null if it is outside the 64-bit range.</summary>
    public static long? InRange(BigInteger value) =>
        value >= long.MinValue && value <= long.MaxValue ? (long)value : null;
}
