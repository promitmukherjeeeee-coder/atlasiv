using System;
using System.Collections.Generic;
using Atlasiv.Core.Numerics;

namespace Atlasiv.Core.Time;

/// <summary>
/// A step: the calendar length of a turn, which also fixes its phase count (Pass 1 spec
/// section 3). Values run from slowest (longest) to fastest (shortest), so "one step
/// faster" is the next value up.
/// </summary>
public enum TurnStep
{
    /// <summary>50-year turns of 20 phases (2.5 years each).</summary>
    FiftyYears = 0,

    /// <summary>20-year turns of 10 phases (2 years each).</summary>
    TwentyYears = 1,

    /// <summary>10-year turns of 10 phases (1 year each).</summary>
    TenYears = 2,

    /// <summary>5-year turns of 5 phases (1 year each).</summary>
    FiveYears = 3,

    /// <summary>2-year turns of 4 phases (6 months each).</summary>
    TwoYears = 4,

    /// <summary>1-year turns of 4 phases (3 months each).</summary>
    OneYear = 5,

    /// <summary>6-month turns of 3 phases (2 months each).</summary>
    SixMonths = 6,

    /// <summary>1-month turns of 5 phases starting on days 1, 7, 13, 19 and 25.</summary>
    OneMonth = 7,
}

/// <summary>The step table (Pass 1 spec section 3).</summary>
public static class StepTable
{
    // One twelfth of the rate year, 31,556,952 / 12, which divides exactly.
    private const long NominalMonthSeconds = RateYear.Seconds / 12;

    // Indexed by step.
    private static readonly int[] TurnMonths = [600, 240, 120, 60, 24, 12, 6, 1];
    private static readonly int[] Phases = [20, 10, 10, 5, 4, 4, 3, 5];

    /// <summary>Every step, slowest first.</summary>
    public static IReadOnlyList<TurnStep> All { get; } =
        [TurnStep.FiftyYears, TurnStep.TwentyYears, TurnStep.TenYears, TurnStep.FiveYears, TurnStep.TwoYears, TurnStep.OneYear, TurnStep.SixMonths, TurnStep.OneMonth];

    /// <summary>The turn length in calendar months.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The step is not one of the eight.</exception>
    public static int MonthsPerTurn(TurnStep step) => TurnMonths[Index(step)];

    /// <summary>
    /// The nominal turn length in whole seconds: its months × one twelfth of the rate year
    /// (2,629,746 s, exact). Used only to compare steps (spec clarification 7); real turn
    /// boundaries are calendar dates (<see cref="TurnDates"/>).
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The step is not one of the eight.</exception>
    public static long NominalSeconds(TurnStep step) => MonthsPerTurn(step) * NominalMonthSeconds;

    /// <summary>The number of phases in each turn.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The step is not one of the eight.</exception>
    public static int PhaseCount(TurnStep step) => Phases[Index(step)];

    /// <summary>
    /// The phase length in calendar months. Every step except 1 month has phases of whole
    /// months; the 1-month step's phases follow day numbers instead (see <see cref="TurnDates"/>).
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The step is the 1-month step, or not one of the eight.</exception>
    internal static int MonthsPerPhase(TurnStep step)
    {
        if (step == TurnStep.OneMonth)
        {
            throw new ArgumentOutOfRangeException(nameof(step), step, "1-month phases are not whole months.");
        }

        return MonthsPerTurn(step) / PhaseCount(step);
    }

    private static int Index(TurnStep step)
    {
        if (step is < TurnStep.FiftyYears or > TurnStep.OneMonth)
        {
            throw new ArgumentOutOfRangeException(nameof(step), step, "Not one of the eight steps.");
        }

        return (int)step;
    }
}
