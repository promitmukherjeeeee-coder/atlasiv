using System;
using System.Collections.Generic;
using System.Linq;
using Atlasiv.Core.Events;
using Atlasiv.Core.Orders;
using Atlasiv.Core.Replay;
using Atlasiv.Core.Scheduling;
using Atlasiv.Core.Time;
using FsCheck;
using FsCheck.Fluent;
using Xunit;
using static Atlasiv.Core.Tests.Scheduling.RunHarness;

namespace Atlasiv.Core.Tests.Scheduling;

/// <summary>
/// Simultaneous orders resolve against the same snapshot, never by internal list order (Pass 1
/// spec section 6): every system running in a phase sees all of that phase's orders at once, in
/// one canonical order that does not depend on the order they were logged in.
/// </summary>
public sealed class SimultaneousOrdersTests
{
    private static readonly NaturalPeriod EveryPhase = NaturalPeriod.PhaseOf(TurnStep.OneMonth);

    [Fact]
    public void Every_system_in_a_phase_sees_all_of_that_phases_orders_and_no_others()
    {
        var log = new InputLog();
        log.Add(1, 2, new Order(5, 0, new Claim(1, 10)));
        log.Add(1, 2, new Order(3, 0, new Claim(1, 20)));
        log.Add(1, 3, new Order(4, 0, new Claim(2, 30)));
        var seen = new List<(uint System, long Turn, uint Phase, ulong[] Issuers)>();
        ScriptedSystem Watcher(uint id, int stage) => new(id, ResolutionStage.Phase(stage), EveryPhase, onRun: (c, _) =>
            seen.Add((c.SystemId, c.Turn, c.Slot.Value, c.Orders.Select(o => o.IssuerId).ToArray())));
        var run = Start(new ScriptedLeadingEdge(TurnStep.OneYear), log, Watcher(1, 2), Watcher(2, 3));

        run.RunTurns(1);

        Assert.Equal([3ul, 5ul], seen.Single(s => s is { System: 1, Phase: 2 }).Issuers);
        Assert.Equal([3ul, 5ul], seen.Single(s => s is { System: 2, Phase: 2 }).Issuers);
        Assert.Equal([4ul], seen.Single(s => s is { System: 1, Phase: 3 }).Issuers);
        Assert.Empty(seen.Single(s => s is { System: 1, Phase: 1 }).Issuers);
        Assert.Empty(seen.Single(s => s is { System: 1, Phase: 4 }).Issuers);
    }

    [Fact]
    public void Turn_end_stages_see_no_orders()
    {
        var log = new InputLog();
        log.Add(1, 4, new Order(1, 0, new Claim(1, 1)));
        var counts = new List<int>();
        var system = new ScriptedSystem(1, ResolutionStage.TurnEnd(1), EveryPhase, onRun: (c, _) => counts.Add(c.Orders.Count));

        Start(new ScriptedLeadingEdge(TurnStep.OneYear), log, system).RunTurns(1);

        Assert.Equal([0], counts);
    }

    [Fact]
    public void The_orders_a_phase_presents_never_depend_on_the_order_they_were_logged_in()
    {
        var orders =
            from count in Gen.Choose(1, 12)
            from items in Gen.ListOf(
                from phase in Gen.Choose(1, 4)
                from issuer in Gen.Choose(1, 5)
                from sequence in Gen.Choose(0, 3)
                from province in Gen.Choose(1, 3)
                from bid in Gen.Choose(0, 4)
                select (Phase: phase, Order: new Order((ulong)issuer, (ulong)sequence, new Claim((ulong)province, bid))),
                count)
            let distinct = items.DistinctBy(i => (i.Phase, i.Order.IssuerId, i.Order.Sequence)).ToArray()
            from first in Gen.Shuffle(distinct)
            from second in Gen.Shuffle(distinct)
            select (First: first, Second: second);

        FsCheck.Check.One(Config.QuickThrowOnFailure.WithMaxTest(200), Prop.ForAll(orders.ToArbitrary(), c =>
            Outcome(c.First).SequenceEqual(Outcome(c.Second))));
    }

    [Fact]
    public void A_contest_resolved_against_the_whole_phase_gives_the_same_winner_whichever_claim_is_logged_first()
    {
        var claims = new[] { new Order(8, 0, new Claim(1, 40)), new Order(2, 0, new Claim(1, 70)), new Order(5, 0, new Claim(1, 70)) };

        var forwards = Outcome(claims.Select(o => (1, o)).ToArray());
        var backwards = Outcome(claims.Reverse().Select(o => (1, o)).ToArray());

        Assert.Equal(["turn 1 phase 1: province 1 contested by 2, 5"], forwards.Where(d => d.Contains("province", StringComparison.Ordinal)));
        Assert.Equal(forwards, backwards);
    }

    [Fact]
    public void A_phase_cannot_log_two_orders_with_the_same_issuer_and_sequence_number()
    {
        var log = new InputLog();
        log.Add(1, 1, new Order(1, 0, new Claim(1, 1)));

        Assert.Throws<ArgumentException>(() => log.Add(1, 1, new Order(1, 0, new Claim(2, 2))));
        log.Add(1, 2, new Order(1, 0, new Claim(2, 2)));                        // another phase is fine
    }

    /// <summary>
    /// Runs one 1-year turn with a contest system in phase stage 2 and returns what it decided
    /// and the orders it saw: for each province claimed in a phase, the highest bidder wins; equal
    /// top bids leave it contested.
    /// </summary>
    private static List<string> Outcome(IReadOnlyList<(int Phase, Order Order)> logged)
    {
        var log = new InputLog();
        foreach (var (phase, order) in logged)
        {
            log.Add(1, phase, order);
        }

        var decisions = new List<string>();
        var contest = new ScriptedSystem(1, ResolutionStage.Phase(2), EveryPhase, onRun: (c, _) =>
        {
            foreach (var province in c.Orders.Select(o => ((Claim)o.Payload).Province).Distinct().Order())
            {
                var bids = c.Orders.Where(o => ((Claim)o.Payload).Province == province).ToList();
                var top = bids.Max(o => ((Claim)o.Payload).Bid);
                var leaders = bids.Where(o => ((Claim)o.Payload).Bid == top).Select(o => o.IssuerId).Distinct().Order().ToList();
                decisions.Add(
                    $"turn {c.Turn} phase {c.Slot.Value}: province {province} "
                    + (leaders.Count == 1 ? $"won by {leaders[0]}" : "contested by " + string.Join(", ", leaders)));
            }

            decisions.Add($"turn {c.Turn} phase {c.Slot.Value}: issuers seen in order " + string.Join(" ", c.Orders.Select(o => $"{o.IssuerId}/{o.Sequence}")));
        });

        Start(new ScriptedLeadingEdge(TurnStep.OneYear), log, contest).RunTurns(1);
        return decisions;
    }

    private sealed record Claim(ulong Province, long Bid) : OrderPayload;
}
