using System;
using System.Globalization;
using Atlasiv.Core.Numerics;

namespace Atlasiv.Core.Time;

/// <summary>
/// A reading of the authoritative clock: whole seconds from the epoch, 1 January
/// 10,000 BCE 00:00 on the proleptic Gregorian calendar, as a signed 64-bit integer
/// (Pass 1 spec section 2). The clock is event-driven: nothing ticks every second, the
/// simulation simply jumps from one instant to the next.
/// </summary>
/// <remarks>The default value is the epoch.</remarks>
public readonly record struct Instant : IComparable<Instant>
{
    /// <summary>Creates the instant <paramref name="secondsSinceEpoch"/> seconds after (or, if negative, before) the epoch.</summary>
    public Instant(long secondsSinceEpoch)
    {
        SecondsSinceEpoch = secondsSinceEpoch;
    }

    /// <summary>1 January 10,000 BCE, 00:00: clock reading 0.</summary>
    public static Instant Epoch => default;

    /// <summary>Whole seconds from the epoch; negative before it.</summary>
    public long SecondsSinceEpoch { get; }

    /// <summary>The calendar day this instant falls on.</summary>
    public CalendarDate Date
    {
        get
        {
            var (year, month, day) = Calendar.DateFromDaysSinceEpoch(Calendar.FloorDivide(SecondsSinceEpoch, Calendar.SecondsPerDay));
            return new CalendarDate(year, month, day);
        }
    }

    /// <summary>Seconds since midnight, 0 to 86,399.</summary>
    public int SecondOfDay =>
        (int)(SecondsSinceEpoch - (Calendar.FloorDivide(SecondsSinceEpoch, Calendar.SecondsPerDay) * Calendar.SecondsPerDay));

    /// <summary>The hour, 0 to 23.</summary>
    public int Hour => SecondOfDay / 3_600;

    /// <summary>The minute, 0 to 59.</summary>
    public int Minute => SecondOfDay / 60 % 60;

    /// <summary>The second, 0 to 59.</summary>
    public int Second => SecondOfDay % 60;

    /// <summary>Midnight (00:00) at the start of <paramref name="date"/>.</summary>
    /// <exception cref="SimulationOverflowException">The date is too far from the epoch for the 64-bit clock.</exception>
    public static Instant FromDate(CalendarDate date) => FromDate(date, 0, 0, 0);

    /// <summary>The given time of day on <paramref name="date"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The hour, minute or second is outside the day.</exception>
    /// <exception cref="SimulationOverflowException">The date is too far from the epoch for the 64-bit clock.</exception>
    public static Instant FromDate(CalendarDate date, int hour, int minute, int second)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(hour);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(hour, 23);
        ArgumentOutOfRangeException.ThrowIfNegative(minute);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(minute, 59);
        ArgumentOutOfRangeException.ThrowIfNegative(second);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(second, 59);

        var days = Calendar.DaysSinceEpoch(date.Year, date.Month, date.Day);
        var seconds = ((Int128)days * Calendar.SecondsPerDay) + (hour * 3_600) + (minute * 60) + second;
        return new Instant(WideMath.ToInt64(seconds, "Instant.FromDate"));
    }

    /// <summary>The instant <paramref name="seconds"/> seconds later (earlier if negative).</summary>
    /// <exception cref="SimulationOverflowException">The result leaves the 64-bit clock range.</exception>
    public Instant AddSeconds(long seconds) =>
        new(WideMath.ToInt64((Int128)SecondsSinceEpoch + seconds, "Instant.AddSeconds"));

    /// <summary>Seconds from <paramref name="earlier"/> to this instant; negative if <paramref name="earlier"/> is actually later.</summary>
    /// <exception cref="SimulationOverflowException">The difference leaves the 64-bit range.</exception>
    public long SecondsSince(Instant earlier) =>
        WideMath.ToInt64((Int128)SecondsSinceEpoch - earlier.SecondsSinceEpoch, "Instant.SecondsSince");

    /// <summary>Orders instants by time.</summary>
    public int CompareTo(Instant other) => SecondsSinceEpoch.CompareTo(other.SecondsSinceEpoch);

    /// <summary>Whether <paramref name="left"/> is earlier than <paramref name="right"/>.</summary>
    public static bool operator <(Instant left, Instant right) => left.SecondsSinceEpoch < right.SecondsSinceEpoch;

    /// <summary>Whether <paramref name="left"/> is later than <paramref name="right"/>.</summary>
    public static bool operator >(Instant left, Instant right) => left.SecondsSinceEpoch > right.SecondsSinceEpoch;

    /// <summary>Whether <paramref name="left"/> is the same as or earlier than <paramref name="right"/>.</summary>
    public static bool operator <=(Instant left, Instant right) => left.SecondsSinceEpoch <= right.SecondsSinceEpoch;

    /// <summary>Whether <paramref name="left"/> is the same as or later than <paramref name="right"/>.</summary>
    public static bool operator >=(Instant left, Instant right) => left.SecondsSinceEpoch >= right.SecondsSinceEpoch;

    /// <summary>The instant as text, such as "1 January 10,000 BCE, 00:00:00".</summary>
    public override string ToString() =>
        Date + ", " + string.Create(CultureInfo.InvariantCulture, $"{Hour:D2}:{Minute:D2}:{Second:D2}");
}
