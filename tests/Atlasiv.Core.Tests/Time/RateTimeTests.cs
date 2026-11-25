using System;
using Atlasiv.Core.Numerics;
using Atlasiv.Core.Time;
using Xunit;

namespace Atlasiv.Core.Tests.Time;

/// <summary>
/// Rate time: per-year rates apply to exact elapsed seconds divided by the mean Gregorian
/// year of 31,556,952 seconds (Pass 1 spec section 3), as a Q32 rounded half to even.
/// Expected raw values were computed separately in Python with exact fractions.
/// </summary>
public sealed class RateTimeTests
{
    private const long Day = 86_400;

    [Theory]
    [InlineData(365 * Day, 4_292_115_685L)]       // 0.99934 rate years
    [InlineData(366 * Day, 4_303_874_906L)]       // 1.00207 rate years
    [InlineData(31_556_952L, 1L << 32)]           // exactly 1
    [InlineData(50 * 31_556_952L, 50L << 32)]     // exactly 50
    [InlineData(Day, 11_759_221L)]
    [InlineData(28 * Day, 329_258_190L)]
    [InlineData(1L, 136L)]
    [InlineData(0L, 0L)]
    public void Elapsed_seconds_convert_to_rate_years(long seconds, long expectedRaw)
    {
        Assert.Equal(expectedRaw, RateYear.FromSeconds(seconds).Raw);
    }

    [Fact]
    public void Common_and_leap_years_read_to_five_decimals_as_the_spec_states()
    {
        Assert.Equal(99_934, FiveDecimals(RateYear.FromSeconds(365 * Day)));
        Assert.Equal(100_207, FiveDecimals(RateYear.FromSeconds(366 * Day)));
    }

    [Fact]
    public void Negative_elapsed_time_is_an_impossible_input()
    {
        Assert.Throws<SimulationDomainException>(() => RateYear.FromSeconds(-1));
    }

    [Fact]
    public void An_interval_measures_its_exact_seconds_and_rate_years()
    {
        var start = Instant.FromDate(new CalendarDate(2_000, 1, 1));
        var end = Instant.FromDate(new CalendarDate(2_001, 1, 1));
        var leapYear = new TimeInterval(start, end);

        Assert.Equal(366 * Day, leapYear.Seconds);
        Assert.Equal(4_303_874_906L, leapYear.RateYears.Raw);
    }

    [Fact]
    public void Intervals_are_half_open()
    {
        var start = new Instant(100);
        var interval = new TimeInterval(start, new Instant(200));

        Assert.True(interval.Contains(new Instant(100)));   // its start belongs to it
        Assert.True(interval.Contains(new Instant(199)));
        Assert.False(interval.Contains(new Instant(200)));  // its end does not
        Assert.False(interval.Contains(new Instant(99)));
        Assert.False(new TimeInterval(start, start).Contains(start)); // an empty interval holds nothing
    }

    [Fact]
    public void An_interval_cannot_end_before_it_starts()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new TimeInterval(new Instant(5), new Instant(4)));
    }

    // The value to five decimal places, rounded half to even, using exact integers.
    private static long FiveDecimals(Q32 value) =>
        Rounding.MultiplyDivideHalfToEven(value.Raw, 100_000, 1L << 32);
}
