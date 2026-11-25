using System;
using Atlasiv.Core.Numerics;
using Atlasiv.Core.Time;
using Xunit;

namespace Atlasiv.Core.Tests.Time;

/// <summary>
/// The candidate step (Pass 1 spec section 4, clarification 7): the step whose nominal length
/// is closest on a log scale to half the leading edge's strategic time, decided by exact
/// integer comparison. Boundary values were computed independently in Python as
/// isqrt(4 × L_slower × L_faster), with nominal lengths in whole seconds (months × 2,629,746).
/// </summary>
public sealed class CandidateStepTests
{
    [Theory]
    [InlineData(TurnStep.FiftyYears, 1_577_847_600L)]
    [InlineData(TurnStep.TwentyYears, 631_139_040L)]
    [InlineData(TurnStep.TenYears, 315_569_520L)]
    [InlineData(TurnStep.FiveYears, 157_784_760L)]
    [InlineData(TurnStep.TwoYears, 63_113_904L)]
    [InlineData(TurnStep.OneYear, 31_556_952L)]
    [InlineData(TurnStep.SixMonths, 15_778_476L)]
    [InlineData(TurnStep.OneMonth, 2_629_746L)]
    public void Nominal_lengths_are_whole_months_of_a_twelfth_of_the_rate_year(TurnStep step, long seconds) =>
        Assert.Equal(seconds, StepTable.NominalSeconds(step));

    [Theory]
    [InlineData(TurnStep.FiftyYears)]
    [InlineData(TurnStep.TwentyYears)]
    [InlineData(TurnStep.TenYears)]
    [InlineData(TurnStep.FiveYears)]
    [InlineData(TurnStep.TwoYears)]
    [InlineData(TurnStep.OneYear)]
    [InlineData(TurnStep.SixMonths)]
    [InlineData(TurnStep.OneMonth)]
    public void A_strategic_time_of_exactly_two_turns_picks_that_step(TurnStep step) =>
        Assert.Equal(step, CandidateStep.For(2 * StepTable.NominalSeconds(step)));

    // Last strategic time that still picks the faster neighbour; one second more picks the slower.
    [Theory]
    [InlineData(TurnStep.FiftyYears, TurnStep.TwentyYears, 1_995_836_886L)]
    [InlineData(TurnStep.TwentyYears, TurnStep.TenYears, 892_565_390L)]
    [InlineData(TurnStep.TenYears, TurnStep.FiveYears, 446_282_695L)]
    [InlineData(TurnStep.FiveYears, TurnStep.TwoYears, 199_583_688L)]
    [InlineData(TurnStep.TwoYears, TurnStep.OneYear, 89_256_539L)]
    [InlineData(TurnStep.OneYear, TurnStep.SixMonths, 44_628_269L)]
    [InlineData(TurnStep.SixMonths, TurnStep.OneMonth, 12_883_071L)]
    public void The_switch_point_between_neighbours_is_their_geometric_mean(TurnStep slower, TurnStep faster, long lastFaster)
    {
        Assert.Equal(faster, CandidateStep.For(lastFaster));
        Assert.Equal(slower, CandidateStep.For(lastFaster + 1));
    }

    // The squared strategic time S² is compared with 4 × L_slower × L_faster (that is, (S / 2)²
    // with L_slower × L_faster), so an exact tie is S² = 4 × L_slower × L_faster.
    [Theory]
    [InlineData(TurnStep.FiftyYears, TurnStep.TwentyYears)]
    [InlineData(TurnStep.TwentyYears, TurnStep.TenYears)]
    [InlineData(TurnStep.TenYears, TurnStep.FiveYears)]
    [InlineData(TurnStep.FiveYears, TurnStep.TwoYears)]
    [InlineData(TurnStep.TwoYears, TurnStep.OneYear)]
    [InlineData(TurnStep.OneYear, TurnStep.SixMonths)]
    [InlineData(TurnStep.SixMonths, TurnStep.OneMonth)]
    public void An_exact_tie_goes_to_the_slower_step(TurnStep slower, TurnStep faster)
    {
        var tie = 4 * (Int128)StepTable.NominalSeconds(slower) * StepTable.NominalSeconds(faster);
        Assert.Equal(slower, CandidateStep.ForSquaredStrategicTime(tie));
        Assert.Equal(faster, CandidateStep.ForSquaredStrategicTime(tie - 1));
    }

    // No two neighbours' nominal lengths multiply to a perfect square (their month counts multiply
    // to 144,000, 28,800, 7,200, 1,440, 288, 72 and 6), so a tie has no whole-second strategic time:
    // the last faster value r and the first slower value r + 1 straddle it strictly. This is why the
    // tie rule above can only be reached through the squared strategic time.
    [Theory]
    [InlineData(TurnStep.FiftyYears, TurnStep.TwentyYears, 1_995_836_886L)]
    [InlineData(TurnStep.TwentyYears, TurnStep.TenYears, 892_565_390L)]
    [InlineData(TurnStep.TenYears, TurnStep.FiveYears, 446_282_695L)]
    [InlineData(TurnStep.FiveYears, TurnStep.TwoYears, 199_583_688L)]
    [InlineData(TurnStep.TwoYears, TurnStep.OneYear, 89_256_539L)]
    [InlineData(TurnStep.OneYear, TurnStep.SixMonths, 44_628_269L)]
    [InlineData(TurnStep.SixMonths, TurnStep.OneMonth, 12_883_071L)]
    public void An_exact_tie_cannot_arise_from_whole_seconds(TurnStep slower, TurnStep faster, long lastFaster)
    {
        var tie = 4 * (Int128)StepTable.NominalSeconds(slower) * StepTable.NominalSeconds(faster);
        Assert.True((Int128)lastFaster * lastFaster < tie);
        Assert.True((Int128)(lastFaster + 1) * (lastFaster + 1) > tie);
    }

    [Theory]
    [InlineData(long.MaxValue, TurnStep.FiftyYears)]
    [InlineData(100L * 1_577_847_600L, TurnStep.FiftyYears)]
    [InlineData(1L, TurnStep.OneMonth)]
    [InlineData(0L, TurnStep.OneMonth)]
    public void Targets_beyond_the_table_clamp_to_its_ends(long strategicSeconds, TurnStep expected) =>
        Assert.Equal(expected, CandidateStep.For(strategicSeconds));

    [Fact]
    public void A_negative_strategic_time_is_a_simulation_error() =>
        Assert.Throws<SimulationDomainException>(() => CandidateStep.For(-1));
}
