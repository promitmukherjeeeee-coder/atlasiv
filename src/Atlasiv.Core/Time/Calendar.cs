using System;

namespace Atlasiv.Core.Time;

/// <summary>
/// The proleptic Gregorian calendar over the whole timeline (Pass 1 spec section 2).
/// Years are astronomical: 1 BCE is year 0, 10,000 BCE is year -9,999.
/// </summary>
public static class Calendar
{
    /// <summary>Days in one 400-year Gregorian cycle.</summary>
    internal const long DaysPer400Years = 146_097;

    /// <summary>Seconds in one calendar day.</summary>
    public const long SecondsPerDay = 86_400;

    /// <summary>
    /// Whether <paramref name="year"/> (astronomical) is a leap year: divisible by 4,
    /// except centuries not divisible by 400.
    /// </summary>
    public static bool IsLeapYear(long year) =>
        year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);

    /// <summary>365, or 366 in a leap year.</summary>
    public static int DaysInYear(long year) => IsLeapYear(year) ? 366 : 365;

    /// <summary>The number of days in month <paramref name="month"/> (1 to 12) of <paramref name="year"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The month is outside 1 to 12.</exception>
    public static int DaysInMonth(long year, int month)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(month, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(month, 12);
        return month switch
        {
            2 => IsLeapYear(year) ? 29 : 28,
            4 or 6 or 9 or 11 => 30,
            _ => 31,
        };
    }

    // Day-counting core: Howard Hinnant's days_from_civil and civil_from_days
    // ("chrono-Compatible Low-Level Date Algorithms"), which count days from 1 March of
    // astronomical year 0 in 400-year cycles, with years starting in March so the leap day
    // falls at the end. Integer only; checked arithmetic throws on overflow (ENG-010).

    /// <summary>Days from 1 March, year 0, to the epoch (1 January 10,000 BCE): negative.</summary>
    private const long EpochFromMarchYear0 = (-25 * DaysPer400Years) + 306;

    /// <summary>
    /// Whole days from the epoch to <paramref name="year"/>-<paramref name="month"/>-<paramref name="day"/>.
    /// The date must be valid; callers check it.
    /// </summary>
    /// <exception cref="OverflowException">The year is so far out that the day count leaves the 64-bit range.</exception>
    internal static long DaysSinceEpoch(long year, int month, int day)
    {
        var y = month <= 2 ? year - 1 : year;
        var era = FloorDivide(y, 400);
        var yearOfEra = y - (era * 400);                                  // 0 to 399
        var monthFromMarch = (month + 9) % 12;                            // March = 0
        var dayOfYear = (((153 * monthFromMarch) + 2) / 5) + day - 1;     // 0 to 365
        var dayOfEra = (yearOfEra * 365) + (yearOfEra / 4) - (yearOfEra / 100) + dayOfYear;
        return (era * DaysPer400Years) + dayOfEra - EpochFromMarchYear0;
    }

    /// <summary>The date (astronomical year, month 1 to 12, day 1 to 31) <paramref name="days"/> whole days after the epoch.</summary>
    /// <exception cref="OverflowException">The day count is so large that the date leaves the 64-bit range.</exception>
    internal static (long Year, int Month, int Day) DateFromDaysSinceEpoch(long days)
    {
        var z = days + EpochFromMarchYear0;
        var era = FloorDivide(z, DaysPer400Years);
        var dayOfEra = z - (era * DaysPer400Years);                       // 0 to 146,096
        var yearOfEra = (dayOfEra - (dayOfEra / 1_460) + (dayOfEra / 36_524) - (dayOfEra / 146_096)) / 365;
        var dayOfYear = dayOfEra - ((365 * yearOfEra) + (yearOfEra / 4) - (yearOfEra / 100));
        var monthFromMarch = ((5 * dayOfYear) + 2) / 153;
        var day = (int)(dayOfYear - (((153 * monthFromMarch) + 2) / 5) + 1);
        var month = (int)(monthFromMarch < 10 ? monthFromMarch + 3 : monthFromMarch - 9);
        var year = (yearOfEra + (era * 400)) + (month <= 2 ? 1 : 0);
        return (year, month, day);
    }

    /// <summary>The year and month <paramref name="months"/> calendar months after (or before) the given one.</summary>
    /// <exception cref="OverflowException">The result is so far out that the month count leaves the 64-bit range.</exception>
    internal static (long Year, int Month) AddMonths(long year, int month, long months)
    {
        var total = (year * 12) + (month - 1) + months;
        var resultYear = FloorDivide(total, 12);
        return (resultYear, (int)(total - (resultYear * 12)) + 1);
    }

    /// <summary>Division rounding towards minus infinity, for a positive divisor.</summary>
    internal static long FloorDivide(long dividend, long divisor)
    {
        var quotient = dividend / divisor;
        return dividend % divisor < 0 ? quotient - 1 : quotient;
    }
}
