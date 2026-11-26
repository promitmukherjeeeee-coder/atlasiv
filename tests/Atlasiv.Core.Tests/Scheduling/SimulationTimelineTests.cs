using System.Linq;
using Atlasiv.Core.Replay;
using Atlasiv.Core.Time;
using Xunit;
using static Atlasiv.Core.Tests.Scheduling.RunHarness;

namespace Atlasiv.Core.Tests.Scheduling;

/// <summary>
/// The timeline of a scripted run (Pass 1 spec sections 3 and 6, clarifications 2 and 16): each
/// turn's phases with their numbers, step and dates, followed by the turn end. Expected dates are
/// worked out by hand from the spec; dates are astronomical year-month-day (year -9,999 =
/// 10,000 BCE, which is not a leap year).
/// </summary>
public sealed class SimulationTimelineTests
{
    [Fact]
    public void Each_turn_lists_its_phases_in_order_then_its_turn_end()
    {
        var run = Start(TurnStep.OneMonth);

        var timeline = run.RunTurns(2);

        Assert.Equal(
            [
                (1L, "phase 1"), (1L, "phase 2"), (1L, "phase 3"), (1L, "phase 4"), (1L, "phase 5"), (1L, "turn end"),
                (2L, "phase 1"), (2L, "phase 2"), (2L, "phase 3"), (2L, "phase 4"), (2L, "phase 5"), (2L, "turn end"),
            ],
            timeline.Entries.Select(e => (e.Turn, e.Slot.ToString())));
        Assert.All(timeline.Entries, e => Assert.Equal(TurnStep.OneMonth, e.Step));
    }

    [Fact]
    public void One_month_phases_start_on_days_1_7_13_19_25_and_the_last_runs_to_the_month_end()
    {
        var run = Start(TurnStep.OneMonth);

        var timeline = run.RunTurns(2);

        // February 10,000 BCE has 28 days, so its last phase is 4 days long.
        Assert.Equal(
            [
                Span(Date(-9999, 2, 1), Date(-9999, 2, 7)), Span(Date(-9999, 2, 7), Date(-9999, 2, 13)),
                Span(Date(-9999, 2, 13), Date(-9999, 2, 19)), Span(Date(-9999, 2, 19), Date(-9999, 2, 25)),
                Span(Date(-9999, 2, 25), Date(-9999, 3, 1)),
            ],
            timeline.Phases.Where(p => p.Turn == 2).Select(p => p.Span));
    }

    [Fact]
    public void A_turn_end_covers_its_whole_turn_and_happens_at_the_turn_end_instant()
    {
        var run = Start(TurnStep.FiftyYears);

        var timeline = run.RunTurns(1);

        var turnEnd = Assert.Single(timeline.TurnEnds);
        Assert.Equal(Span(Instant.Epoch, Date(-9949, 1, 1)), turnEnd.Span);
        Assert.Equal(Date(-9949, 1, 1), turnEnd.At);
        Assert.Equal(20, turnEnd.PhaseCount);
    }

    [Fact]
    public void Fifty_year_phases_alternate_between_1_January_and_1_July()
    {
        var run = Start(TurnStep.FiftyYears);

        var timeline = run.RunTurns(1);

        Assert.Equal(20, timeline.Phases.Count());
        Assert.Equal(
            [Date(-9999, 1, 1), Date(-9997, 7, 1), Date(-9994, 1, 1), Date(-9992, 7, 1)],
            timeline.Phases.Take(4).Select(p => p.Span.Start));
        Assert.Equal(Span(Date(-9952, 7, 1), Date(-9949, 1, 1)), timeline.Phases.Last().Span);
    }

    [Fact]
    public void A_run_through_two_step_switches_keeps_turn_numbers_dates_and_phases_consistent_end_to_end()
    {
        // 1-year turns; from turn 21 (1 January 9,980 BCE) the candidate is 1 month. The controller
        // goes one step at a time: 6 months from turn 30 (1 January 9,971 BCE), then 1 month from
        // turn 50, 20 six-month turns later (1 January 9,961 BCE, a leap year).
        var edge = new ScriptedLeadingEdge(TurnStep.OneYear).Then(Date(-9979, 1, 1), TurnStep.OneMonth);
        var run = Start(edge, new InputLog());

        var timeline = run.RunTurns(52);

        Assert.Equal(
            [(TurnStep.SixMonths, 30L, Date(-9970, 1, 1)), (TurnStep.OneMonth, 50L, Date(-9960, 1, 1))],
            timeline.EraTransitions.Select(t => (t.NewStep, t.EffectiveTurn, t.EffectiveAt)));

        // Turn numbers run 1 to 52 without a gap or reset; each turn is its phases 1 to N, then its turn end.
        var turns = timeline.Entries.GroupBy(e => e.Turn).ToList();
        Assert.Equal(Enumerable.Range(1, 52).Select(t => (long)t), turns.Select(g => g.Key));
        foreach (var turn in turns)
        {
            var entries = turn.ToList();
            var count = entries[0].PhaseCount;
            Assert.Equal(
                Enumerable.Range(1, count).Select(p => TurnSlot.Phase(p)).Append(TurnSlot.TurnEnd),
                entries.Select(e => e.Slot));
            Assert.All(entries, e => Assert.Equal(entries[0].Step, e.Step));
            Assert.Equal(new TimeInterval(entries[0].Span.Start, entries[^2].Span.End), entries[^1].Span);
        }

        Assert.Equal(TurnStep.OneYear, timeline.Entries.First(e => e.Turn == 29).Step);
        Assert.Equal(TurnStep.SixMonths, timeline.Entries.First(e => e.Turn == 30).Step);
        Assert.Equal(TurnStep.OneMonth, timeline.Entries.First(e => e.Turn == 50).Step);

        // Turn 51 is February 9,961 BCE, a leap year: its last phase runs 25 February to 1 March, 5 days.
        Assert.Equal(Span(Date(-9960, 2, 25), Date(-9960, 3, 1)), timeline.Entries.Single(e => e.Turn == 51 && e.Slot == TurnSlot.Phase(5)).Span);
        Assert.Equal(5 * 86_400, timeline.Entries.Single(e => e.Turn == 51 && e.Slot == TurnSlot.Phase(5)).Span.Seconds);
    }

    [Fact]
    public void Each_phase_starts_where_the_last_one_ended()
    {
        var run = Start(TurnStep.SixMonths);

        var timeline = run.RunTurns(5);

        var phases = timeline.Phases.ToList();
        Assert.Equal(Instant.Epoch, phases[0].Span.Start);
        for (var i = 1; i < phases.Count; i++)
        {
            Assert.Equal(phases[i - 1].Span.End, phases[i].Span.Start);
        }
    }
}
