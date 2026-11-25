using System;
using System.Linq;
using Atlasiv.Core.Numerics;
using Atlasiv.Core.Randomness;
using Xunit;

namespace Atlasiv.Core.Tests.Randomness;

/// <summary>
/// The draw helpers on <see cref="RandomDraws"/>: uniform integers, fixed-point uniforms,
/// Bernoulli and Poisson draws, stochastic rounding and time-to-event draws (spec sections
/// 9 and 10). Statistical checks use fixed seeds, so each run is identical, and bounds of
/// about five standard deviations, so a correct generator cannot fail them by chance.
/// </summary>
public sealed class RandomDrawsTests
{
    private static DrawAddress Key(ulong entity = 1) =>
        new(0xA71A_5100_0000_0002UL, 5, DrawSlot.Phase(2), 0, 3, entity);

    // ---- Raw draws ----

    [Fact]
    public void A_stream_reads_its_address_draws_in_order()
    {
        var key = Key();
        var draws = new RandomDraws(key);

        var drawn = Enumerable.Range(0, 9).Select(_ => draws.NextUInt64()).ToArray();

        Assert.Equal(Enumerable.Range(0, 9).Select(i => key.Draw((ulong)i)), drawn);
        Assert.Equal(9UL, draws.NextDrawIndex);
    }

    [Fact]
    public void A_stream_can_start_at_a_later_draw_index()
    {
        var key = Key();
        var draws = new RandomDraws(key, firstDrawIndex: 100);

        Assert.Equal(key.Draw(100), draws.NextUInt64());
        Assert.Equal(101UL, draws.NextDrawIndex);
    }

    // ---- Uniform integers ----

    [Theory]
    [InlineData(1UL)]
    [InlineData(2UL)]
    [InlineData(3UL)]
    [InlineData(1_000UL)]
    [InlineData((1UL << 63) + 1)]
    [InlineData(ulong.MaxValue)]
    public void A_uniform_integer_is_below_its_bound(ulong bound)
    {
        var draws = new RandomDraws(Key());
        for (var i = 0; i < 2_000; i++)
        {
            Assert.True(draws.NextBelow(bound) < bound);
        }
    }

    [Fact]
    public void Uniform_integers_are_fair_on_a_die()
    {
        const int Rolls = 60_000;
        var draws = new RandomDraws(Key());
        var counts = new int[6];
        for (var i = 0; i < Rolls; i++)
        {
            counts[draws.NextBelow(6)]++;
        }

        // Chi-squared with 5 degrees of freedom: 25 is far beyond its 0.9999 quantile (about 25.7 is 0.0001).
        var expected = Rolls / 6.0;
        var chiSquared = counts.Sum(c => (c - expected) * (c - expected) / expected);
        Assert.True(chiSquared < 25, $"chi-squared {chiSquared:F2}, counts {string.Join(", ", counts)}");
    }

    [Fact]
    public void Uniform_integers_have_no_modulo_bias_for_large_bounds()
    {
        // With bound 3 * 2^62, taking the draw modulo the bound would put half the results
        // below 2^62 instead of a third.
        const int Samples = 30_000;
        var bound = 3UL << 62;
        var draws = new RandomDraws(Key());
        var low = Enumerable.Range(0, Samples).Count(_ => draws.NextBelow(bound) < (1UL << 62));

        Assert.InRange(low / (double)Samples, 1.0 / 3 - 0.014, 1.0 / 3 + 0.014); // about 5 standard deviations
    }

    [Fact]
    public void A_uniform_integer_needs_a_positive_bound()
    {
        Assert.Throws<SimulationDomainException>(() => new RandomDraws(Key()).NextBelow(0));
    }

    [Fact]
    public void An_inclusive_range_reaches_both_ends()
    {
        var draws = new RandomDraws(Key());
        var seen = Enumerable.Range(0, 2_000).Select(_ => draws.NextInRange(-2, 2)).Distinct().Order();

        Assert.Equal(new long[] { -2, -1, 0, 1, 2 }, seen);
    }

    [Fact]
    public void The_full_64_bit_range_is_allowed()
    {
        var draws = new RandomDraws(Key());
        var values = Enumerable.Range(0, 1_000).Select(_ => draws.NextInRange(long.MinValue, long.MaxValue)).ToArray();

        Assert.Contains(values, v => v < 0);
        Assert.Contains(values, v => v > 0);
    }

    [Fact]
    public void A_single_value_range_returns_that_value()
    {
        Assert.Equal(long.MaxValue, new RandomDraws(Key()).NextInRange(long.MaxValue, long.MaxValue));
    }

    [Fact]
    public void An_empty_range_is_refused()
    {
        Assert.Throws<SimulationDomainException>(() => new RandomDraws(Key()).NextInRange(1, 0));
    }

    // ---- Fixed-point uniform, Bernoulli and stochastic rounding ----

    [Fact]
    public void A_fixed_point_uniform_lies_in_the_unit_interval_and_averages_one_half()
    {
        const int Samples = 40_000;
        var draws = new RandomDraws(Key());
        var values = Enumerable.Range(0, Samples).Select(_ => draws.NextUnit()).ToArray();

        Assert.All(values, v => Assert.InRange(v.Raw, 0L, (1L << 32) - 1));
        var mean = values.Average(v => v.Raw / 4_294_967_296.0);
        Assert.InRange(mean, 0.5 - 0.0073, 0.5 + 0.0073); // 5 standard deviations of 1/sqrt(12 n)
    }

    [Fact]
    public void A_fixed_point_uniform_uses_the_top_32_bits_of_one_draw()
    {
        var key = Key();
        var draws = new RandomDraws(key);

        Assert.Equal((long)(key.Draw(0) >> 32), draws.NextUnit().Raw);
        Assert.Equal(1UL, draws.NextDrawIndex);
    }

    [Fact]
    public void Bernoulli_draws_with_probability_zero_or_one_are_certain()
    {
        var draws = new RandomDraws(Key());
        for (var i = 0; i < 1_000; i++)
        {
            Assert.False(draws.NextBernoulli(Q32.Zero));
            Assert.True(draws.NextBernoulli(Q32.One));
        }
    }

    [Theory]
    [InlineData(1, 1_000)]
    [InlineData(1, 4)]
    [InlineData(1, 2)]
    [InlineData(9, 10)]
    public void Bernoulli_draws_succeed_at_their_probability(long numerator, long denominator)
    {
        const int Samples = 50_000;
        var p = numerator / (double)denominator;
        var draws = new RandomDraws(Key());
        var probability = Q32.FromRatio(numerator, denominator);
        var hits = Enumerable.Range(0, Samples).Count(_ => draws.NextBernoulli(probability));

        var tolerance = 5 * Math.Sqrt(p * (1 - p) / Samples);
        Assert.InRange(hits / (double)Samples, p - tolerance, p + tolerance);
    }

    [Theory]
    [InlineData(-1L)]
    [InlineData((1L << 32) + 1)]
    public void A_probability_outside_zero_to_one_is_refused(long raw)
    {
        Assert.Throws<SimulationDomainException>(() => new RandomDraws(Key()).NextBernoulli(Q32.FromRaw(raw)));
    }

    [Fact]
    public void Stochastic_rounding_goes_to_a_neighbour_and_keeps_the_mean()
    {
        const int Samples = 40_000;
        var value = Q32.FromRatio(9, 4); // 2.25
        var draws = new RandomDraws(Key());
        var results = Enumerable.Range(0, Samples).Select(_ => draws.RoundStochastic(value)).ToArray();

        Assert.All(results, r => Assert.InRange(r, 2L, 3L));
        Assert.InRange(results.Average(), 2.25 - 0.011, 2.25 + 0.011); // 5 sd of sqrt(0.25 * 0.75 / n)
    }

    [Fact]
    public void Stochastic_rounding_leaves_whole_numbers_alone()
    {
        var draws = new RandomDraws(Key());
        for (var i = 0; i < 1_000; i++)
        {
            Assert.Equal(-7L, draws.RoundStochastic(Q32.FromInt(-7)));
        }
    }

    [Fact]
    public void Stochastic_rounding_of_a_product_keeps_the_expected_count()
    {
        // 1,000 people at a 0.15% hazard: 1.5 deaths expected, so 1 or 2 each time.
        const int Samples = 40_000;
        var rate = Q32.FromRatio(15, 10_000);
        var draws = new RandomDraws(Key());
        var results = Enumerable.Range(0, Samples).Select(_ => draws.MultiplyStochastic(1_000, rate)).ToArray();

        Assert.All(results, r => Assert.InRange(r, 1L, 2L));
        Assert.InRange(results.Average(), 1.5 - 0.0125, 1.5 + 0.0125); // 5 sd
    }

    [Fact]
    public void Stochastic_rounding_uses_one_draw_through_the_J1_hook()
    {
        var key = Key();
        var draws = new RandomDraws(key);
        var value = Q32.FromRaw((5L << 32) + 123_456_789);

        Assert.Equal(Rounding.Stochastic(value, key.Draw(0)), draws.RoundStochastic(value));
        Assert.Equal(Rounding.MultiplyStochastic(77, value, key.Draw(1)), draws.MultiplyStochastic(77, value));
        Assert.Equal(2UL, draws.NextDrawIndex);
    }

    // ---- Poisson ----

    [Theory]
    [InlineData(1, 1_024)]   // rare: about 0.001 events
    [InlineData(1, 2)]
    [InlineData(3, 1)]
    [InlineData(16, 1)]      // the largest single-draw mean
    [InlineData(41, 1)]      // three pieces: 16 + 16 + 9
    [InlineData(250, 1)]
    public void Poisson_counts_have_the_right_mean_and_variance(long numerator, long denominator)
    {
        const int Samples = 40_000;
        var mean = numerator / (double)denominator;
        var draws = new RandomDraws(Key());
        var q = Q32.FromRatio(numerator, denominator);
        var counts = Enumerable.Range(0, Samples).Select(_ => draws.NextPoisson(q)).ToArray();

        var sampleMean = counts.Average();
        var sampleVariance = counts.Sum(c => (c - sampleMean) * (c - sampleMean)) / (Samples - 1);

        // Standard errors: sqrt(mean / n) for the mean, about sqrt((mean + 2 mean^2) / n) for the variance.
        var meanTolerance = 5 * Math.Sqrt(mean / Samples);
        var varianceTolerance = 5 * Math.Sqrt((mean + (2 * mean * mean)) / Samples);
        Assert.InRange(sampleMean, mean - meanTolerance, mean + meanTolerance);
        Assert.InRange(sampleVariance, mean - varianceTolerance, mean + varianceTolerance);
    }

    [Fact]
    public void Poisson_counts_follow_the_Poisson_distribution()
    {
        // Mean 2.5, counts 0 to 7 and "8 or more", against the exact probabilities.
        const int Samples = 60_000;
        const double Mean = 2.5;
        var draws = new RandomDraws(Key());
        var observed = new int[9];
        for (var i = 0; i < Samples; i++)
        {
            observed[Math.Min(draws.NextPoisson(Q32.FromRatio(5, 2)), 8)]++;
        }

        var chiSquared = 0.0;
        var tail = 1.0;
        for (var k = 0; k <= 8; k++)
        {
            var p = k < 8 ? Math.Exp(-Mean) * Math.Pow(Mean, k) / Enumerable.Range(1, k).Aggregate(1.0, (f, j) => f * j) : tail;
            tail -= p;
            var expected = p * Samples;
            chiSquared += (observed[k] - expected) * (observed[k] - expected) / expected;
        }

        // 8 degrees of freedom: 0.9999 quantile is about 31.8.
        Assert.True(chiSquared < 31.8, $"chi-squared {chiSquared:F2}, counts {string.Join(", ", observed)}");
    }

    [Fact]
    public void A_Poisson_count_with_mean_zero_is_zero_and_uses_no_draw()
    {
        var draws = new RandomDraws(Key());

        Assert.Equal(0L, draws.NextPoisson(Q32.Zero));
        Assert.Equal(0UL, draws.NextDrawIndex);
    }

    [Theory]
    [InlineData(1, 1UL)]
    [InlineData(16, 1UL)]
    [InlineData(17, 2UL)]
    [InlineData(250, 16UL)]
    public void A_Poisson_count_uses_one_draw_per_piece_of_mean_up_to_16(long mean, ulong expectedDraws)
    {
        var draws = new RandomDraws(Key());
        draws.NextPoisson(Q32.FromInt(mean));

        Assert.Equal(expectedDraws, draws.NextDrawIndex);
    }

    [Fact]
    public void A_negative_Poisson_mean_is_refused()
    {
        Assert.Throws<SimulationDomainException>(() => new RandomDraws(Key()).NextPoisson(-Q32.Epsilon));
    }

    // ---- Time to event ----

    [Fact]
    public void A_unit_exponential_has_mean_and_variance_one()
    {
        const int Samples = 40_000;
        var draws = new RandomDraws(Key());
        var values = Enumerable.Range(0, Samples).Select(_ => draws.NextUnitExponential().Raw / 4_294_967_296.0).ToArray();

        var mean = values.Average();
        var variance = values.Sum(v => (v - mean) * (v - mean)) / (Samples - 1);
        Assert.All(values, v => Assert.InRange(v, 0.0, 22.2)); // -ln(2^-32) is about 22.18
        Assert.InRange(mean, 1 - 0.025, 1 + 0.025);              // 5 sd of 1 / sqrt(n)
        Assert.InRange(variance, 1 - 0.075, 1 + 0.075);          // 5 sd of sqrt(8 / n)
    }

    [Fact]
    public void Times_to_event_agree_with_the_hazard_primitive()
    {
        // An event at 0.7 per year falls within one rate year with probability 1 - exp(-0.7)
        // (J1's hazard primitive), and has a mean waiting time of 1 / 0.7 rate years.
        const int Samples = 40_000;
        var rate = Q32.FromRatio(7, 10);
        var draws = new RandomDraws(Key());
        var times = Enumerable.Range(0, Samples).Select(_ => draws.NextTimeToEventSeconds(rate)!.Value).ToArray();

        var p = Hazard.Probability(rate, RateYear.Seconds).Raw / 4_294_967_296.0;
        var within = times.Count(t => t < RateYear.Seconds) / (double)Samples;
        Assert.InRange(within, p - (5 * Math.Sqrt(p * (1 - p) / Samples)), p + (5 * Math.Sqrt(p * (1 - p) / Samples)));

        var meanYears = times.Average() / RateYear.Seconds;
        Assert.InRange(meanYears, (1 / 0.7) * (1 - 0.025), (1 / 0.7) * (1 + 0.025));
        Assert.All(times, t => Assert.True(t >= 0));
    }

    [Fact]
    public void A_zero_rate_never_happens_but_still_uses_its_draw()
    {
        // Using the draw either way keeps later draws in the stream where they were.
        var draws = new RandomDraws(Key());

        Assert.Null(draws.NextTimeToEventSeconds(Q32.Zero));
        Assert.Equal(1UL, draws.NextDrawIndex);
    }

    [Fact]
    public void The_smallest_rate_gives_a_finite_time_without_overflow()
    {
        var draws = new RandomDraws(Key());
        for (var i = 0; i < 1_000; i++)
        {
            Assert.True(draws.NextTimeToEventSeconds(Q32.Epsilon) >= 0);
        }
    }

    [Fact]
    public void A_negative_rate_is_refused()
    {
        Assert.Throws<SimulationDomainException>(() => new RandomDraws(Key()).NextTimeToEventSeconds(-Q32.Epsilon));
    }

    // ---- Published helper vectors ----

    [Fact]
    public void A_fixed_sequence_of_helper_draws_matches_the_published_values()
    {
        // Expected values computed independently in Python with exact fractions and
        // 80-digit decimals (ENG-009). Any change here changes replay hashes.
        var draws = new RandomDraws(Key());

        Assert.Equal(3UL, draws.NextBelow(6));
        Assert.Equal(5_514_518_108_279_141_684UL, draws.NextBelow(3UL << 62));
        Assert.Equal(-245L, draws.NextInRange(-1_000, 1_000));
        Assert.Equal(4_257_458_963L, draws.NextUnit().Raw);
        Assert.True(draws.NextBernoulli(Q32.FromRatio(3, 10)));
        Assert.Equal(2L, draws.RoundStochastic(Q32.FromRatio(9, 4)));
        Assert.Equal(2L, draws.MultiplyStochastic(1_000, Q32.FromRatio(15, 10_000)));
        Assert.Equal(3L, draws.NextPoisson(Q32.FromRatio(5, 2)));
        Assert.Equal(35L, draws.NextPoisson(Q32.FromInt(41)));
        Assert.Equal(2_258_348_940L, draws.NextUnitExponential().Raw);
        Assert.Equal(23_237_557L, draws.NextTimeToEventSeconds(Q32.FromRatio(7, 10)));
        Assert.Equal(13UL, draws.NextDrawIndex);
    }
}
