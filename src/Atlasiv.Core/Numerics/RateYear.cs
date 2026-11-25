namespace Atlasiv.Core.Numerics;

/// <summary>The rate year: per-year rates apply to elapsed seconds divided by its length.</summary>
public static class RateYear
{
    /// <summary>
    /// Seconds in one rate year: 365.2425 days, the mean Gregorian year (Pass 1 spec section 3).
    /// A 365-day year counts as about 0.99934 rate years.
    /// </summary>
    public const long Seconds = 31_556_952;

    /// <summary>
    /// Rate time: <paramref name="elapsedSeconds"/> divided by <see cref="Seconds"/>, rounded
    /// half to even to the Q32 grid (2^-32 of a rate year, about 0.007 seconds). A leap day
    /// therefore counts in full, and the result does not depend on how the time is split
    /// into turns, apart from that rounding.
    /// </summary>
    /// <exception cref="SimulationDomainException">The elapsed time is negative.</exception>
    /// <exception cref="SimulationOverflowException">The result exceeds the Q32 range (about 2.1 billion rate years).</exception>
    public static Q32 FromSeconds(long elapsedSeconds)
    {
        if (elapsedSeconds < 0)
        {
            throw new SimulationDomainException(nameof(elapsedSeconds), elapsedSeconds, "An elapsed time cannot be negative.");
        }

        return Q32.FromRatio(elapsedSeconds, Seconds);
    }
}
