using System.Collections.Generic;
using Atlasiv.Core.Time;
using Xunit;

namespace Atlasiv.Core.Tests.Time;

/// <summary>
/// The step controller (Pass 1 spec section 4, clarifications 4 to 7, 10 and 16), driven through
/// a scripted leading edge: each test sets the strategic time the controller will read at the
/// next turn boundary, ends turns one by one, and checks each turn's number and step and every
/// era-transition announcement. Expected turn numbers and dates are worked out by hand from
/// the spec; dates are astronomical year-month-day (year -9,999 = 10,000 BCE).
/// </summary>
public sealed class StepControllerTests
{
    [Fact]
    public void The_first_turn_starts_at_the_epoch_on_the_candidate_step()
    {
        var edge = new ScriptedLeadingEdge(TwoTurnsOf(TurnStep.TenYears));

        var controller = new StepController(edge);

        Assert.Equal(1, controller.TurnNumber);
        Assert.Equal(TurnStep.TenYears, controller.Turn.Step);
        Assert.Equal(Instant.Epoch, controller.Turn.Start);
        Assert.Null(controller.PendingSwitch);
        Assert.Equal([Instant.Epoch], edge.Readings);
    }

    [Fact]
    public void While_the_candidate_matches_turns_follow_each_other_on_the_same_step()
    {
        var edge = new ScriptedLeadingEdge(TwoTurnsOf(TurnStep.FiftyYears));
        var controller = new StepController(edge);

        for (var i = 0; i < 30; i++)
        {
            Assert.Null(controller.EndTurn());
        }

        Assert.Equal(31, controller.TurnNumber);
        Assert.Equal(TurnStep.FiftyYears, controller.Turn.Step);
        Assert.Equal(Date(-8499, 1, 1), controller.Turn.Start);   // 30 × 50 years after 10,000 BCE
        Assert.Null(controller.PendingSwitch);
    }

    [Fact]
    public void The_leading_edge_is_read_for_the_reference_undertaking_at_each_turn_boundary()
    {
        var edge = new ScriptedLeadingEdge(TwoTurnsOf(TurnStep.OneYear));
        var undertaking = new ReferenceUndertaking(4_000_000);
        var controller = new StepController(edge, undertaking);

        controller.EndTurn();
        controller.EndTurn();

        Assert.Equal([Instant.Epoch, Date(-9998, 1, 1), Date(-9997, 1, 1)], edge.Readings);
        Assert.All(edge.Undertakings, u => Assert.Equal(undertaking, u));
    }

    [Fact]
    public void The_uncalibrated_reference_undertaking_is_used_by_default()
    {
        var edge = new ScriptedLeadingEdge(TwoTurnsOf(TurnStep.OneYear));

        _ = new StepController(edge);

        Assert.Equal([ReferenceUndertaking.Uncalibrated], edge.Undertakings);
    }

    [Fact]
    public void A_faster_candidate_switches_after_10_consecutive_turns_at_the_next_boundary()
    {
        var edge = new ScriptedLeadingEdge(TwoTurnsOf(TurnStep.FiftyYears));
        var controller = new StepController(edge);
        Run(controller, edge, TurnStep.FiftyYears, 25);   // past the 20-turn gap: now on turn 26

        Assert.Empty(Run(controller, edge, TurnStep.TwentyYears, 9));
        Assert.Equal(TurnStep.FiftyYears, controller.Turn.Step);

        var announced = controller.EndTurn();             // the 10th: end of turn 35

        Assert.NotNull(announced);
        Assert.Equal(36, controller.TurnNumber);
        Assert.Equal(TurnStep.TwentyYears, controller.Turn.Step);
        Assert.Equal(Date(-8249, 1, 1), controller.Turn.Start);   // 35 × 50 years after the epoch
        Assert.Null(controller.PendingSwitch);
    }

    [Fact]
    public void A_faster_switch_is_announced_and_takes_effect_at_the_same_boundary()
    {
        var edge = new ScriptedLeadingEdge(TwoTurnsOf(TurnStep.FiftyYears));
        var controller = new StepController(edge);
        Run(controller, edge, TurnStep.FiftyYears, 25);

        var announced = Assert.Single(Run(controller, edge, TurnStep.TwentyYears, 10));

        Assert.Equal(TurnStep.FiftyYears, announced.OldStep);
        Assert.Equal(TurnStep.TwentyYears, announced.NewStep);
        Assert.Equal(Date(-8249, 1, 1), announced.AnnouncedAt);
        Assert.Equal(Date(-8249, 1, 1), announced.EffectiveAt);
        Assert.Equal(36, announced.EffectiveTurn);
        Assert.Equal(10, announced.NewPhaseCount);
        Assert.Equal(
            [
                Span(Date(-8249, 1, 1), Date(-8247, 1, 1)), Span(Date(-8247, 1, 1), Date(-8245, 1, 1)),
                Span(Date(-8245, 1, 1), Date(-8243, 1, 1)), Span(Date(-8243, 1, 1), Date(-8241, 1, 1)),
                Span(Date(-8241, 1, 1), Date(-8239, 1, 1)), Span(Date(-8239, 1, 1), Date(-8237, 1, 1)),
                Span(Date(-8237, 1, 1), Date(-8235, 1, 1)), Span(Date(-8235, 1, 1), Date(-8233, 1, 1)),
                Span(Date(-8233, 1, 1), Date(-8231, 1, 1)), Span(Date(-8231, 1, 1), Date(-8229, 1, 1)),
            ],
            announced.FirstTurnPhases);
    }

    [Fact]
    public void A_slower_candidate_switches_after_20_consecutive_turns()
    {
        var edge = new ScriptedLeadingEdge(TwoTurnsOf(TurnStep.FiveYears));
        var controller = new StepController(edge);
        Run(controller, edge, TurnStep.FiveYears, 25);   // now on turn 26

        Assert.Empty(Run(controller, edge, TurnStep.TenYears, 19));
        Assert.Equal(TurnStep.FiveYears, controller.Turn.Step);

        var announced = controller.EndTurn();             // the 20th: end of turn 45

        Assert.NotNull(announced);
        Assert.Equal(46, controller.TurnNumber);
        Assert.Equal(TurnStep.TenYears, controller.Turn.Step);   // every 5-year boundary is 1 January: no wait
        Assert.Equal(Date(-9774, 1, 1), controller.Turn.Start);  // 45 × 5 years after the epoch
    }

    [Fact]
    public void A_slower_switch_waits_for_1_January_or_1_July_and_previews_its_first_turn()
    {
        var edge = new ScriptedLeadingEdge(TwoTurnsOf(TurnStep.OneMonth));
        var controller = new StepController(edge);

        // Turn 1 is January 10,000 BCE; turns 1 to 20 each see a slower candidate, so the count
        // and the 20-turn gap both complete at the end of turn 20 (August 9,999 BCE).
        var announced = Assert.Single(Run(controller, edge, TurnStep.SixMonths, 20));

        Assert.Equal(TurnStep.OneMonth, announced.OldStep);
        Assert.Equal(TurnStep.SixMonths, announced.NewStep);
        Assert.Equal(Date(-9998, 9, 1), announced.AnnouncedAt);
        Assert.Equal(Date(-9997, 1, 1), announced.EffectiveAt);
        Assert.Equal(25, announced.EffectiveTurn);
        Assert.Equal(3, announced.NewPhaseCount);
        Assert.Equal(
            [
                Span(Date(-9997, 1, 1), Date(-9997, 3, 1)),
                Span(Date(-9997, 3, 1), Date(-9997, 5, 1)),
                Span(Date(-9997, 5, 1), Date(-9997, 7, 1)),
            ],
            announced.FirstTurnPhases);

        // September to December run on the old step while the switch waits.
        Assert.Equal(21, controller.TurnNumber);
        Assert.Equal(TurnDates.Starting(TurnStep.OneMonth, Date(-9998, 9, 1)), controller.Turn);
        Assert.Same(announced, controller.PendingSwitch);
        Assert.Empty(Run(controller, edge, TurnStep.SixMonths, 3));
        Assert.Equal(TurnDates.Starting(TurnStep.OneMonth, Date(-9998, 12, 1)), controller.Turn);

        Assert.Null(controller.EndTurn());
        Assert.Equal(25, controller.TurnNumber);
        Assert.Equal(announced.FirstTurn, controller.Turn);
        Assert.Null(controller.PendingSwitch);
    }

    [Fact]
    public void A_6_month_to_1_year_switch_waits_one_turn_for_1_January()
    {
        var edge = new ScriptedLeadingEdge(TwoTurnsOf(TurnStep.SixMonths));
        var controller = new StepController(edge);
        Run(controller, edge, TurnStep.SixMonths, 1);     // now on turn 2, July 10,000 BCE

        // Turns 2 to 21 see a slower candidate; turn 22 would start on 1 July 9,990 BCE.
        var announced = Assert.Single(Run(controller, edge, TurnStep.OneYear, 20));

        Assert.Equal(Date(-9989, 7, 1), announced.AnnouncedAt);
        Assert.Equal(Date(-9988, 1, 1), announced.EffectiveAt);
        Assert.Equal(23, announced.EffectiveTurn);
        Assert.Equal(TurnStep.SixMonths, controller.Turn.Step);
        controller.EndTurn();
        Assert.Equal(TurnStep.OneYear, controller.Turn.Step);
    }

    [Fact]
    public void An_announced_switch_is_committed_even_if_the_candidate_drifts_back()
    {
        var edge = new ScriptedLeadingEdge(TwoTurnsOf(TurnStep.OneMonth));
        var controller = new StepController(edge);
        Assert.Single(Run(controller, edge, TurnStep.SixMonths, 20));

        Assert.Empty(Run(controller, edge, TurnStep.OneMonth, 4));   // turns 21 to 24

        Assert.Equal(25, controller.TurnNumber);
        Assert.Equal(TurnStep.SixMonths, controller.Turn.Step);
    }

    [Fact]
    public void The_gap_counts_from_the_turn_the_switch_takes_effect()
    {
        var edge = new ScriptedLeadingEdge(TwoTurnsOf(TurnStep.OneMonth));
        var controller = new StepController(edge);
        Run(controller, edge, TurnStep.SixMonths, 24);     // announced after turn 20, effective on turn 25

        // A faster candidate from turn 25 completes its count after turn 34, but the step may
        // not change again until turn 45: 20 turns after the effective turn, not the announcement.
        Assert.Empty(Run(controller, edge, TurnStep.OneMonth, 19));
        var announced = controller.EndTurn();

        Assert.NotNull(announced);
        Assert.Equal(45, announced.EffectiveTurn);
        Assert.Equal(Date(-9987, 1, 1), announced.EffectiveAt);   // 20 six-month turns after 1 January 9,998 BCE
        Assert.Equal(TurnStep.OneMonth, controller.Turn.Step);
    }

    [Fact]
    public void A_candidate_equal_to_the_current_step_resets_a_faster_count()
    {
        var edge = new ScriptedLeadingEdge(TwoTurnsOf(TurnStep.TenYears));
        var controller = new StepController(edge);
        Run(controller, edge, TurnStep.TenYears, 25);

        Assert.Empty(Run(controller, edge, TurnStep.FiveYears, 9));
        Assert.Empty(Run(controller, edge, TurnStep.TenYears, 1));
        Assert.Empty(Run(controller, edge, TurnStep.FiveYears, 9));

        Assert.Equal(TurnStep.FiveYears, controller.EndTurn()?.NewStep);
    }

    [Fact]
    public void A_candidate_equal_to_the_current_step_resets_a_slower_count()
    {
        var edge = new ScriptedLeadingEdge(TwoTurnsOf(TurnStep.TenYears));
        var controller = new StepController(edge);
        Run(controller, edge, TurnStep.TenYears, 25);

        Assert.Empty(Run(controller, edge, TurnStep.TwentyYears, 19));
        Assert.Empty(Run(controller, edge, TurnStep.TenYears, 1));
        Assert.Empty(Run(controller, edge, TurnStep.TwentyYears, 19));

        Assert.Equal(TurnStep.TwentyYears, controller.EndTurn()?.NewStep);
    }

    [Fact]
    public void A_change_of_direction_resets_the_count()
    {
        var edge = new ScriptedLeadingEdge(TwoTurnsOf(TurnStep.TenYears));
        var controller = new StepController(edge);
        Run(controller, edge, TurnStep.TenYears, 25);

        Assert.Empty(Run(controller, edge, TurnStep.FiveYears, 9));
        Assert.Empty(Run(controller, edge, TurnStep.TwentyYears, 1));
        Assert.Empty(Run(controller, edge, TurnStep.FiveYears, 9));

        Assert.Equal(TurnStep.FiveYears, controller.EndTurn()?.NewStep);
    }

    [Fact]
    public void The_count_follows_direction_not_the_exact_candidate_and_a_switch_moves_one_step()
    {
        var edge = new ScriptedLeadingEdge(TwoTurnsOf(TurnStep.TenYears));
        var controller = new StepController(edge);
        Run(controller, edge, TurnStep.TenYears, 25);

        // Ten turns of faster candidates that keep changing: 5 years, 1 year, 5 years, ...
        var announcements = new List<EraTransition>();
        for (var i = 0; i < 10; i++)
        {
            announcements.AddRange(Run(controller, edge, i % 2 == 0 ? TurnStep.FiveYears : TurnStep.OneYear, 1));
        }

        var announced = Assert.Single(announcements);
        Assert.Equal(TurnStep.FiveYears, announced.NewStep);
        Assert.Equal(36, announced.EffectiveTurn);
    }

    [Fact]
    public void A_count_completed_inside_the_gap_fires_at_the_first_allowed_turn()
    {
        var edge = new ScriptedLeadingEdge(TwoTurnsOf(TurnStep.FiftyYears));
        var controller = new StepController(edge);

        // Faster from the end of turn 1: the count completes after turn 10, the gap after turn 20.
        Assert.Empty(Run(controller, edge, TurnStep.TwentyYears, 19));
        var announced = controller.EndTurn();

        Assert.Equal(21, announced?.EffectiveTurn);
        Assert.Equal(TurnStep.TwentyYears, controller.Turn.Step);
    }

    [Fact]
    public void A_count_completed_inside_the_gap_does_not_fire_if_it_no_longer_holds()
    {
        var edge = new ScriptedLeadingEdge(TwoTurnsOf(TurnStep.FiftyYears));
        var controller = new StepController(edge);

        Assert.Empty(Run(controller, edge, TurnStep.TwentyYears, 12));   // count 12 after turn 12
        Assert.Empty(Run(controller, edge, TurnStep.FiftyYears, 1));     // reset after turn 13

        // Counting again from turn 14: only 7 by turn 20, so the first allowed turn passes;
        // the 10th is turn 23, so the switch takes effect on turn 24.
        Assert.Empty(Run(controller, edge, TurnStep.TwentyYears, 9));
        Assert.Equal(24, controller.EndTurn()?.EffectiveTurn);
    }

    [Fact]
    public void A_fast_leading_edge_brings_the_step_down_one_step_every_20_turns()
    {
        var edge = new ScriptedLeadingEdge(TwoTurnsOf(TurnStep.FiftyYears));
        var controller = new StepController(edge);

        var announcements = Run(controller, edge, TurnStep.OneMonth, 150);

        Assert.Equal(
            [TurnStep.TwentyYears, TurnStep.TenYears, TurnStep.FiveYears, TurnStep.TwoYears, TurnStep.OneYear, TurnStep.SixMonths, TurnStep.OneMonth],
            announcements.ConvertAll(a => a.NewStep));
        Assert.Equal([21L, 41, 61, 81, 101, 121, 141], announcements.ConvertAll(a => a.EffectiveTurn));
        Assert.Equal(151, controller.TurnNumber);   // turn numbers never reset across switches
    }

    [Fact]
    public void After_a_collapse_the_step_slows_one_step_at_a_time()
    {
        var edge = new ScriptedLeadingEdge(TwoTurnsOf(TurnStep.OneMonth));
        var controller = new StepController(edge);

        var announcements = Run(controller, edge, TurnStep.FiftyYears, 150);

        Assert.Equal(
            [TurnStep.SixMonths, TurnStep.OneYear, TurnStep.TwoYears, TurnStep.FiveYears, TurnStep.TenYears, TurnStep.TwentyYears, TurnStep.FiftyYears],
            announcements.ConvertAll(a => a.NewStep));

        // The first waits from September to January (turns 21 to 24); every later boundary is
        // already 1 January, because the 20-turn gap is a whole number of years on each step.
        Assert.Equal([25L, 45, 65, 85, 105, 125, 145], announcements.ConvertAll(a => a.EffectiveTurn));
        Assert.Equal([false, true, true, true, true, true, true], announcements.ConvertAll(a => a.AnnouncedAt == a.EffectiveAt));
    }

    /// <summary>Sets the candidate to <paramref name="candidate"/> and ends <paramref name="turns"/> turns, returning every announcement.</summary>
    private static List<EraTransition> Run(StepController controller, ScriptedLeadingEdge edge, TurnStep candidate, int turns)
    {
        edge.StrategicSeconds = TwoTurnsOf(candidate);
        var announcements = new List<EraTransition>();
        for (var i = 0; i < turns; i++)
        {
            if (controller.EndTurn() is { } announced)
            {
                announcements.Add(announced);
            }
        }

        return announcements;
    }

    private static TimeInterval Span(Instant start, Instant end) => new(start, end);

    private static Instant Date(long year, int month, int day) => Instant.FromDate(new CalendarDate(year, month, day));

    /// <summary>A strategic time whose candidate is <paramref name="step"/>: exactly two of its turns.</summary>
    private static long TwoTurnsOf(TurnStep step) => 2 * StepTable.NominalSeconds(step);

    /// <summary>A leading edge whose strategic time the test sets before each turn boundary.</summary>
    private sealed class ScriptedLeadingEdge(long strategicSeconds) : ILeadingEdge
    {
        public long StrategicSeconds { get; set; } = strategicSeconds;

        public List<Instant> Readings { get; } = [];

        public List<ReferenceUndertaking> Undertakings { get; } = [];

        long ILeadingEdge.StrategicSeconds(ReferenceUndertaking undertaking, Instant asOf)
        {
            Readings.Add(asOf);
            Undertakings.Add(undertaking);
            return StrategicSeconds;
        }
    }
}
