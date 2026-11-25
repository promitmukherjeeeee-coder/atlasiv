using System;
using System.Linq;
using Atlasiv.Core.Time;
using Xunit;

namespace Atlasiv.Core.Tests.Time;

/// <summary>The eight steps and their phase counts (Pass 1 spec section 3).</summary>
public sealed class StepTableTests
{
    [Theory]
    [InlineData(TurnStep.FiftyYears, 600, 20)]
    [InlineData(TurnStep.TwentyYears, 240, 10)]
    [InlineData(TurnStep.TenYears, 120, 10)]
    [InlineData(TurnStep.FiveYears, 60, 5)]
    [InlineData(TurnStep.TwoYears, 24, 4)]
    [InlineData(TurnStep.OneYear, 12, 4)]
    [InlineData(TurnStep.SixMonths, 6, 3)]
    [InlineData(TurnStep.OneMonth, 1, 5)]
    public void Each_step_has_its_length_and_phase_count(TurnStep step, int months, int phases)
    {
        Assert.Equal(months, StepTable.MonthsPerTurn(step));
        Assert.Equal(phases, StepTable.PhaseCount(step));
    }

    [Fact]
    public void There_are_eight_steps_from_slowest_to_fastest()
    {
        Assert.Equal(
            [TurnStep.FiftyYears, TurnStep.TwentyYears, TurnStep.TenYears, TurnStep.FiveYears, TurnStep.TwoYears, TurnStep.OneYear, TurnStep.SixMonths, TurnStep.OneMonth],
            StepTable.All);
        Assert.True(StepTable.All.Zip(StepTable.All.Skip(1)).All(p => StepTable.MonthsPerTurn(p.First) > StepTable.MonthsPerTurn(p.Second)));
    }

    [Theory]
    [InlineData(TurnStep.FiftyYears, 30)]
    [InlineData(TurnStep.TwentyYears, 24)]
    [InlineData(TurnStep.TenYears, 12)]
    [InlineData(TurnStep.FiveYears, 12)]
    [InlineData(TurnStep.TwoYears, 6)]
    [InlineData(TurnStep.OneYear, 3)]
    [InlineData(TurnStep.SixMonths, 2)]
    [InlineData(TurnStep.OneMonth, 0)]   // the 1-month step uses day numbers instead
    public void Phases_are_whole_months_except_on_the_1_month_step(TurnStep step, int monthsPerPhase)
    {
        Assert.Equal(monthsPerPhase, StepTable.MonthsPerPhase(step));
    }

    [Fact]
    public void Unknown_steps_are_refused()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => StepTable.PhaseCount((TurnStep)8));
        Assert.Throws<ArgumentOutOfRangeException>(() => StepTable.MonthsPerTurn((TurnStep)(-1)));
    }
}
