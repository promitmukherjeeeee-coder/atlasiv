using System;
using System.Linq;
using Atlasiv.Core.Integration;
using Atlasiv.Core.Tests.Time;
using Atlasiv.Core.Time;
using FsCheck;
using FsCheck.Fluent;
using Xunit;
using static Atlasiv.Core.Tests.Scheduling.RunHarness;

namespace Atlasiv.Core.Tests.Integration;

/// <summary>
/// The substep driver splits an interval into 1 to N calendar-exact substeps (spec section 10,
/// clarification 13): whole days, shared out as evenly as possible (owner ruling of 27 Nov 2026).
/// Expected dates are worked out by hand; year -9,999 (10,000 BCE) has 365 days and -9,996 has 366.
/// </summary>
public sealed class SubstepDriverTests
{
    [Fact]
    public void A_365_day_year_splits_into_4_substeps_of_91_91_91_and_92_days()
    {
        var year = Span(Date(-9999, 1, 1), Date(-9998, 1, 1));

        var substeps = SubstepDriver.Split(year, 4);

        Assert.Equal(
            [
                Span(Date(-9999, 1, 1), Date(-9999, 4, 2)),
                Span(Date(-9999, 4, 2), Date(-9999, 7, 2)),
                Span(Date(-9999, 7, 2), Date(-9999, 10, 1)),
                Span(Date(-9999, 10, 1), Date(-9998, 1, 1)),
            ],
            substeps);
    }

    [Fact]
    public void A_366_day_year_splits_into_4_substeps_of_91_92_91_and_92_days()
    {
        var leapYear = Span(Date(-9996, 1, 1), Date(-9995, 1, 1));

        var substeps = SubstepDriver.Split(leapYear, 4);

        Assert.Equal(
            [
                Span(Date(-9996, 1, 1), Date(-9996, 4, 1)),
                Span(Date(-9996, 4, 1), Date(-9996, 7, 2)),
                Span(Date(-9996, 7, 2), Date(-9996, 10, 1)),
                Span(Date(-9996, 10, 1), Date(-9995, 1, 1)),
            ],
            substeps);
    }

    [Fact]
    public void One_substep_is_the_whole_interval()
    {
        var sixYears = Span(Date(-9999, 1, 1), Date(-9993, 1, 1));

        Assert.Equal([sixYears], SubstepDriver.Split(sixYears, 1));
    }

    [Fact]
    public void More_substeps_than_the_default_budget_are_allowed()
    {
        // Clarification 13: the driver accepts any N; 4 is only the default budget.
        var month = Span(Date(-9999, 1, 1), Date(-9999, 2, 1));

        var substeps = SubstepDriver.Split(month, 31);

        Assert.Equal(4, SubstepDriver.DefaultBudget);
        Assert.Equal(31, substeps.Count);
        Assert.All(substeps, s => Assert.Equal(Calendar.SecondsPerDay, s.Seconds));
    }

    [Fact]
    public void Substeps_tile_the_interval_on_midnights_and_differ_by_at_most_one_day()
    {
        var gen =
            from startDay in Gen.Choose(-1_000_000, 10_000_000)
            from days in Gen.Choose(1, 40_000)
            from count in Gen.Choose(1, Math.Min(days, 200))
            select (Start: new Instant(startDay * TimeGen.Day), Days: days, Count: count);

        TimeGen.Check(Prop.ForAll(Arb.From(gen), c =>
        {
            var interval = new TimeInterval(c.Start, c.Start.AddSeconds(c.Days * TimeGen.Day));
            var substeps = SubstepDriver.Split(interval, c.Count);

            var lengths = substeps.Select(s => s.Seconds / TimeGen.Day).ToList();
            return substeps.Count == c.Count
                && substeps[0].Start == interval.Start
                && substeps[^1].End == interval.End
                && substeps.Zip(substeps.Skip(1)).All(p => p.First.End == p.Second.Start)
                && substeps.All(s => s.Start.SecondOfDay == 0 && s.Seconds % TimeGen.Day == 0 && s.Seconds > 0)
                && lengths.Max() - lengths.Min() <= 1;
        }));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(32)]
    public void A_count_below_1_or_above_the_number_of_days_is_refused(int count)
    {
        var month = Span(Date(-9999, 1, 1), Date(-9999, 2, 1));

        Assert.Throws<ArgumentOutOfRangeException>(() => SubstepDriver.Split(month, count));
    }

    [Fact]
    public void An_interval_that_does_not_start_and_end_at_midnight_is_refused()
    {
        var offMidnight = Span(Date(-9999, 1, 1).AddSeconds(3_600), Date(-9999, 2, 1));

        Assert.Throws<ArgumentException>(() => SubstepDriver.Split(offMidnight, 2));
    }
}
