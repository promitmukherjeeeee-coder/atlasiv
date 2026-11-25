using System;

namespace Atlasiv.Core.Time;

/// <summary>
/// The boundary calculator for one turn: given its step and start, the turn's end and every
/// phase's dates (Pass 1 spec section 3). Boundaries are calendar dates, not fixed second
/// counts, so month lengths and leap days are exact. Turns and phases are half-open
/// [start, end): an instant on a boundary belongs to the phase that begins there.
/// </summary>
/// <remarks>
/// <para>Phase rules, all at 00:00:</para>
/// <list type="bullet">
/// <item>Steps of a year or longer start every turn on 1 January.</item>
/// <item>50 years: phases every 30 months, alternating 1 January and 1 July.</item>
/// <item>20, 10 and 5 years: phases on 1 January.</item>
/// <item>2 years: 1 January and 1 July. 1 year: 1 January, 1 April, 1 July, 1 October.</item>
/// <item>6 months: turns start on 1 January or 1 July; phases on the 1st of the turn's first, third and fifth months.</item>
/// <item>1 month: turns start on the 1st; phases on days 1, 7, 13, 19 and 25, the last running to the month's end (4 to 7 days).</item>
/// </list>
/// <para>
/// Only the step and start are stored; everything else is computed on demand with a few
/// integer operations. The default value is the first 50-year turn at the epoch.
/// </para>
/// </remarks>
public readonly record struct TurnDates
{
    /// <summary>Days between phase starts on the 1-month step (days 1, 7, 13, 19, 25).</summary>
    private const int OneMonthPhaseDays = 6;

    private TurnDates(TurnStep step, Instant start)
    {
        Step = step;
        Start = start;
    }

    /// <summary>The step: the turn's calendar length.</summary>
    public TurnStep Step { get; }

    /// <summary>The turn's first instant.</summary>
    public Instant Start { get; }

    /// <summary>The first instant after the turn, which is also where the next turn starts.</summary>
    public Instant End => FirstOfMonth(StepTable.MonthsPerTurn(Step));

    /// <summary>The whole turn, [start, end).</summary>
    public TimeInterval Span => new(Start, End);

    /// <summary>The number of phases in the turn.</summary>
    public int PhaseCount => StepTable.PhaseCount(Step);

    /// <summary>The next turn on the same step, starting at this turn's end.</summary>
    public TurnDates Next => new(Step, End);

    /// <summary>The turn of step <paramref name="step"/> that starts at <paramref name="start"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The step is not one of the eight, or <paramref name="start"/> is not a valid turn start for it (<see cref="IsValidStart"/>).</exception>
    public static TurnDates Starting(TurnStep step, Instant start)
    {
        if (!IsValidStart(step, start))
        {
            throw new ArgumentOutOfRangeException(
                nameof(start), start, "Not a valid turn start for the " + step + " step.");
        }

        return new TurnDates(step, start);
    }

    /// <summary>
    /// Whether a turn of step <paramref name="step"/> may start at <paramref name="start"/>:
    /// 00:00 on 1 January for steps of a year or longer, on 1 January or 1 July for the
    /// 6-month step, and on the 1st of any month for the 1-month step.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The step is not one of the eight.</exception>
    public static bool IsValidStart(TurnStep step, Instant start)
    {
        var monthsPerTurn = StepTable.MonthsPerTurn(step);
        if (start.SecondOfDay != 0)
        {
            return false;
        }

        var date = start.Date;
        return date.Day == 1 && (date.Month - 1) % AlignmentMonths(monthsPerTurn) == 0;
    }

    /// <summary>Phase <paramref name="phase"/> (counting from 1) as [start, end).</summary>
    /// <exception cref="ArgumentOutOfRangeException">The phase is outside 1 to <see cref="PhaseCount"/>.</exception>
    public TimeInterval Phase(int phase)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(phase, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(phase, PhaseCount);
        return new TimeInterval(PhaseStart(phase), phase == PhaseCount ? End : PhaseStart(phase + 1));
    }

    /// <summary>Whether <paramref name="instant"/> falls in this turn, [start, end).</summary>
    public bool Contains(Instant instant) => Span.Contains(instant);

    /// <summary>The number (from 1) of the phase containing <paramref name="instant"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The instant is outside the turn.</exception>
    public int PhaseContaining(Instant instant)
    {
        if (!Contains(instant))
        {
            throw new ArgumentOutOfRangeException(nameof(instant), instant, "Outside the turn starting " + Start + ".");
        }

        var date = instant.Date;
        if (Step == TurnStep.OneMonth)
        {
            return int.Min((date.Day - 1) / OneMonthPhaseDays, PhaseCount - 1) + 1;
        }

        var start = Start.Date;
        var monthsIn = ((date.Year - start.Year) * 12) + (date.Month - start.Month);
        return (int)(monthsIn / StepTable.MonthsPerPhase(Step)) + 1;
    }

    private Instant PhaseStart(int phase) =>
        Step == TurnStep.OneMonth
            ? Start.AddSeconds((phase - 1) * OneMonthPhaseDays * Calendar.SecondsPerDay)
            : FirstOfMonth((phase - 1) * StepTable.MonthsPerPhase(Step));

    /// <summary>00:00 on the 1st of the month <paramref name="months"/> months after the turn's start month.</summary>
    private Instant FirstOfMonth(int months)
    {
        var start = Start.Date;
        var (year, month) = Calendar.AddMonths(start.Year, start.Month, months);
        return Instant.FromDate(new CalendarDate(year, month, 1));
    }

    // Turns of a year or more align to the year (12 months); shorter ones to their own length.
    private static int AlignmentMonths(int monthsPerTurn) => int.Min(monthsPerTurn, 12);
}
