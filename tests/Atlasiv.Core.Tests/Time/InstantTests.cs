using System;
using Atlasiv.Core.Numerics;
using Atlasiv.Core.Time;
using FsCheck;
using FsCheck.Fluent;
using Xunit;

namespace Atlasiv.Core.Tests.Time;

/// <summary>
/// The authoritative clock reading: signed 64-bit seconds from the epoch, 1 January
/// 10,000 BCE 00:00, proleptic Gregorian (Pass 1 spec section 2).
/// </summary>
public sealed class InstantTests
{
    private const long Day = 86_400;

    [Fact]
    public void The_epoch_is_1_January_10_000_BCE_at_midnight()
    {
        var epoch = Instant.Epoch;

        Assert.Equal(0, epoch.SecondsSinceEpoch);
        Assert.Equal(default, epoch);
        Assert.Equal(new CalendarDate(-9_999, 1, 1), epoch.Date);
        Assert.Equal(CalendarDate.FromDisplay(new DisplayYear(10_000, CalendarEra.Bce), 1, 1), epoch.Date);
        Assert.Equal(0, epoch.SecondOfDay);
        Assert.Equal(epoch, Instant.FromDate(new CalendarDate(-9_999, 1, 1)));
        Assert.Equal("1 January 10,000 BCE, 00:00:00", epoch.ToString());
    }

    [Theory]
    [InlineData(-9_999L, 1, 2, Day)]
    [InlineData(-9_998L, 1, 1, 365 * Day)]
    [InlineData(1L, 1, 1, 315_569_520_000L)]     // exactly 10,000 rate years: 25 whole Gregorian cycles
    [InlineData(2_000L, 1, 1, 378_651_801_600L)] // 3,652,425 + 730,119 days
    [InlineData(-10_000L, 12, 31, -Day)]          // the day before the epoch
    public void Epoch_conversion_vectors(long year, int month, int day, long seconds)
    {
        var date = new CalendarDate(year, month, day);

        Assert.Equal(seconds, Instant.FromDate(date).SecondsSinceEpoch);
        Assert.Equal(date, new Instant(seconds).Date);
    }

    [Fact]
    public void Ten_thousand_years_are_ten_thousand_rate_years()
    {
        Assert.Equal(10_000 * RateYear.Seconds, Instant.FromDate(new CalendarDate(1, 1, 1)).SecondsSinceEpoch);
    }

    [Fact]
    public void A_12_450_year_run_needs_about_3_9_times_10_to_the_11_seconds()
    {
        var end = Instant.FromDate(new CalendarDate(2_451, 1, 1));

        Assert.InRange(end.SecondsSinceEpoch, 390_000_000_000L, 400_000_000_000L);
    }

    [Fact]
    public void Time_of_day_is_split_out()
    {
        var instant = Instant.FromDate(new CalendarDate(1_969, 7, 20), 20, 17, 40);

        Assert.Equal(new CalendarDate(1_969, 7, 20), instant.Date);
        Assert.Equal((20 * 3_600) + (17 * 60) + 40, instant.SecondOfDay);
        Assert.Equal(20, instant.Hour);
        Assert.Equal(17, instant.Minute);
        Assert.Equal(40, instant.Second);
        Assert.Equal("20 July 1969 CE, 20:17:40", instant.ToString());
    }

    [Fact]
    public void Instants_before_the_epoch_count_back_from_midnight()
    {
        var instant = new Instant(-1);

        Assert.Equal(new CalendarDate(-10_000, 12, 31), instant.Date);
        Assert.Equal(23, instant.Hour);
        Assert.Equal(59, instant.Minute);
        Assert.Equal(59, instant.Second);
        Assert.Equal("31 December 10,001 BCE, 23:59:59", instant.ToString());
    }

    [Theory]
    [InlineData(24, 0, 0)]
    [InlineData(-1, 0, 0)]
    [InlineData(0, 60, 0)]
    [InlineData(0, 0, 60)]
    [InlineData(0, 0, -1)]
    public void Times_of_day_outside_the_day_are_refused(int hour, int minute, int second)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Instant.FromDate(new CalendarDate(2_000, 1, 1), hour, minute, second));
    }

    [Fact]
    public void Adding_and_measuring_seconds()
    {
        var start = Instant.FromDate(new CalendarDate(2_000, 2, 28));
        var later = start.AddSeconds(Day);

        Assert.Equal(new CalendarDate(2_000, 2, 29), later.Date);
        Assert.Equal(Day, later.SecondsSince(start));
        Assert.Equal(-Day, start.SecondsSince(later));
        Assert.True(start < later);
        Assert.True(later >= start);
        Assert.Equal(-1, start.CompareTo(later));
    }

    [Fact]
    public void The_clock_overflows_instead_of_wrapping()
    {
        Assert.Throws<SimulationOverflowException>(() => new Instant(long.MaxValue).AddSeconds(1));
        Assert.Throws<SimulationOverflowException>(() => new Instant(long.MinValue).AddSeconds(-1));
        Assert.Throws<SimulationOverflowException>(() => new Instant(long.MinValue).SecondsSince(new Instant(1)));
        Assert.Throws<SimulationOverflowException>(() => Instant.FromDate(new CalendarDate(400_000_000_000L, 1, 1)));
    }

    [Fact]
    public void Every_instant_survives_a_round_trip_through_its_date_and_time()
    {
        TimeGen.Check(Prop.ForAll(Arb.From(TimeGen.Seconds()), seconds =>
        {
            var instant = new Instant(seconds);
            var rebuilt = Instant.FromDate(instant.Date, instant.Hour, instant.Minute, instant.Second);
            return rebuilt == instant && instant.SecondOfDay is >= 0 and < (int)Day;
        }));
    }

    [Fact]
    public void Every_date_survives_a_round_trip_through_its_instant()
    {
        TimeGen.Check(Prop.ForAll(Arb.From(TimeGen.Days()), days =>
        {
            var date = new Instant(days * Day).Date;
            return Instant.FromDate(date).Date == date
                && Instant.FromDate(date).SecondsSinceEpoch == days * Day;
        }));
    }

    [Fact]
    public void Every_date_survives_a_round_trip_through_its_display_year()
    {
        TimeGen.Check(Prop.ForAll(Arb.From(TimeGen.Days()), days =>
        {
            var date = new Instant(days * Day).Date;
            var display = date.DisplayYear;
            return display.Number >= 1
                && CalendarDate.FromDisplay(display, date.Month, date.Day) == date;
        }));
    }
}
