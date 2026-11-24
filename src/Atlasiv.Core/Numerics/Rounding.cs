using System;

namespace Atlasiv.Core.Numerics;

/// <summary>
/// Conversion to whole base units (people, money minor units, mass units).
/// Round-half-to-even is the default; stochastic rounding is used only where a rule
/// says so (Pass 1 spec section 8). All products use 128-bit intermediates and any
/// result outside the 64-bit range raises <see cref="SimulationOverflowException"/>.
/// </summary>
public static class Rounding
{
    /// <summary>The nearest whole number; an exact half goes to the even neighbour.</summary>
    public static long HalfToEven(Q32 value) =>
        (long)WideMath.ShiftRightHalfToEven((Int128)value.Raw, Q32.FractionBits);

    /// <summary><paramref name="count"/> times <paramref name="factor"/>, rounded half to even.</summary>
    /// <exception cref="SimulationOverflowException">The result is outside the 64-bit range.</exception>
    public static long MultiplyHalfToEven(long count, Q32 factor) =>
        WideMath.ToInt64(
            WideMath.ShiftRightHalfToEven((Int128)count * factor.Raw, Q32.FractionBits),
            "Rounding.MultiplyHalfToEven");

    /// <summary><paramref name="a"/> times <paramref name="b"/> divided by <paramref name="divisor"/>, rounded half to even.</summary>
    /// <exception cref="DivideByZeroException">The divisor is zero.</exception>
    /// <exception cref="SimulationOverflowException">The result is outside the 64-bit range.</exception>
    public static long MultiplyDivideHalfToEven(long a, long b, long divisor) =>
        WideMath.ToInt64(
            WideMath.DivideHalfToEven((Int128)a * b, divisor),
            "Rounding.MultiplyDivideHalfToEven");

    /// <summary>
    /// Stochastic rounding hook. Rounds down or up to a neighbouring whole number, going up
    /// with probability equal to the fractional part, so the expected result equals the value.
    /// </summary>
    /// <param name="value">The value to round.</param>
    /// <param name="uniform">
    /// A uniform 64-bit draw from a seeded random stream (supplied by J2). Only its top
    /// 32 bits are used: the value rounds up exactly when they are below the fraction
    /// in units of 2^-32, which happens with probability fraction exactly.
    /// </param>
    public static long Stochastic(Q32 value, ulong uniform) =>
        (long)RoundStochastic(value.Raw, uniform); // at most 2^31, always in range

    /// <summary>
    /// <paramref name="count"/> times <paramref name="factor"/>, stochastically rounded on the
    /// exact product, for example turning a hazard into whole people. See <see cref="Stochastic"/>.
    /// </summary>
    /// <exception cref="SimulationOverflowException">The result is outside the 64-bit range.</exception>
    public static long MultiplyStochastic(long count, Q32 factor, ulong uniform) =>
        WideMath.ToInt64(RoundStochastic((Int128)count * factor.Raw, uniform), "Rounding.MultiplyStochastic");

    private static Int128 RoundStochastic(Int128 raw, ulong uniform)
    {
        var floor = raw >> Q32.FractionBits;
        var fraction = (ulong)(raw - (floor << Q32.FractionBits)); // 0 to 2^32 - 1
        return (uniform >> Q32.FractionBits) < fraction ? floor + 1 : floor;
    }
}
