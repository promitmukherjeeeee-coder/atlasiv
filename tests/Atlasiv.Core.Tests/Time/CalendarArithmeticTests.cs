using System;
using Atlasiv.Core.Numerics;
using Atlasiv.Core.Time;
using FsCheck;
using FsCheck.Fluent;
using Xunit;

namespace Atlasiv.Core.Tests.Time;

/// <summary>
/// The internal day-counting helpers behind every date conversion. System.DateTime is an
/// independent proleptic Gregorian reference for 1 CE to 9999 CE; tests may use it, the
/// simulation never does.
/// </summary>
public sealed class CalendarArithmeticTests
{
    // 10,000 BCE to 1 CE is exactly 10,000 years: 25 Gregorian cycles of 146,097 days.
    private const long DaysFromEpochTo1Ce = 3_652_425;

    [Theory]
    [InlineData(-9_999L, 1, 1, 0L)]
    [InlineData(-9_999L, 1, 2, 1L)]
    [InlineData(-9_999L, 12, 31, 364L)]   // 10,000 BCE is not a leap year
    [InlineData(-9_998L, 1, 1, 365L)]
    [InlineData(0L, 1, 1, DaysFromEpochTo1Ce - 366)]   // 1 BCE is a leap year
    [InlineData(1L, 1, 1, DaysFromEpochTo1Ce)]
    [InlineData(-10_000L, 12, 31, -1L)]   // the day before the epoch
    public void Days_since_epoch_vectors(long year, int month, int day, long days)
    {
        var date = new CalendarDate(year, month, day);

        Assert.Equal(days, Calendar.DaysSinceEpoch(date));
        Assert.Equal(date, Calendar.DateFromDaysSinceEpoch(days));
    }

    [Fact]
    public void Days_since_epoch_match_DateTime_for_every_day_in_some_CE_spans()
    {
        foreach (var (from, to) in new[] { (1, 3), (1_598, 1_602), (1_698, 1_702), (1_898, 1_902), (1_998, 2_002), (9_997, 9_999) })
        {
            for (var date = new DateTime(from, 1, 1); date.Year <= to; date = date.AddDays(1))
            {
                var expected = DaysFromEpochTo1Ce + (date - DateTime.MinValue).Days;

                var calendarDate = new CalendarDate(date.Year, date.Month, date.Day);

                Assert.Equal(expected, Calendar.DaysSinceEpoch(calendarDate));
                Assert.Equal(calendarDate, Calendar.DateFromDaysSinceEpoch(expected));
                if (date.Year == 9_999 && date.Month == 12 && date.Day == 31)
                {
                    break;
                }
            }
        }
    }

    [Fact]
    public void Random_CE_dates_match_DateTime()
    {
        var gen =
            from year in Gen.Choose(1, 9_999)
            from month in Gen.Choose(1, 12)
            from day in Gen.Choose(1, DateTime.DaysInMonth(year, month))
            select new DateTime(year, month, day);

        TimeGen.Check(Prop.ForAll(Arb.From(gen), date =>
            Calendar.DaysSinceEpoch(new CalendarDate(date.Year, date.Month, date.Day))
                == DaysFromEpochTo1Ce + (date - DateTime.MinValue).Days));
    }

    [Fact]
    public void Day_counts_and_dates_round_trip_over_a_huge_range()
    {
        // CalendarDate refuses impossible days, so building one checks the date is real.
        TimeGen.Check(Prop.ForAll(Arb.From(TimeGen.Days()), days =>
            Calendar.DaysSinceEpoch(Calendar.DateFromDaysSinceEpoch(days)) == days));
    }

    [Fact]
    public void Consecutive_day_counts_are_consecutive_dates()
    {
        TimeGen.Check(Prop.ForAll(Arb.From(TimeGen.Days()), days =>
        {
            var date = Calendar.DateFromDaysSinceEpoch(days);
            var (year, month, day) = (date.Year, date.Month, date.Day);
            var next = Calendar.DateFromDaysSinceEpoch(days + 1);
            var expected = day < Calendar.DaysInMonth(year, month) ? new CalendarDate(year, month, day + 1)
                : month < 12 ? new CalendarDate(year, month + 1, 1)
                : new CalendarDate(year + 1, 1, 1);
            return next == expected;
        }));
    }

    [Theory]
    [InlineData(2_000L, 1, 0L, 2_000L, 1)]
    [InlineData(2_000L, 1, 11L, 2_000L, 12)]
    [InlineData(2_000L, 12, 1L, 2_001L, 1)]
    [InlineData(2_000L, 7, 600L, 2_050L, 7)]
    [InlineData(1L, 1, -1L, 0L, 12)]          // the month before January 1 CE is December 1 BCE
    [InlineData(-9_999L, 1, -1L, -10_000L, 12)]
    [InlineData(-9_999L, 11, 30L, -9_996L, 5)]
    public void Adding_months_crosses_year_ends(long year, int month, long add, long expectedYear, int expectedMonth)
    {
        Assert.Equal((expectedYear, expectedMonth), Calendar.AddMonths(year, month, add));
    }

    [Fact]
    public void Absurd_years_overflow_instead_of_wrapping()
    {
        Assert.Throws<SimulationOverflowException>(() => Calendar.DaysSinceEpoch(new CalendarDate(long.MaxValue / 2, 1, 1)));
        Assert.Throws<SimulationOverflowException>(() => Calendar.DaysSinceEpoch(new CalendarDate(long.MinValue, 1, 1)));
        Assert.Throws<SimulationOverflowException>(() => Calendar.AddMonths(long.MaxValue, 12, 1));
        Assert.Throws<SimulationOverflowException>(() => Calendar.AddMonths(long.MinValue, 1, -1));
    }

    [Theory]
    [InlineData(long.MinValue)]
    [InlineData(long.MaxValue)]
    public void Every_64_bit_day_count_has_a_date(long days)
    {
        Assert.Equal(days, Calendar.DaysSinceEpoch(Calendar.DateFromDaysSinceEpoch(days)));
    }
}
