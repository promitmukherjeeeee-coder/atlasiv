using System;
using System.Collections.Generic;
using Atlasiv.Core.Time;

namespace Atlasiv.Core.Integration;

/// <summary>
/// The substep driver: splits an interval into 1 to N calendar-exact substeps (spec section 10,
/// clarification 13). Every substep starts at 00:00 on a calendar date, and the interval's whole
/// days are shared out as evenly as possible: substep k (from 0) starts ⌊D·k/N⌋ days in, where D
/// is the number of days (owner ruling of 27 Nov 2026, ENG-014). A 365-day year in 4 substeps is
/// 91, 91, 91 and 92 days.
/// </summary>
public static class SubstepDriver
{
    /// <summary>
    /// The default substep budget (spec section 10, ADR-041). It is a budget, not a cap: a system
    /// may ask for more, and the run logs each such escalation.
    /// </summary>
    public const int DefaultBudget = 4;

    /// <summary>Splits <paramref name="interval"/> into <paramref name="count"/> contiguous half-open substeps.</summary>
    /// <exception cref="ArgumentException">The interval does not start and end at 00:00.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is below 1 or above the interval's number of days.</exception>
    public static IReadOnlyList<TimeInterval> Split(TimeInterval interval, int count)
    {
        if (interval.Start.SecondOfDay != 0 || interval.End.SecondOfDay != 0)
        {
            throw new ArgumentException(
                "Substeps are whole days, so the interval must start and end at 00:00 (" + interval.Start + " to " + interval.End + ").",
                nameof(interval));
        }

        var days = interval.Seconds / Calendar.SecondsPerDay;
        if (count < 1 || count > days)
        {
            throw new ArgumentOutOfRangeException(
                nameof(count), count, "An interval of " + days + " days splits into 1 to " + days + " substeps.");
        }

        var substeps = new TimeInterval[count];
        var start = interval.Start;
        for (var k = 0; k < count; k++)
        {
            var dayIn = (long)((Int128)days * (k + 1) / count); // floor: both are positive
            var end = k == count - 1 ? interval.End : interval.Start.AddSeconds(dayIn * Calendar.SecondsPerDay);
            substeps[k] = new TimeInterval(start, end);
            start = end;
        }

        return substeps;
    }
}
