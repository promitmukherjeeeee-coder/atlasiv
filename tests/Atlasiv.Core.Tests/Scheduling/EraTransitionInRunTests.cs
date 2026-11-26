using System.Linq;
using Atlasiv.Core.Events;
using Atlasiv.Core.Replay;
using Atlasiv.Core.Scheduling;
using Atlasiv.Core.Time;
using Xunit;
using static Atlasiv.Core.Tests.Scheduling.RunHarness;

namespace Atlasiv.Core.Tests.Scheduling;

/// <summary>
/// Step switches seen through the run (Pass 1 spec section 4, clarifications 6 and 10): the turn
/// end where a switch is decided records it, and an informational era-transition event with the
/// preview resolves in the first phase of the turn that starts there. Dates are astronomical
/// year-month-day (year -9,999 = 10,000 BCE).
/// </summary>
public sealed class EraTransitionInRunTests
{
    [Fact]
    public void A_faster_switch_is_recorded_at_the_turn_end_and_announced_in_the_next_turns_first_phase()
    {
        // 50-year turns; a faster candidate from turn 27's start, so the 10th reading is at turn 35's end.
        var edge = new ScriptedLeadingEdge(TurnStep.FiftyYears).Then(Date(-8699, 1, 1), TurnStep.TwentyYears);
        var run = Start(edge, new InputLog());

        var timeline = run.RunTurns(36);

        var decided = Assert.Single(timeline.TurnEnds, t => t.Announced is not null);
        Assert.Equal(35, decided.Turn);
        var transition = decided.Announced!;
        Assert.Equal((TurnStep.FiftyYears, TurnStep.TwentyYears), (transition.OldStep, transition.NewStep));
        Assert.Equal(Date(-8249, 1, 1), transition.EffectiveAt);

        var firstPhase = timeline.Entries.Single(e => e.Turn == 36 && e.Slot == TurnSlot.Phase(1));
        Assert.Equal(TurnStep.TwentyYears, firstPhase.Step);
        var announcement = Assert.Single(firstPhase.Events);
        Assert.Equal(new EraTransitionAnnounced(transition), announcement.Payload);
        Assert.Equal(InterruptionClass.Informational, announcement.Interruption);
        Assert.Equal(
            new EventKey(Date(-8249, 1, 1), ResolutionStage.Phase(4), 0, new StableEventId(SystemRegistration.ClockSystemId, 35, TurnSlot.TurnEnd, 0)),
            announcement.Key);
    }

    [Fact]
    public void A_slower_switch_is_announced_when_decided_and_the_old_step_runs_on_until_it_takes_effect()
    {
        // 1-month turns; a slower candidate from 1 February 10,000 BCE. Decided at 1 September
        // 9,999 BCE (turn 21's start), effective 1 January 9,998 BCE (turn 25).
        var edge = new ScriptedLeadingEdge(TurnStep.OneMonth).Then(Date(-9999, 2, 1), TurnStep.SixMonths);
        var run = Start(edge, new InputLog());

        var timeline = run.RunTurns(25);

        var transition = Assert.Single(timeline.EraTransitions);
        Assert.Equal(Date(-9998, 9, 1), transition.AnnouncedAt);
        Assert.Equal(25, transition.EffectiveTurn);
        Assert.Equal(3, transition.NewPhaseCount);
        Assert.Equal(
            [Span(Date(-9997, 1, 1), Date(-9997, 3, 1)), Span(Date(-9997, 3, 1), Date(-9997, 5, 1)), Span(Date(-9997, 5, 1), Date(-9997, 7, 1))],
            transition.FirstTurnPhases);

        var announcedIn = Assert.Single(timeline.Entries, e => e.Events.Any(ev => ev.Payload is EraTransitionAnnounced));
        Assert.Equal((21L, 1u, TurnStep.OneMonth), (announcedIn.Turn, announcedIn.Slot.Value, announcedIn.Step));
        Assert.Equal(
            [TurnStep.OneMonth, TurnStep.OneMonth, TurnStep.OneMonth, TurnStep.OneMonth, TurnStep.SixMonths],
            timeline.TurnEnds.Where(t => t.Turn >= 21).Select(t => t.Step));
    }
}
