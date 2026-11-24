using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Atlasiv.Core.Numerics;
using FsCheck;
using FsCheck.Fluent;
using Xunit;

namespace Atlasiv.Core.Tests.Numerics;

/// <summary>
/// The hazard primitive: probability within an interval = 1 - exp(-λΔt), with λ a per-year
/// rate and Δt exact elapsed seconds (one rate year = 31,556,952 s, ADR-039 ruling 10).
/// Varying rates are integrated exactly as the sum of λ·Δt over the pieces.
/// Vectors are true values rounded half to even, computed with 70-digit decimal arithmetic.
/// </summary>
public sealed class HazardTests
{
    private const long One = 1L << 32;
    private const long Year = 31_556_952;
    private const long Day = 86_400;

    [Fact]
    public void A_rate_year_is_31_556_952_seconds()
    {
        Assert.Equal(Year, RateYear.Seconds);
    }

    [Theory]
    [InlineData(One, Year, 2_714_937_127L)]             // 1 per year for 1 year: 1 - e^-1
    [InlineData(One / 2, Year / 2, 950_043_403L)]       // 0.5 per year for half a year
    [InlineData(85_899_346L, Year, 85_046_051L)]        // 0.02 per year for 1 year
    [InlineData(85_899_346L, 10 * Year, 778_545_488L)]  // 0.02 per year for 10 years
    [InlineData(4_294_967L, 2_629_746L, 357_899L)]      // 0.001 per year for a twelfth of a year
    [InlineData(One, 1L, 136L)]                         // 1 per year for 1 second
    [InlineData(One, 0L, 0L)]                           // no time, no chance
    [InlineData(0L, Year, 0L)]                          // no rate, no chance
    [InlineData(100 * One, 50 * Year, One)]             // certain
    public void Probability_vectors(long rateRaw, long seconds, long expected)
    {
        Assert.Equal(expected, Hazard.Probability(Q32.FromRaw(rateRaw), seconds).Raw);
    }

    [Fact]
    public void Survival_is_exp_of_minus_the_integrated_hazard()
    {
        var hazard = IntegratedHazard.None.Add(Q32.One, Year);
        Assert.Equal(One, hazard.Value.Raw);
        Assert.Equal(1_580_030_169L, hazard.Survival.Raw);   // e^-1
        Assert.Equal(2_714_937_127L, hazard.Probability.Raw); // 1 - e^-1
    }

    [Fact]
    public void A_varying_rate_integrates_piece_by_piece()
    {
        // 1 per year for half a year, then 3 per year for half a year: hazard 2.
        var hazard = IntegratedHazard.None
            .Add(Q32.One, Year / 2)
            .Add(Q32.FromInt(3), Year / 2);
        Assert.Equal(2 * One, hazard.Value.Raw);
        Assert.Equal(581_260_615L, hazard.Survival.Raw);     // e^-2
        Assert.Equal(3_713_706_681L, hazard.Probability.Raw); // 1 - e^-2
    }

    [Fact]
    public void A_huge_hazard_is_certain_even_when_its_value_does_not_fit()
    {
        var hazard = IntegratedHazard.None.Add(Q32.FromInt(1 << 30), 1000 * Year);
        Assert.Equal(Q32.One, hazard.Probability);
        Assert.Equal(Q32.Zero, hazard.Survival);
        Assert.Throws<SimulationOverflowException>(() => hazard.Value);
    }

    [Fact]
    public void Negative_rates_and_times_are_errors()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Hazard.Probability(-Q32.Epsilon, Year));
        Assert.Throws<ArgumentOutOfRangeException>(() => Hazard.Probability(Q32.One, -1));
    }

    /// <summary>
    /// Time-step consistency (spec section 13): a year advanced as twelve calendar months
    /// gives exactly the same hazard and probability as the year in one step.
    /// </summary>
    [Fact]
    public void Twelve_calendar_months_give_the_same_result_as_one_year()
    {
        var rate = Q32.FromRatio(3, 100);
        long[] monthDays = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
        var monthly = monthDays.Aggregate(IntegratedHazard.None, (h, days) => h.Add(rate, days * Day));
        var whole = IntegratedHazard.None.Add(rate, 365 * Day);

        Assert.Equal(whole.Value, monthly.Value);
        Assert.Equal(whole.Probability, monthly.Probability);
    }

    /// <summary>Any split of an interval, in any order, gives identical integer results.</summary>
    [Fact]
    public void Splitting_and_reordering_the_pieces_never_changes_the_result() =>
        NumericGen.Check(Prop.ForAll(Arb.From(Pieces), pieces =>
        {
            var inOrder = Integrate(pieces);
            var reversed = Integrate(pieces.AsEnumerable().Reverse());
            var halved = Integrate(pieces.SelectMany(p => new[] { (p.Rate, p.Seconds / 2), (p.Rate, p.Seconds - (p.Seconds / 2)) }));
            return inOrder.Value == reversed.Value
                && inOrder.Value == halved.Value
                && inOrder.Probability == halved.Probability;
        }));

    /// <summary>The probability is within one raw unit of the true 1 - exp(-H) for the rounded hazard H.</summary>
    [Fact]
    public void Probability_is_within_one_unit_of_the_true_value() =>
        NumericGen.Check(Prop.ForAll(Arb.From(Pieces), pieces =>
        {
            var hazard = Integrate(pieces);
            var h = hazard.Value.Raw;
            var expected = h >= 30 * One ? One : ReferenceMath.OneMinusExpNeg(h);
            var p = hazard.Probability.Raw;
            return BigInteger.Abs(p - expected) <= 1 && p >= 0 && p <= One;
        }));

    [Fact]
    public void Probability_never_decreases_as_time_passes() =>
        NumericGen.Check(Prop.ForAll(
            Arb.From(from rate in NumericGen.NonNegativeRaw(40)
                     from a in Gen.Choose(0, int.MaxValue)
                     from b in Gen.Choose(0, int.MaxValue)
                     select (rate, Short: (long)Math.Min(a, b), Long: (long)Math.Max(a, b))),
            t => Hazard.Probability(Q32.FromRaw(t.rate), t.Short) <= Hazard.Probability(Q32.FromRaw(t.rate), t.Long)));

    private static Gen<(Q32 Rate, long Seconds)[]> Pieces =>
        from count in Gen.Choose(1, 8)
        from pieces in Gen.ArrayOf(
            from rate in NumericGen.NonNegativeRaw(36)     // up to 16 per year
            from seconds in Gen.Choose(0, int.MaxValue)    // up to about 68 years each
            select (Q32.FromRaw(rate), (long)seconds),
            count)
        select pieces;

    private static IntegratedHazard Integrate(IEnumerable<(Q32 Rate, long Seconds)> pieces) =>
        pieces.Aggregate(IntegratedHazard.None, (h, p) => h.Add(p.Rate, p.Seconds));
}
