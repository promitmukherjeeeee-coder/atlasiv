using System;
using Atlasiv.Core.Time;
using Xunit;

namespace Atlasiv.Core.Tests.Time;

/// <summary>
/// Astronomical years inside, BCE/CE years on display, with no year zero
/// (Pass 1 spec section 2): 1 BCE is year 0, 10,000 BCE is year -9,999.
/// </summary>
public sealed class DisplayYearTests
{
    [Theory]
    [InlineData(-9_999L, 10_000L, CalendarEra.Bce)]
    [InlineData(-1L, 2L, CalendarEra.Bce)]
    [InlineData(0L, 1L, CalendarEra.Bce)]
    [InlineData(1L, 1L, CalendarEra.Ce)]
    [InlineData(2_000L, 2_000L, CalendarEra.Ce)]
    public void Astronomical_and_display_years_convert_both_ways(long astronomical, long number, CalendarEra era)
    {
        var display = DisplayYear.FromAstronomical(astronomical);

        Assert.Equal(number, display.Number);
        Assert.Equal(era, display.Era);
        Assert.Equal(astronomical, new DisplayYear(number, era).Astronomical);
    }

    [Fact]
    public void There_is_no_year_zero_on_display()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new DisplayYear(0, CalendarEra.Bce));
        Assert.Throws<ArgumentOutOfRangeException>(() => new DisplayYear(0, CalendarEra.Ce));
        Assert.Throws<ArgumentOutOfRangeException>(() => new DisplayYear(-5, CalendarEra.Ce));
    }

    [Fact]
    public void The_year_before_1_CE_is_1_BCE()
    {
        var oneCe = new DisplayYear(1, CalendarEra.Ce).Astronomical;

        Assert.Equal(new DisplayYear(1, CalendarEra.Bce), DisplayYear.FromAstronomical(oneCe - 1));
    }

    [Fact]
    public void The_default_display_year_is_1_BCE()
    {
        Assert.Equal(0, default(DisplayYear).Astronomical);
        Assert.Equal(1, default(DisplayYear).Number);
    }

    [Theory]
    [InlineData(-9_999L, "10,000 BCE")]
    [InlineData(-999L, "1000 BCE")]
    [InlineData(0L, "1 BCE")]
    [InlineData(1L, "1 CE")]
    [InlineData(1_066L, "1066 CE")]
    [InlineData(12_345L, "12,345 CE")]
    public void Display_years_read_as_BCE_or_CE(long astronomical, string expected)
    {
        Assert.Equal(expected, DisplayYear.FromAstronomical(astronomical).ToString());
    }
}
