using System.Collections.Generic;
using System.Linq;
using Atlasiv.Core.Events;
using Atlasiv.Core.Orders;
using Atlasiv.Core.Replay;
using Atlasiv.Core.Scheduling;
using Atlasiv.Core.Time;
using Xunit;
using static Atlasiv.Core.Tests.Scheduling.RunHarness;

namespace Atlasiv.Core.Tests.Scheduling;

/// <summary>
/// The run's optional stop filter (spec clarifications 9 and 11): a stopping event lets its phase
/// (or turn end) finish, then the run stops at that boundary; calling the run again carries on
/// exactly where it stopped. J7 builds Advance Until's real stop list on this hook. Dates are
/// astronomical year-month-day (year -9,999 = 10,000 BCE).
/// </summary>
public sealed class StopAndContinueTests
{
    private static readonly NaturalPeriod EveryPhase = NaturalPeriod.PhaseOf(TurnStep.OneMonth);

    [Fact]
    public void A_stopping_event_mid_phase_stops_the_run_at_the_end_of_that_phase()
    {
        var run = Start(new ScriptedLeadingEdge(TurnStep.OneYear), new InputLog(), Alarm(Date(-9998, 5, 20)), Busy());

        var timeline = run.RunTurns(5, new StopOnDecisionCritical());

        var last = timeline.Entries[^1];
        Assert.Equal((2L, 2u), (last.Turn, last.Slot.Value));                  // 1 April to 1 July 9,999 BCE
        Assert.Equal(["alarm", "after the alarm"], last.Events.Select(e => ((Note)e.Payload).Text).Where(t => t != "news"));
        Assert.Equal(new Note("alarm"), timeline.StoppedBy?.Payload);
    }

    [Fact]
    public void A_stopping_event_at_a_turn_end_lets_the_turn_end_finish_and_stops_before_the_next_turn()
    {
        var turnEndAlarm = new ScriptedSystem(1, ResolutionStage.TurnEnd(2), EveryPhase,
            onRun: (c, _) =>
            {
                if (c.Turn == 1)
                {
                    c.Schedule(c.Now, ResolutionStage.TurnEnd(2), 0, InterruptionClass.DecisionCritical, new Note("famine"));
                }
            },
            onResolve: (_, _) => { });
        var late = new ScriptedSystem(2, ResolutionStage.TurnEnd(10), EveryPhase);
        var run = Start(new ScriptedLeadingEdge(TurnStep.OneYear), new InputLog(), turnEndAlarm, late);

        var timeline = run.RunTurns(3, new StopOnDecisionCritical());

        var last = timeline.Entries[^1];
        Assert.True(last.IsTurnEnd);
        Assert.Equal(1, last.Turn);
        Assert.Contains(last.Runs, r => r.Stage == ResolutionStage.TurnEnd(10));
        Assert.Equal(2, run.TurnNumber);
    }

    [Fact]
    public void Informational_events_do_not_stop_a_run_whose_filter_only_stops_on_decision_critical_ones()
    {
        var run = Start(new ScriptedLeadingEdge(TurnStep.OneYear), new InputLog(), Busy());

        var timeline = run.RunTurns(3, new StopOnDecisionCritical());

        Assert.Null(timeline.StoppedBy);
        Assert.Equal(3, timeline.TurnEnds.Count());
    }

    [Fact]
    public void Without_a_stop_filter_even_decision_critical_events_do_not_stop_the_run()
    {
        var run = Start(new ScriptedLeadingEdge(TurnStep.OneYear), new InputLog(), Alarm(Date(-9998, 5, 20)));

        var timeline = run.RunTurns(3);

        Assert.Null(timeline.StoppedBy);
        Assert.Equal(3, timeline.TurnEnds.Count());
    }

    [Fact]
    public void A_run_stopped_and_continued_records_exactly_what_an_uninterrupted_run_does()
    {
        var whole = Describe(StartScenario().RunTurns(40).Entries);

        var parts = new List<TimelineEntry>();
        var stopped = StartScenario();
        var turnsDone = 0L;
        var calls = 0;
        while (turnsDone < 40)
        {
            calls++;
            var part = stopped.RunTurns(40 - turnsDone, new StopOnDecisionCritical());
            parts.AddRange(part.Entries);
            turnsDone += part.TurnEnds.Count();
        }

        Assert.Equal(whole, Describe(parts));
        Assert.True(calls > 5, "The alarms should have stopped the run several times.");
    }

    [Fact]
    public void Running_turns_in_several_calls_records_exactly_what_one_call_does()
    {
        var whole = Describe(StartScenario().RunTurns(40).Entries);

        var run = StartScenario();
        var parts = new List<TimelineEntry>();
        foreach (var turns in new[] { 1, 0, 7, 13, 19 })
        {
            parts.AddRange(run.RunTurns(turns).Entries);
        }

        Assert.Equal(whole, Describe(parts));
    }

    /// <summary>
    /// A run with a step switch, phase and turn-end systems, informational events and a
    /// decision-critical alarm every 7th phase.
    /// </summary>
    private static SimulationRun StartScenario()
    {
        var edge = new ScriptedLeadingEdge(TurnStep.OneYear).Then(Date(-9979, 1, 1), TurnStep.SixMonths);
        var phaseCount = 0;
        var alarms = new ScriptedSystem(3, ResolutionStage.Phase(8), EveryPhase,
            onRun: (c, _) =>
            {
                if (++phaseCount % 7 == 0)
                {
                    c.Schedule(c.SlotSpan.End.AddSeconds(-1), ResolutionStage.Phase(8), 0, InterruptionClass.DecisionCritical, new Note("alarm"));
                }
            },
            onResolve: (_, _) => { });
        var yearly = new ScriptedSystem(4, ResolutionStage.TurnEnd(1), NaturalPeriod.TurnOf(TurnStep.OneYear));
        return Start(edge, new InputLog(), Busy(), alarms, yearly);
    }

    private static List<string> Describe(IEnumerable<TimelineEntry> entries) =>
        entries.Select(e =>
            $"{e.Turn} {e.Slot} {e.Step} {e.Span.Start.SecondsSinceEpoch}-{e.Span.End.SecondsSinceEpoch} "
            + string.Join(",", e.Runs.Select(r => $"{r.SystemId}@{r.Stage}:{r.Covered.Start.SecondsSinceEpoch}-{r.Covered.End.SecondsSinceEpoch}")) + " | "
            + string.Join(",", e.Events.Select(ev => $"{ev.Key} {ev.Payload}")) + " | " + e.Announced?.EffectiveAt)
        .ToList();

    /// <summary>A system that schedules one decision-critical alarm at <paramref name="at"/>, and a follow-up a second later.</summary>
    private static ScriptedSystem Alarm(Instant at)
    {
        var done = false;
        return new ScriptedSystem(1, ResolutionStage.Phase(1), EveryPhase,
            onRun: (c, _) =>
            {
                if (!done)
                {
                    done = true;
                    c.Schedule(at, ResolutionStage.Phase(7), 0, InterruptionClass.DecisionCritical, new Note("alarm"));
                }
            },
            onResolve: (e, c) =>
            {
                if (((Note)e.Payload).Text == "alarm")
                {
                    c.Schedule(c.Now.AddSeconds(1), ResolutionStage.Phase(7), 0, InterruptionClass.Informational, new Note("after the alarm"));
                }
            });
    }

    /// <summary>A system that schedules an informational event in the middle of every phase.</summary>
    private static ScriptedSystem Busy() =>
        new(2, ResolutionStage.Phase(4), EveryPhase,
            onRun: (c, _) => c.Schedule(c.Now.AddSeconds(c.SlotSpan.Seconds / 2), ResolutionStage.Phase(6), 0, InterruptionClass.Informational, new Note("news")),
            onResolve: (_, _) => { });

    private sealed class StopOnDecisionCritical : IStopFilter
    {
        public bool StopsOn(SimulationEvent<EventPayload> resolved) => resolved.Interruption == InterruptionClass.DecisionCritical;
    }
}
