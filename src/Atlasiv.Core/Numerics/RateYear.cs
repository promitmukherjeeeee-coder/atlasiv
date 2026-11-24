namespace Atlasiv.Core.Numerics;

/// <summary>The rate year: per-year rates apply to elapsed seconds divided by its length.</summary>
public static class RateYear
{
    /// <summary>
    /// Seconds in one rate year: 365.2425 days, the mean Gregorian year (ADR-039 ruling 10).
    /// A 365-day year counts as about 0.99934 rate years.
    /// </summary>
    public const long Seconds = 31_556_952;
}
