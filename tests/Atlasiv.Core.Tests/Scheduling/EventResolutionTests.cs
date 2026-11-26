using System;
using System.Collections.Generic;
using System.Linq;
using Atlasiv.Core.Events;
using Atlasiv.Core.Replay;
using Atlasiv.Core.Scheduling;
using Atlasiv.Core.Time;
using Xunit;
using static Atlasiv.Core.Tests.Scheduling.RunHarness;

namespace Atlasiv.Core.Tests.Scheduling;

/// <summary>
/// How the run resolves queued events (Pass 1 spec sections 3, 5 and 6): in the phase that
/// contains their timestamp, in event-key order, interleaved with the stages, and with the old
/// turn's turn end always before the new turn's first phase. Dates are astronomical
/// year-month-day (year -9,999 = 10,000 BCE).
/// </summary>
public sealed class EventResolutionTests
{
    private static readonly NaturalPeriod EveryPhase = NaturalPeriod.PhaseOf(TurnStep.OneMonth);

    [Fact]
    public void An_event_resolves_in_the_phase_containing_its_timestamp_and_its_creator_resolves_it()
    {
        var resolvedBy = new List<(string Text, Instant Now)>();
        var scheduled = false;
        var system = new ScriptedSystem(
            1,
            ResolutionStage.Phase(4),
            EveryPhase,
            onRun: (context, _) =>
            {
                if (!scheduled)
                {
                    scheduled = true;
                    context.Schedule(Date(-9999, 5, 15), ResolutionStage.Phase(7), 0, InterruptionClass.Informational, new Note("battle"));
                }
            },
            onResolve: (e, context) => resolvedBy.Add((((Note)e.Payload).Text, context.Now)));
        var run = Start(TurnStep.OneYear, system);

        var timeline = run.RunTurns(1);

        var phase = Assert.Single(timeline.Phases, p => p.Events.Count > 0);
        Assert.Equal(2u, phase.Slot.Value);                                    // 1 April to 1 July
        Assert.Equal(new Note("battle"), Assert.Single(phase.Events).Payload);
        Assert.Equal([("battle", Date(-9999, 5, 15))], resolvedBy);
    }

    [Fact]
    public void An_event_exactly_on_a_phase_boundary_resolves_in_the_phase_that_starts_there()
    {
        var run = Start(TurnStep.OneYear, OneShot(Date(-9999, 7, 1), ResolutionStage.Phase(4)));

        var timeline = run.RunTurns(1);

        Assert.Equal(3u, Assert.Single(timeline.Phases, p => p.Events.Count > 0).Slot.Value);   // 1 July to 1 October
    }

    [Fact]
    public void An_event_at_the_turn_boundary_resolves_in_the_next_turns_first_phase_after_the_turn_end()
    {
        // Scheduled from the first phase for 00:00 on 1 January 9,999 BCE, where turn 2 begins.
        var log = new List<string>();
        var oneShot = OneShot(Date(-9998, 1, 1), ResolutionStage.Phase(1), onResolve: () => log.Add("event"));
        var turnEnd = new ScriptedSystem(2, ResolutionStage.TurnEnd(10), EveryPhase, onRun: (_, _) => log.Add("turn end 10"));
        var run = Start(new ScriptedLeadingEdge(TurnStep.OneYear), new InputLog(), oneShot, turnEnd);

        var timeline = run.RunTurns(2);

        Assert.Equal(["turn end 10", "event", "turn end 10"], log);
        var entry = Assert.Single(timeline.Entries, e => e.Events.Count > 0);
        Assert.Equal((2L, 1u), (entry.Turn, entry.Slot.Value));
    }

    [Fact]
    public void An_event_at_the_phase_start_in_an_earlier_stage_resolves_before_later_stages_run()
    {
        var log = new List<string>();
        var start = Date(-9999, 4, 1);
        var scheduler = OneShot(start, ResolutionStage.Phase(3), onResolve: () => log.Add("stage-3 event"));
        var later = new ScriptedSystem(2, ResolutionStage.Phase(8), EveryPhase, onRun: (c, _) =>
        {
            if (c.Now == start)
            {
                log.Add("stage-8 system");
            }
        });
        var earlier = new ScriptedSystem(3, ResolutionStage.Phase(2), EveryPhase, onRun: (c, _) =>
        {
            if (c.Now == start)
            {
                log.Add("stage-2 system");
            }
        });
        var run = Start(new ScriptedLeadingEdge(TurnStep.OneYear), new InputLog(), scheduler, later, earlier);

        run.RunTurns(1);

        Assert.Equal(["stage-2 system", "stage-3 event", "stage-8 system"], log);
    }

    [Fact]
    public void Events_in_a_phase_resolve_by_timestamp_then_stage_then_priority_then_ID()
    {
        // Scheduled in the first phase, in an order unlike the resolution order, for 10 February,
        // in the second phase (7 to 13 February) of turn 2.
        var at = Date(-9999, 2, 10);
        var system = new ScriptedSystem(1, ResolutionStage.Phase(4), EveryPhase, onRun: (c, _) =>
        {
            if (c.Turn == 1 && c.Slot.Value == 1)
            {
                c.Schedule(at.AddSeconds(1), ResolutionStage.Phase(4), 0, InterruptionClass.Informational, new Note("d"));
                c.Schedule(at, ResolutionStage.Phase(9), 0, InterruptionClass.Informational, new Note("c"));
                c.Schedule(at, ResolutionStage.Phase(6), 5, InterruptionClass.Informational, new Note("b2"));
                c.Schedule(at, ResolutionStage.Phase(6), 5, InterruptionClass.Informational, new Note("b3"));
                c.Schedule(at, ResolutionStage.Phase(6), -1, InterruptionClass.Informational, new Note("b1"));
                c.Schedule(at, ResolutionStage.Phase(5), 9, InterruptionClass.Informational, new Note("a"));
            }
        }, onResolve: (_, _) => { });

        var timeline = Start(TurnStep.OneMonth, system).RunTurns(2);

        var phase = Assert.Single(timeline.Phases, p => p.Events.Count > 0);
        Assert.Equal((2L, 2u), (phase.Turn, phase.Slot.Value));
        Assert.Equal(["a", "b1", "b2", "b3", "c", "d"], phase.Events.Select(e => ((Note)e.Payload).Text));
    }
    [Fact]
    public void An_event_can_schedule_a_follow_up_that_resolves_later_in_the_same_phase()
    {
        var first = Date(-9999, 3, 2);
        var system = new ScriptedSystem(1, ResolutionStage.Phase(4), EveryPhase,
            onRun: (c, _) =>
            {
                if (c.Turn == 1 && c.Slot.Value == 1)
                {
                    c.Schedule(first, ResolutionStage.Phase(7), 0, InterruptionClass.Informational, new Note("siege"));
                }
            },
            onResolve: (e, c) =>
            {
                if (((Note)e.Payload).Text == "siege")
                {
                    c.Schedule(c.Now.AddSeconds(3_600), ResolutionStage.Phase(7), 0, InterruptionClass.Informational, new Note("surrender"));
                }
            });
        var run = Start(TurnStep.SixMonths, system);

        var timeline = run.RunTurns(1);

        var phase = Assert.Single(timeline.Phases, p => p.Events.Count > 0);
        Assert.Equal(2u, phase.Slot.Value);                                     // 1 March to 1 May
        Assert.Equal(["siege", "surrender"], phase.Events.Select(e => ((Note)e.Payload).Text));
    }

    [Fact]
    public void Turn_end_stage_events_resolve_at_the_turn_end_in_stage_order()
    {
        var log = new List<string>();
        var phaseSystem = new ScriptedSystem(1, ResolutionStage.Phase(4), EveryPhase,
            onRun: (c, _) =>
            {
                if (c.Slot.Value == 1)
                {
                    c.Schedule(c.TurnSpan.End, ResolutionStage.TurnEnd(6), 0, InterruptionClass.Informational, new Note("ledger"));
                }
            },
            onResolve: (_, _) => log.Add("turn-end-6 event"));
        var stage5 = new ScriptedSystem(2, ResolutionStage.TurnEnd(5), EveryPhase, onRun: (_, _) => log.Add("turn-end-5 system"));
        var stage7 = new ScriptedSystem(3, ResolutionStage.TurnEnd(7), EveryPhase, onRun: (_, _) => log.Add("turn-end-7 system"));
        var run = Start(new ScriptedLeadingEdge(TurnStep.OneYear), new InputLog(), phaseSystem, stage5, stage7);

        var timeline = run.RunTurns(1);

        Assert.Equal(["turn-end-5 system", "turn-end-6 event", "turn-end-7 system"], log);
        Assert.Single(timeline.TurnEnds.Single().Events);
    }

    [Fact]
    public void A_turn_end_stage_event_must_be_timestamped_at_the_current_turns_end()
    {
        var system = new ScriptedSystem(1, ResolutionStage.Phase(4), EveryPhase, onRun: (c, _) =>
            c.Schedule(c.SlotSpan.End, ResolutionStage.TurnEnd(2), 0, InterruptionClass.Informational, new Note("early")));   // the end of phase 1, not of the turn
        var run = Start(TurnStep.OneYear, system);

        Assert.Throws<ArgumentException>(() => run.RunTurns(1));
    }

    [Fact]
    public void An_event_cannot_be_scheduled_before_the_point_the_run_has_reached()
    {
        var past = new ScriptedSystem(1, ResolutionStage.Phase(4), EveryPhase, onRun: (c, _) =>
            c.Schedule(c.Now.AddSeconds(-1), ResolutionStage.Phase(9), 0, InterruptionClass.Informational, new Note("past")));
        var earlierStage = new ScriptedSystem(1, ResolutionStage.Phase(4), EveryPhase, onRun: (c, _) =>
            c.Schedule(c.Now, ResolutionStage.Phase(3), 0, InterruptionClass.Informational, new Note("earlier stage")));

        Assert.Throws<ArgumentException>(() => Start(TurnStep.OneYear, past).RunTurns(1));
        Assert.Throws<ArgumentException>(() => Start(TurnStep.OneYear, earlierStage).RunTurns(1));
    }

    [Fact]
    public void A_context_kept_after_its_call_has_returned_cannot_schedule_events()
    {
        SystemContext? kept = null;
        var keeper = new ScriptedSystem(1, ResolutionStage.Phase(2), EveryPhase, onRun: (c, _) => kept ??= c, onResolve: (_, _) => { });
        var user = new ScriptedSystem(2, ResolutionStage.Phase(3), EveryPhase, onRun: (_, _) =>
            kept!.Schedule(kept.SlotSpan.End.AddSeconds(-1), ResolutionStage.Phase(9), 0, InterruptionClass.Informational, new Note("stale")));
        var run = Start(new ScriptedLeadingEdge(TurnStep.OneYear), new InputLog(), keeper, user);

        Assert.Throws<InvalidOperationException>(() => run.RunTurns(1));
    }

    [Fact]
    public void An_event_far_in_the_future_waits_in_the_queue_until_its_phase()
    {
        var run = Start(TurnStep.TenYears, OneShot(Date(-9969, 3, 1), ResolutionStage.Phase(4)));

        var timeline = run.RunTurns(5);

        var entry = Assert.Single(timeline.Entries, e => e.Events.Count > 0);
        Assert.Equal((4L, 1u), (entry.Turn, entry.Slot.Value));                // turn 4 starts 1 January 9,970 BCE
    }

    [Fact]
    public void Event_IDs_carry_the_creating_system_the_turn_and_the_slot_and_number_from_0_in_each_slot()
    {
        var ids = new List<StableEventId>();
        var system = new ScriptedSystem(9, ResolutionStage.Phase(4), EveryPhase,
            onRun: (c, _) =>
            {
                ids.Add(c.Schedule(c.SlotSpan.End.AddSeconds(-1), ResolutionStage.Phase(6), 0, InterruptionClass.Informational, new Note("a")));
                ids.Add(c.Schedule(c.SlotSpan.End.AddSeconds(-1), ResolutionStage.Phase(6), 0, InterruptionClass.Informational, new Note("b")));
            },
            onResolve: (_, _) => { });
        var run = Start(TurnStep.OneYear, system);

        run.RunTurns(1);

        Assert.Equal(8, ids.Count);
        Assert.All(ids, id => Assert.Equal((9u, 1L), (id.SystemId, id.Turn)));
        Assert.Equal([1u, 1u, 2u, 2u, 3u, 3u, 4u, 4u], ids.Select(id => id.Slot.Value));
        Assert.Equal([0ul, 1ul, 0ul, 1ul, 0ul, 1ul, 0ul, 1ul], ids.Select(id => id.Sequence));
    }

    [Fact]
    public void A_system_running_and_resolving_in_one_slot_shares_one_ID_sequence()
    {
        var ids = new List<StableEventId>();
        var system = new ScriptedSystem(9, ResolutionStage.Phase(4), EveryPhase,
            onRun: (c, _) =>
            {
                if (c.Slot.Value == 1)
                {
                    ids.Add(c.Schedule(c.Now.AddSeconds(10), ResolutionStage.Phase(6), 0, InterruptionClass.Informational, new Note("first")));
                }
            },
            onResolve: (e, c) =>
            {
                if (((Note)e.Payload).Text == "first")
                {
                    ids.Add(c.Schedule(c.Now.AddSeconds(10), ResolutionStage.Phase(6), 0, InterruptionClass.Informational, new Note("second")));
                }
            });
        var run = Start(TurnStep.OneYear, system);

        run.RunTurns(1);

        Assert.Equal([0ul, 1ul], ids.Select(id => id.Sequence));
    }

    /// <summary>A system that, in the first phase it runs, schedules one event at <paramref name="at"/> in <paramref name="stage"/>.</summary>
    private static ScriptedSystem OneShot(Instant at, ResolutionStage stage, Action? onResolve = null)
    {
        var done = false;
        return new ScriptedSystem(
            1,
            ResolutionStage.Phase(1),
            EveryPhase,
            onRun: (c, _) =>
            {
                if (!done)
                {
                    done = true;
                    c.Schedule(at, stage, 0, InterruptionClass.Informational, new Note("one shot"));
                }
            },
            onResolve: (_, _) => onResolve?.Invoke());
    }
}
