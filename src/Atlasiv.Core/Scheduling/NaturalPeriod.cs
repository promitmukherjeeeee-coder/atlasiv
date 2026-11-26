using System;
using Atlasiv.Core.Time;

namespace Atlasiv.Core.Scheduling;

/// <summary>
/// How much calendar time passes between a system's runs: one of the legal step or phase
/// lengths (spec clarification 8). "Due" follows the calendar: the period's boundaries are fixed
/// calendar dates counted from the epoch, and a system runs at the first opportunity in its
/// stage that reaches a boundary it has not yet passed (ENG-013).
/// </summary>
/// <remarks>
/// <para>
/// A period of whole months has a boundary at 00:00 on the 1st of every month whose distance in
/// months from the epoch (1 January 10,000 BCE) is a multiple of the period. A 1-year period's
/// boundaries are therefore every 1 January, a 6-month period's every 1 January and 1 July, and
/// a 30-month period's alternate between 1 January and 1 July, matching the 50-year step's
/// phases.
/// </para>
/// <para>
/// The 1-month step's phase length is not a whole number of months or days: its boundaries are
/// 00:00 on days 1, 7, 13, 19 and 25 of every month, the 1-month step's phase starts.
/// </para>
/// <para>The default value is not a valid period (<see cref="IsValid"/>).</para>
/// </remarks>
public readonly record struct NaturalPeriod
{
    // The value stored for the 1-month step's phase, which is not a whole number of months.
    private const int OneMonthPhase = -1;

    // Days of the month on which 1-month-step phases start.
    private static readonly int[] OneMonthPhaseDays = [1, 7, 13, 19, 25];

    // Whole months, or OneMonthPhase; 0 is invalid.
    private readonly int _months;

    private NaturalPeriod(int months) => _months = months;

    /// <summary>False only for the default value, which no factory can produce.</summary>
    public bool IsValid => _months != 0;

    /// <summary>The length of a turn of step <paramref name="step"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The step is not one of the eight.</exception>
    public static NaturalPeriod TurnOf(TurnStep step) => new(StepTable.MonthsPerTurn(step));

    /// <summary>
    /// The length of one phase of step <paramref name="step"/>. Equal lengths give equal
    /// periods: the 10-year and 5-year steps' phases are both 1 year, the same as a 1-year turn.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The step is not one of the eight.</exception>
    public static NaturalPeriod PhaseOf(TurnStep step) =>
        step == TurnStep.OneMonth ? new(OneMonthPhase) : new(StepTable.MonthsPerPhase(step));

    /// <summary>
    /// The first boundary of this period strictly after <paramref name="after"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">The period is the default value.</exception>
    internal Instant NextBoundaryAfter(Instant after)
    {
        if (!IsValid)
        {
            throw new InvalidOperationException("The default NaturalPeriod is not a period; use TurnOf or PhaseOf.");
        }

        var date = after.Date;
        if (_months == OneMonthPhase)
        {
            foreach (var day in OneMonthPhaseDays)
            {
                var candidate = Instant.FromDate(new CalendarDate(date.Year, date.Month, day));
                if (candidate > after)
                {
                    return candidate;
                }
            }

            var (nextYear, nextMonth) = Calendar.AddMonths(date.Year, date.Month, 1);
            return Instant.FromDate(new CalendarDate(nextYear, nextMonth, 1));
        }

        // The first 1st-of-month after `after` is at least one month on from its own month; the
        // boundary is the first such month whose distance from the epoch is a multiple of the period.
        var monthsFromEpoch = MonthsFromEpoch(date);
        var target = Calendar.FloorDivide(monthsFromEpoch, _months) * _months;
        while (target <= monthsFromEpoch)
        {
            target += _months;
        }

        var epoch = Instant.Epoch.Date;
        var (year, month) = Calendar.AddMonths(epoch.Year, epoch.Month, target);
        return Instant.FromDate(new CalendarDate(year, month, 1));
    }

    /// <summary>The period as text, such as "12 months" or "1-month-step phase".</summary>
    public override string ToString() =>
        !IsValid ? "no period"
        : _months == OneMonthPhase ? "1-month-step phase"
        : _months.ToString(System.Globalization.CultureInfo.InvariantCulture) + " months";

    private static long MonthsFromEpoch(CalendarDate date)
    {
        var epoch = Instant.Epoch.Date;
        return ((date.Year - epoch.Year) * 12) + (date.Month - epoch.Month);
    }
}
