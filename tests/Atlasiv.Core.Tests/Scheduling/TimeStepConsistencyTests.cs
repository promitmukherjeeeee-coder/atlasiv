using System;
using System.Collections.Generic;
using System.Linq;
using Atlasiv.Core.Integration;
using Atlasiv.Core.Numerics;
using Atlasiv.Core.Scheduling;
using Atlasiv.Core.Tests.Time;
using Atlasiv.Core.Time;
using FsCheck;
using FsCheck.Fluent;
using Xunit;
using static Atlasiv.Core.Tests.Scheduling.RunHarness;

namespace Atlasiv.Core.Tests.Scheduling;

/// <summary>
/// Time-step consistency (spec section 13, PASS1_PLAN J6) and the propagator cache in a run (spec
/// clarification 18), with a dummy hazard and a dummy decay: a quantity Q decays by the survival
/// factor exp(-λΔt) over each pass, Q ← round(Q × factor).
/// </summary>
public sealed class TimeStepConsistencyTests
{
    [Fact]
    public void A_hazard_over_one_6_year_pass_equals_three_2_year_passes_exactly()
    {
        TimeGen.Check(Prop.ForAll(Arb.From(SixYearCases()), c =>
        {
            var (whole, parts) = Split(c.Start);
            var one = IntegratedHazard.None.Add(c.Rate, whole.Seconds);
            var three = parts.Aggregate(IntegratedHazard.None, (h, p) => h.Add(c.Rate, p.Seconds));
            return one == three && one.Probability == three.Probability;
        }));
    }

    [Fact]
    public void A_decay_over_one_6_year_pass_and_three_2_year_passes_differ_by_at_most_the_documented_bound()
    {
        // ENG-014: each factor is within 2 raw units (2^-31) of the true exp(-λΔt), and each multiply
        // rounds by at most half a unit, so the two results differ by at most ⌈8Q / 2^32⌉ + 3.
        var cache = new PropagatorCache<Q32, Q32>(new DecaySource());
        var largest = 0L;
        TimeGen.Check(Prop.ForAll(Arb.From(SixYearCases()), c =>
        {
            var (whole, parts) = Split(c.Start);
            var one = Rounding.MultiplyHalfToEven(c.Quantity, cache.For(c.Rate, whole.Seconds));
            var three = parts.Aggregate(c.Quantity, (q, p) => Rounding.MultiplyHalfToEven(q, cache.For(c.Rate, p.Seconds)));
            var difference = Math.Abs(one - three);
            largest = Math.Max(largest, difference);
            return difference <= DecayBound(c.Quantity);
        }));

        // The splits do differ, which is why clarification 18 never composes propagators.
        Assert.True(largest > 0);
    }

    [Fact]
    public void A_cold_cache_a_warm_cache_and_a_cache_cleared_every_run_give_identical_runs()
    {
        // A 5-year decay system on the 2-year step covers 6 years, then 4: neither is a legal length.
        var cold = RunDecay(new PropagatorCache<Q32, Q32>(new DecaySource()), clearEachRun: false);

        var warmCache = new PropagatorCache<Q32, Q32>(new DecaySource());
        RunDecay(warmCache, clearEachRun: false);
        var keptAfterFirstRun = warmCache.Count;
        var warm = RunDecay(warmCache, clearEachRun: false);

        var cleared = RunDecay(new PropagatorCache<Q32, Q32>(new DecaySource()), clearEachRun: true);

        Assert.Equal(keptAfterFirstRun, warmCache.Count);
        Assert.Equal(cold.Quantities, warm.Quantities);
        Assert.Equal(cold.Quantities, cleared.Quantities);
        Assert.Equal(cold.Runs, warm.Runs);
        Assert.Equal(cold.Runs, cleared.Runs);
        Assert.Equal(20, cold.Quantities.Count);
        Assert.Contains(cold.Runs, r => r.Covered.Seconds == Span(Date(-9999, 1, 1), Date(-9993, 1, 1)).Seconds);
    }

    private static long DecayBound(long quantity) =>
        (long)(((Int128)quantity * 8 + (1L << 32) - 1) >> 32) + 3;

    private static (TimeInterval Whole, TimeInterval[] Parts) Split(Instant start)
    {
        var date = start.Date;
        var at = (int years) => Instant.FromDate(new CalendarDate(date.Year + years, date.Month, 1));
        return (new TimeInterval(start, at(6)), [new(start, at(2)), new(at(2), at(4)), new(at(4), at(6))]);
    }

    private static Gen<(Instant Start, Q32 Rate, long Quantity)> SixYearCases() =>
        from year in Gen.Choose(-9999, 3000)
        from month in Gen.Choose(1, 12)
        from rateRaw in Gen.Choose(0, int.MaxValue)
        from rateScale in Gen.Choose(0, 2)
        from quantityHigh in Gen.Choose(0, 1 << 20)
        from quantityLow in Gen.Choose(0, int.MaxValue)
        select (
            Instant.FromDate(new CalendarDate(year, month, 1)),
            Q32.FromRaw((long)rateRaw << rateScale),                       // up to 2 per year
            ((long)quantityHigh << 30) + quantityLow);                     // up to about 2^50

    private static (List<long> Quantities, List<(long Turn, TimeInterval Covered)> Runs) RunDecay(
        PropagatorCache<Q32, Q32> cache, bool clearEachRun)
    {
        var rate = Q32.FromRatio(3, 100);
        var quantity = 8_000_000_000L;
        var quantities = new List<long>();
        var system = new ScriptedSystem(9, ResolutionStage.TurnEnd(2), NaturalPeriod.TurnOf(TurnStep.FiveYears), (_, covered) =>
        {
            if (clearEachRun)
            {
                cache.Clear();
            }

            quantity = Rounding.MultiplyHalfToEven(quantity, cache.For(rate, covered.Seconds));
            quantities.Add(quantity);
        });
        var timeline = Start(TurnStep.TwoYears, system).RunTurns(50);
        var runs = timeline.Entries.SelectMany(e => e.Runs.Select(r => (e.Turn, r.Covered))).ToList();
        return (quantities, runs);
    }

    /// <summary>The dummy decay: the survival factor exp(-λΔt), computed with the integer exp.</summary>
    private sealed class DecaySource : IPropagatorSource<Q32, Q32>
    {
        public Q32 Compute(Q32 regime, long elapsedSeconds) => IntegratedHazard.None.Add(regime, elapsedSeconds).Survival;
    }
}
