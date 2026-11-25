using System;
using Atlasiv.Core.Time;
using Xunit;

namespace Atlasiv.Core.Tests.Time;

/// <summary>
/// The Gregorian leap rule over the whole timeline (Pass 1 spec section 2): divisible by 4,
/// except centuries not divisible by 400. Years are astronomical, so 1 BCE is year 0.
/// </summary>
public sealed class LeapYearTests
{
    [Theory]
    [InlineData(1600L, true)]
    [InlineData(1700L, false)]
    [InlineData(1900L, false)]
    [InlineData(2000L, true)]
    [InlineData(2024L, true)]
    [InlineData(2023L, false)]
    [InlineData(0L, true)]         // 1 BCE
    [InlineData(-4L, true)]        // 5 BCE
    [InlineData(-1L, false)]       // 2 BCE
    [InlineData(-100L, false)]     // 101 BCE: a century not divisible by 400
    [InlineData(-400L, true)]      // 401 BCE
    [InlineData(-9_999L, false)]   // 10,000 BCE, the epoch year
    [InlineData(-9_996L, true)]    // 9,997 BCE
    [InlineData(-9_600L, true)]    // 9,601 BCE
    public void Leap_years_follow_the_Gregorian_rule(long year, bool leap)
    {
        Assert.Equal(leap, Calendar.IsLeapYear(year));
        Assert.Equal(leap ? 366 : 365, Calendar.DaysInYear(year));
    }

    [Theory]
    [InlineData(2000L, 2, 29)]
    [InlineData(1900L, 2, 28)]
    [InlineData(1600L, 2, 29)]
    [InlineData(1700L, 2, 28)]
    [InlineData(2023L, 1, 31)]
    [InlineData(2023L, 4, 30)]
    [InlineData(2023L, 6, 30)]
    [InlineData(2023L, 7, 31)]
    [InlineData(2023L, 9, 30)]
    [InlineData(2023L, 11, 30)]
    [InlineData(2023L, 12, 31)]
    [InlineData(-9_999L, 2, 28)]
    public void Months_have_their_Gregorian_lengths(long year, int month, int days)
    {
        Assert.Equal(days, Calendar.DaysInMonth(year, month));
    }

    [Fact]
    public void Months_outside_1_to_12_are_refused()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Calendar.DaysInMonth(2000, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => Calendar.DaysInMonth(2000, 13));
    }

    [Fact]
    public void Every_400_year_cycle_has_146_097_days_and_97_leap_years()
    {
        foreach (var start in new[] { -9_999L, -400L, 0L, 1_601L })
        {
            long days = 0;
            var leaps = 0;
            for (var year = start; year < start + 400; year++)
            {
                days += Calendar.DaysInYear(year);
                leaps += Calendar.IsLeapYear(year) ? 1 : 0;
            }

            Assert.Equal(146_097L, days);
            Assert.Equal(97, leaps);
        }
    }
}
