using System;
using System.Globalization;

namespace Atlasiv.Core.Time;

/// <summary>
/// A day on the proleptic Gregorian calendar: astronomical year, month 1 to 12 and day of
/// month. Every value is a real day; impossible dates such as 29 February 1900 are refused.
/// </summary>
/// <remarks>
/// Month and day are stored counting from 0, so the default value is 1 January of year 0
/// (1 BCE), a real day, rather than an impossible "day 0 of month 0".
/// </remarks>
public readonly record struct CalendarDate : IComparable<CalendarDate>
{
    private static readonly string[] MonthNames =
    [
        "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December",
    ];

    private readonly byte _monthIndex;
    private readonly byte _dayIndex;

    /// <summary>Creates a date.</summary>
    /// <param name="year">The astronomical year (1 BCE = 0, 10,000 BCE = -9,999).</param>
    /// <param name="month">The month, 1 to 12.</param>
    /// <param name="day">The day of the month, from 1 to the month's length in that year.</param>
    /// <exception cref="ArgumentOutOfRangeException">The month or day does not exist.</exception>
    public CalendarDate(long year, int month, int day)
    {
        var length = Calendar.DaysInMonth(year, month);
        ArgumentOutOfRangeException.ThrowIfLessThan(day, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(day, length);

        Year = year;
        _monthIndex = (byte)(month - 1);
        _dayIndex = (byte)(day - 1);
    }

    /// <summary>The astronomical year (1 BCE = 0, 10,000 BCE = -9,999).</summary>
    public long Year { get; }

    /// <summary>The month, 1 to 12.</summary>
    public int Month => _monthIndex + 1;

    /// <summary>The day of the month, from 1.</summary>
    public int Day => _dayIndex + 1;

    /// <summary>The year as players read it, BCE or CE.</summary>
    public DisplayYear DisplayYear => DisplayYear.FromAstronomical(Year);

    /// <summary>Creates a date from a displayed (BCE or CE) year.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The month or day does not exist.</exception>
    public static CalendarDate FromDisplay(DisplayYear year, int month, int day) =>
        new(year.Astronomical, month, day);

    /// <summary>Orders dates by time: year, then month, then day.</summary>
    public int CompareTo(CalendarDate other) =>
        Year != other.Year ? Year.CompareTo(other.Year)
        : _monthIndex != other._monthIndex ? _monthIndex.CompareTo(other._monthIndex)
        : _dayIndex.CompareTo(other._dayIndex);

    /// <summary>Whether <paramref name="left"/> is earlier than <paramref name="right"/>.</summary>
    public static bool operator <(CalendarDate left, CalendarDate right) => left.CompareTo(right) < 0;

    /// <summary>Whether <paramref name="left"/> is later than <paramref name="right"/>.</summary>
    public static bool operator >(CalendarDate left, CalendarDate right) => left.CompareTo(right) > 0;

    /// <summary>Whether <paramref name="left"/> is the same day as or earlier than <paramref name="right"/>.</summary>
    public static bool operator <=(CalendarDate left, CalendarDate right) => left.CompareTo(right) <= 0;

    /// <summary>Whether <paramref name="left"/> is the same day as or later than <paramref name="right"/>.</summary>
    public static bool operator >=(CalendarDate left, CalendarDate right) => left.CompareTo(right) >= 0;

    /// <summary>The date as text, such as "1 January 10,000 BCE" or "29 February 2000 CE".</summary>
    public override string ToString() =>
        Day.ToString(CultureInfo.InvariantCulture) + " " + MonthNames[_monthIndex] + " " + DisplayYear;
}
