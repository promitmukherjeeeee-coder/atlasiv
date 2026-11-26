using System;
using Atlasiv.Core.Events;
using Atlasiv.Core.Scheduling;
using Atlasiv.Core.Time;
using FsCheck.Fluent;
using Xunit;

namespace Atlasiv.Core.Tests.Events;

/// <summary>
/// The event ordering key: (timestamp, resolution stage, event priority, stable event ID)
/// (spec section 5). At the same instant, turn-end stages come before phase stages (owner
/// ruling, 26 Nov 2026, from the section 3 boundary rule); lower priority numbers come first
/// (owner ruling, 26 Nov 2026).
/// </summary>
public sealed class EventKeyTests
{
    private static readonly Instant T = new(1_000);
    private static readonly StableEventId IdA = new(1, 5, TurnSlot.Phase(1), 0);
    private static readonly StableEventId IdB = new(1, 5, TurnSlot.Phase(1), 1);

    private static void AssertBefore(EventKey earlier, EventKey later)
    {
        Assert.True(earlier.CompareTo(later) < 0, $"{earlier} should resolve before {later}");
        Assert.True(later.CompareTo(earlier) > 0, $"{later} should resolve after {earlier}");
    }

    // ---- Each field outranks every later field ----

    [Fact]
    public void An_earlier_timestamp_resolves_first_whatever_the_other_fields()
    {
        AssertBefore(
            new EventKey(T, ResolutionStage.Phase(11), int.MaxValue, IdB),
            new EventKey(T.AddSeconds(1), ResolutionStage.TurnEnd(1), int.MinValue, IdA));
    }

    [Fact]
    public void At_the_same_instant_an_earlier_stage_resolves_first_whatever_the_priority_and_id()
    {
        AssertBefore(
            new EventKey(T, ResolutionStage.Phase(4), int.MaxValue, IdB),
            new EventKey(T, ResolutionStage.Phase(5), int.MinValue, IdA));
    }

    [Fact]
    public void At_the_same_instant_and_stage_a_lower_priority_number_resolves_first_whatever_the_id()
    {
        AssertBefore(
            new EventKey(T, ResolutionStage.Phase(4), 0, IdB),
            new EventKey(T, ResolutionStage.Phase(4), 1, IdA));
    }

    [Fact]
    public void When_everything_else_ties_the_stable_id_decides()
    {
        AssertBefore(
            new EventKey(T, ResolutionStage.Phase(4), 0, IdA),
            new EventKey(T, ResolutionStage.Phase(4), 0, IdB));
    }

    // ---- Stage order ----

    [Fact]
    public void Turn_end_stages_resolve_before_phase_stages_at_the_same_instant()
    {
        // On a turn boundary the old turn's turn-end stages run before the new turn's first
        // phase (spec section 3), so the last turn-end stage still comes before phase stage 1.
        AssertBefore(
            new EventKey(T, ResolutionStage.TurnEnd(10), 0, IdB),
            new EventKey(T, ResolutionStage.Phase(1), 0, IdA));
    }

    [Fact]
    public void Stages_resolve_in_their_numbered_order()
    {
        for (var n = 1; n < ResolutionStage.PhaseStageCount; n++)
        {
            AssertBefore(
                new EventKey(T, ResolutionStage.Phase(n), 0, IdB),
                new EventKey(T, ResolutionStage.Phase(n + 1), 0, IdA));
        }

        for (var n = 1; n < ResolutionStage.TurnEndStageCount; n++)
        {
            AssertBefore(
                new EventKey(T, ResolutionStage.TurnEnd(n), 0, IdB),
                new EventKey(T, ResolutionStage.TurnEnd(n + 1), 0, IdA));
        }
    }

    [Fact]
    public void There_are_11_phase_stages_and_10_turn_end_stages()
    {
        Assert.Equal(11, ResolutionStage.PhaseStageCount);
        Assert.Equal(10, ResolutionStage.TurnEndStageCount);
        Assert.Throws<ArgumentOutOfRangeException>(() => ResolutionStage.Phase(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => ResolutionStage.Phase(12));
        Assert.Throws<ArgumentOutOfRangeException>(() => ResolutionStage.TurnEnd(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => ResolutionStage.TurnEnd(11));
        Assert.False(default(ResolutionStage).IsValid);
    }

    // ---- No two events ever compare equal ----

    [Fact]
    public void Keys_with_different_stable_ids_never_compare_equal()
    {
        EventGen.Check(Prop.ForAll(
            EventGen.Key().ToArbitrary(),
            EventGen.Key().ToArbitrary(),
            (a, b) =>
            {
                var ab = a.CompareTo(b);
                var ba = b.CompareTo(a);
                var sameId = a.Id == b.Id;
                return (sameId || ab != 0) && Math.Sign(ab) == -Math.Sign(ba);
            }));
    }

    [Fact]
    public void Key_order_is_transitive()
    {
        EventGen.Check(Prop.ForAll(
            EventGen.Key().ToArbitrary(),
            EventGen.Key().ToArbitrary(),
            EventGen.Key().ToArbitrary(),
            (a, b, c) => !(a.CompareTo(b) <= 0 && b.CompareTo(c) <= 0) || a.CompareTo(c) <= 0));
    }
}
