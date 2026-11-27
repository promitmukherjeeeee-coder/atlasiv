using System.Collections.Generic;
using Atlasiv.Core.Time;

namespace Atlasiv.Core.Integration;

/// <summary>
/// Every exact length, in seconds, that a turn or a phase of any of the eight steps can have: the
/// lengths the propagator cache computes in advance (spec section 10, clarification 18).
/// </summary>
/// <remarks>
/// <para>
/// A step's months vary in days, so each legal length is a small set of exact lengths: a 1-year
/// turn is 365 or 366 days, a 1-month turn 28 to 31. The list is worked out over one 400-year
/// Gregorian cycle, after which the calendar repeats exactly, from every month start, so it covers
/// every turn and phase whatever month it starts in. It holds a few lengths no turn reaches (a
/// 6-month turn starting in February, say), which costs only a little precomputation.
/// </para>
/// <para>
/// The 1-month step's phases follow day numbers (four of 6 days, then one of 4 to 7), so their
/// lengths are taken from <see cref="TurnDates.Phase"/> for every month.
/// </para>
/// </remarks>
public static class LegalLengths
{
    private const int MonthsPer400Years = 4_800;

    /// <summary>Every legal turn and phase length in seconds, ascending, each once.</summary>
    public static IReadOnlyList<long> Seconds { get; } = Compute();

    private static long[] Compute()
    {
        var monthCounts = new SortedSet<int>();
        foreach (var step in StepTable.All)
        {
            monthCounts.Add(StepTable.MonthsPerTurn(step));
            if (step != TurnStep.OneMonth)
            {
                monthCounts.Add(StepTable.MonthsPerPhase(step));
            }
        }

        var days = new SortedSet<long>();
        var epoch = Instant.Epoch.Date;
        for (var m = 0; m < MonthsPer400Years; m++)
        {
            var (year, month) = Calendar.AddMonths(epoch.Year, epoch.Month, m);
            var first = new CalendarDate(year, month, 1);
            var start = Calendar.DaysSinceEpoch(first);

            // The 1-month step's phases follow day numbers, so take them from the calendar's own rule.
            var monthTurn = TurnDates.Starting(TurnStep.OneMonth, Instant.FromDate(first));
            for (var phase = 1; phase <= monthTurn.PhaseCount; phase++)
            {
                days.Add(monthTurn.Phase(phase).Seconds / Calendar.SecondsPerDay);
            }

            foreach (var count in monthCounts)
            {
                var (endYear, endMonth) = Calendar.AddMonths(year, month, count);
                days.Add(Calendar.DaysSinceEpoch(new CalendarDate(endYear, endMonth, 1)) - start);
            }
        }

        var seconds = new long[days.Count];
        var i = 0;
        foreach (var d in days)
        {
            seconds[i++] = d * Calendar.SecondsPerDay;
        }

        return seconds;
    }
}
