using System;
using System.Collections.Generic;
using System.Linq;
using Atlasiv.Core.Integration;
using Atlasiv.Core.Numerics;
using Atlasiv.Core.Time;
using Xunit;
using static Atlasiv.Core.Tests.Scheduling.RunHarness;

namespace Atlasiv.Core.Tests.Integration;

/// <summary>
/// The propagator cache, keyed by exact elapsed seconds and regime (spec section 10, clarification
/// 18): every legal turn and phase length is computed in advance, any other length on first use, and
/// each is computed once and kept.
/// </summary>
public sealed class PropagatorCacheTests
{
    private const long Day = Calendar.SecondsPerDay;

    // Six calendar years from the epoch: 2,191 days (one leap day, in 9,997 BCE). No legal length.
    private static readonly long SixYears = Span(Date(-9999, 1, 1), Date(-9993, 1, 1)).Seconds;

    [Theory]
    [InlineData(28)]
    [InlineData(29)]
    [InlineData(30)]
    [InlineData(31)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(181)]
    [InlineData(182)]
    [InlineData(184)]
    [InlineData(365)]
    [InlineData(366)]
    [InlineData(18_262)]
    [InlineData(18_263)]
    public void The_legal_lengths_include_hand_worked_turn_and_phase_lengths(long days)
    {
        // 1-month turns 28 to 31 days; its phases 6 days, the last 4 to 7; 6-month turns 181 or 182
        // days from 1 January and 184 from 1 July; 1-year turns 365 or 366; 50-year turns 18,262 or 18,263.
        Assert.Contains(days * Day, LegalLengths.Seconds);
    }

    [Fact]
    public void Every_turn_and_phase_the_calendar_produces_over_800_years_has_a_legal_length()
    {
        var legal = LegalLengths.Seconds.ToHashSet();
        var until = Date(-9199, 1, 1);
        foreach (var step in StepTable.All)
        {
            for (var turn = TurnDates.Starting(step, Instant.Epoch); turn.Start < until; turn = turn.Next)
            {
                Assert.Contains(turn.Span.Seconds, legal);
                for (var phase = 1; phase <= turn.PhaseCount; phase++)
                {
                    Assert.Contains(turn.Phase(phase).Seconds, legal);
                }
            }
        }
    }

    [Fact]
    public void The_legal_lengths_are_ascending_and_distinct_and_exclude_6_years()
    {
        var lengths = LegalLengths.Seconds;

        Assert.True(lengths.Zip(lengths.Skip(1)).All(p => p.First < p.Second));
        Assert.DoesNotContain(SixYears, lengths);
    }

    [Fact]
    public void A_regimes_first_use_computes_every_legal_length_once_in_advance()
    {
        var source = new CountingSource();
        var cache = new PropagatorCache<int, long>(source);

        cache.For(1, 365 * Day);
        foreach (var length in LegalLengths.Seconds)
        {
            cache.For(1, length);
        }

        Assert.Equal(LegalLengths.Seconds.Select(s => (1, s)), source.Calls);
    }

    [Fact]
    public void Another_length_is_computed_on_first_use_and_then_kept()
    {
        var source = new CountingSource();
        var cache = new PropagatorCache<int, long>(source);
        cache.For(1, 365 * Day);
        var before = source.Calls.Count;

        var first = cache.For(1, SixYears);
        var again = cache.For(1, SixYears);

        Assert.Equal([(1, SixYears)], source.Calls.Skip(before));
        Assert.Equal(CountingSource.Value(1, SixYears), first);
        Assert.Equal(first, again);
    }

    [Fact]
    public void Regimes_are_kept_apart()
    {
        var cache = new PropagatorCache<int, long>(new CountingSource());

        Assert.Equal(CountingSource.Value(1, SixYears), cache.For(1, SixYears));
        Assert.Equal(CountingSource.Value(2, SixYears), cache.For(2, SixYears));
    }

    [Fact]
    public void A_cold_cache_a_warm_cache_and_a_cleared_cache_return_the_same_values()
    {
        var lengths = new[] { SixYears, 365 * Day, 2_922 * Day, 1 * Day, 0 };
        var warm = new PropagatorCache<int, long>(new CountingSource());
        foreach (var length in lengths)
        {
            warm.For(3, length);
        }

        var cleared = new PropagatorCache<int, long>(new CountingSource());
        cleared.For(3, SixYears);
        cleared.Clear();

        foreach (var length in lengths)
        {
            var cold = new PropagatorCache<int, long>(new CountingSource()).For(3, length);
            Assert.Equal(cold, warm.For(3, length));
            Assert.Equal(cold, cleared.For(3, length));
        }

        Assert.Equal(0, new PropagatorCache<int, long>(new CountingSource()).Count);
    }

    [Fact]
    public void A_negative_length_is_refused()
    {
        var cache = new PropagatorCache<int, long>(new CountingSource());

        Assert.Throws<SimulationDomainException>(() => cache.For(1, -1));
    }

    /// <summary>Records every computation; the "propagator" is a value made from its inputs.</summary>
    private sealed class CountingSource : IPropagatorSource<int, long>
    {
        public List<(int Regime, long Seconds)> Calls { get; } = [];

        public static long Value(int regime, long seconds) => (regime * 1_000_000_007L) + seconds;

        public long Compute(int regime, long elapsedSeconds)
        {
            Calls.Add((regime, elapsedSeconds));
            return Value(regime, elapsedSeconds);
        }
    }
}
