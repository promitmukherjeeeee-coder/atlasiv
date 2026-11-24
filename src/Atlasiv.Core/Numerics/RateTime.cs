namespace Atlasiv.Core.Numerics;

/// <summary>Rate time: per-year rates apply to elapsed seconds divided by this year length.</summary>
public static class RateTime
{
    /// <summary>
    /// Seconds in one rate year: 365.2425 days, the mean Gregorian year (ADR-039 ruling 10).
    /// A 365-day year counts as about 0.99934 rate years.
    /// </summary>
    public const long SecondsPerYear = 31_556_952;
}
