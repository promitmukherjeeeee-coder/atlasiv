using System;
using Atlasiv.Core.Time;
using Xunit;

namespace Atlasiv.Core.Tests.Time;

/// <summary>Calendar dates: astronomical year, month and day, always a real day.</summary>
public sealed class CalendarDateTests
{
    [Theory]
    [InlineData(2_000L, 2, 29)]
    [InlineData(1_600L, 2, 29)]
    [InlineData(0L, 2, 29)]       // 1 BCE is a leap year
    [InlineData(2_023L, 12, 31)]
    public void Real_days_are_accepted(long year, int month, int day)
    {
        var date = new CalendarDate(year, month, day);

        Assert.Equal(year, date.Year);
        Assert.Equal(month, date.Month);
        Assert.Equal(day, date.Day);
    }

    [Theory]
    [InlineData(1_900L, 2, 29)]
    [InlineData(1_700L, 2, 29)]
    [InlineData(-1L, 2, 29)]      // 2 BCE is not a leap year
    [InlineData(2_023L, 4, 31)]
    [InlineData(2_023L, 13, 1)]
    [InlineData(2_023L, 0, 1)]
    [InlineData(2_023L, 1, 0)]
    public void Impossible_days_are_refused(long year, int month, int day)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CalendarDate(year, month, day));
    }

    [Fact]
    public void The_default_date_is_1_January_1_BCE()
    {
        Assert.Equal(new CalendarDate(0, 1, 1), default);
        Assert.Equal("1 January 1 BCE", default(CalendarDate).ToString());
    }

    [Theory]
    [InlineData(-9_999L, 1, 1, "1 January 10,000 BCE")]
    [InlineData(-3_000L, 7, 1, "1 July 3001 BCE")]
    [InlineData(1L, 3, 15, "15 March 1 CE")]
    [InlineData(2_000L, 2, 29, "29 February 2000 CE")]
    public void Dates_read_in_BCE_and_CE(long year, int month, int day, string expected)
    {
        Assert.Equal(expected, new CalendarDate(year, month, day).ToString());
    }

    [Fact]
    public void Display_years_build_dates()
    {
        Assert.Equal(new CalendarDate(-43, 3, 15), CalendarDate.FromDisplay(new DisplayYear(44, CalendarEra.Bce), 3, 15));
        Assert.Equal(new DisplayYear(44, CalendarEra.Bce), new CalendarDate(-43, 3, 15).DisplayYear);
    }

    [Fact]
    public void Dates_order_by_year_then_month_then_day()
    {
        Assert.True(new CalendarDate(-1, 12, 31) < new CalendarDate(0, 1, 1));
        Assert.True(new CalendarDate(2_000, 1, 31) < new CalendarDate(2_000, 2, 1));
        Assert.True(new CalendarDate(2_000, 2, 1) < new CalendarDate(2_000, 2, 2));
        Assert.Equal(0, new CalendarDate(5, 5, 5).CompareTo(new CalendarDate(5, 5, 5)));
    }
}
