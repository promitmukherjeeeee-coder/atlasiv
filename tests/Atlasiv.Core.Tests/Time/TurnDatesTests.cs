using System;
using System.Globalization;
using System.Linq;
using Atlasiv.Core.Time;
using FsCheck;
using FsCheck.Fluent;
using Xunit;

namespace Atlasiv.Core.Tests.Time;

/// <summary>
/// The boundary calculator: each step's turn and phase dates (Pass 1 spec section 3).
/// Turns and phases are half-open [start, end), so a boundary instant belongs to the
/// phase that begins there. Expected dates are written out by hand from the spec's rules.
/// Dates below are astronomical year-month-day (year 0 = 1 BCE).
/// </summary>
public sealed class TurnDatesTests
{
    private const long Day = 86_400;

    public static TheoryData<TurnStep, string, string[]> PhaseDates => new()
    {
        // 50-year step: phase boundaries alternate 1 January and 1 July (2.5-year phases).
        {
            TurnStep.FiftyYears, "1900-01-01",
            [
                "1900-01-01", "1902-07-01", "1905-01-01", "1907-07-01", "1910-01-01",
                "1912-07-01", "1915-01-01", "1917-07-01", "1920-01-01", "1922-07-01",
                "1925-01-01", "1927-07-01", "1930-01-01", "1932-07-01", "1935-01-01",
                "1937-07-01", "1940-01-01", "1942-07-01", "1945-01-01", "1947-07-01",
                "1950-01-01",
            ]
        },
        {
            TurnStep.FiftyYears, "-9999-01-01",   // the first turn at the epoch
            [
                "-9999-01-01", "-9997-07-01", "-9994-01-01", "-9992-07-01", "-9989-01-01",
                "-9987-07-01", "-9984-01-01", "-9982-07-01", "-9979-01-01", "-9977-07-01",
                "-9974-01-01", "-9972-07-01", "-9969-01-01", "-9967-07-01", "-9964-01-01",
                "-9962-07-01", "-9959-01-01", "-9957-07-01", "-9954-01-01", "-9952-07-01",
                "-9949-01-01",
            ]
        },
        {
            TurnStep.TwentyYears, "1980-01-01",
            [
                "1980-01-01", "1982-01-01", "1984-01-01", "1986-01-01", "1988-01-01",
                "1990-01-01", "1992-01-01", "1994-01-01", "1996-01-01", "1998-01-01",
                "2000-01-01",
            ]
        },
        {
            TurnStep.TenYears, "1995-01-01",
            [
                "1995-01-01", "1996-01-01", "1997-01-01", "1998-01-01", "1999-01-01",
                "2000-01-01", "2001-01-01", "2002-01-01", "2003-01-01", "2004-01-01",
                "2005-01-01",
            ]
        },
        {
            TurnStep.FiveYears, "1898-01-01",   // crosses 1900, not a leap year
            ["1898-01-01", "1899-01-01", "1900-01-01", "1901-01-01", "1902-01-01", "1903-01-01"]
        },
        {
            TurnStep.FiveYears, "-2-01-01",     // 3 BCE to 3 CE, across 1 BCE (year 0, a leap year)
            ["-2-01-01", "-1-01-01", "0-01-01", "1-01-01", "2-01-01", "3-01-01"]
        },
        {
            TurnStep.TwoYears, "1999-01-01",
            ["1999-01-01", "1999-07-01", "2000-01-01", "2000-07-01", "2001-01-01"]
        },
        {
            TurnStep.OneYear, "2000-01-01",
            ["2000-01-01", "2000-04-01", "2000-07-01", "2000-10-01", "2001-01-01"]
        },
        {
            TurnStep.SixMonths, "2000-01-01",
            ["2000-01-01", "2000-03-01", "2000-05-01", "2000-07-01"]
        },
        {
            TurnStep.SixMonths, "1999-07-01",   // crosses the year end
            ["1999-07-01", "1999-09-01", "1999-11-01", "2000-01-01"]
        },
        {
            TurnStep.OneMonth, "2000-02-01",    // leap February: last phase 5 days
            ["2000-02-01", "2000-02-07", "2000-02-13", "2000-02-19", "2000-02-25", "2000-03-01"]
        },
        {
            TurnStep.OneMonth, "1900-02-01",    // February 1900, not leap: last phase 4 days
            ["1900-02-01", "1900-02-07", "1900-02-13", "1900-02-19", "1900-02-25", "1900-03-01"]
        },
        {
            TurnStep.OneMonth, "1999-12-01",    // crosses the year end: last phase 7 days
            ["1999-12-01", "1999-12-07", "1999-12-13", "1999-12-19", "1999-12-25", "2000-01-01"]
        },
        {
            TurnStep.OneMonth, "0-12-01",       // December 1 BCE runs into January 1 CE
            ["0-12-01", "0-12-07", "0-12-13", "0-12-19", "0-12-25", "1-01-01"]
        },
    };

    [Theory]
    [MemberData(nameof(PhaseDates))]
    public void Every_step_produces_its_phase_dates(TurnStep step, string start, string[] boundaries)
    {
        var turn = TurnDates.Starting(step, At(start));

        Assert.Equal(boundaries.Length - 1, turn.PhaseCount);
        var actual = Enumerable.Range(1, turn.PhaseCount).Select(p => turn.Phase(p).Start).Append(turn.End).Select(Text);
        Assert.Equal(boundaries, actual);
    }

    [Theory]
    [InlineData(1_900L, 2, 4)]
    [InlineData(2_000L, 2, 5)]
    [InlineData(2_023L, 2, 4)]
    [InlineData(2_023L, 4, 6)]
    [InlineData(2_023L, 1, 7)]
    [InlineData(-9_999L, 2, 4)]
    public void The_last_phase_of_a_month_runs_to_its_end(long year, int month, int days)
    {
        var turn = TurnDates.Starting(TurnStep.OneMonth, Instant.FromDate(new CalendarDate(year, month, 1)));

        Assert.Equal(days * Day, turn.Phase(5).Seconds);
        Assert.All(Enumerable.Range(1, 4), p => Assert.Equal(6 * Day, turn.Phase(p).Seconds));
    }

    [Fact]
    public void A_leap_day_lengthens_the_phase_that_contains_it()
    {
        var leap = TurnDates.Starting(TurnStep.OneYear, At("2000-01-01"));
        var common = TurnDates.Starting(TurnStep.OneYear, At("1900-01-01"));

        Assert.Equal(91 * Day, leap.Phase(1).Seconds);
        Assert.Equal(90 * Day, common.Phase(1).Seconds);
        Assert.Equal(366 * Day, leap.Span.Seconds);
        Assert.Equal(365 * Day, common.Span.Seconds);
    }

    [Theory]
    [InlineData(TurnStep.FiftyYears, "1900-01-01", true)]
    [InlineData(TurnStep.FiftyYears, "1901-01-01", true)]
    [InlineData(TurnStep.FiftyYears, "1900-07-01", false)]
    [InlineData(TurnStep.TwentyYears, "1900-07-01", false)]
    [InlineData(TurnStep.TenYears, "1900-07-01", false)]
    [InlineData(TurnStep.FiveYears, "1900-07-01", false)]
    [InlineData(TurnStep.TwoYears, "1900-07-01", false)]
    [InlineData(TurnStep.OneYear, "1900-04-01", false)]
    [InlineData(TurnStep.OneYear, "-9999-01-01", true)]
    [InlineData(TurnStep.SixMonths, "1900-01-01", true)]
    [InlineData(TurnStep.SixMonths, "1900-07-01", true)]
    [InlineData(TurnStep.SixMonths, "1900-04-01", false)]
    [InlineData(TurnStep.OneMonth, "1900-04-01", true)]
    [InlineData(TurnStep.OneMonth, "1900-04-07", false)]
    [InlineData(TurnStep.OneMonth, "1900-04-02", false)]
    public void Turns_start_only_on_their_steps_dates(TurnStep step, string date, bool valid)
    {
        Assert.Equal(valid, TurnDates.IsValidStart(step, At(date)));
        if (!valid)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => TurnDates.Starting(step, At(date)));
        }
    }

    [Fact]
    public void Turns_start_at_midnight()
    {
        var oneSecondLate = At("1900-01-01").AddSeconds(1);

        Assert.False(TurnDates.IsValidStart(TurnStep.OneMonth, oneSecondLate));
        Assert.Throws<ArgumentOutOfRangeException>(() => TurnDates.Starting(TurnStep.OneMonth, oneSecondLate));
    }

    [Fact]
    public void An_event_on_a_phase_boundary_belongs_to_the_phase_starting_there()
    {
        var turn = TurnDates.Starting(TurnStep.OneYear, At("2000-01-01"));
        var firstOfJuly = At("2000-07-01");

        Assert.Equal(3, turn.PhaseContaining(firstOfJuly));
        Assert.Equal(2, turn.PhaseContaining(firstOfJuly.AddSeconds(-1)));
        Assert.True(turn.Phase(3).Contains(firstOfJuly));
        Assert.False(turn.Phase(2).Contains(firstOfJuly));
    }

    [Fact]
    public void An_event_on_a_turn_boundary_belongs_to_the_first_phase_of_the_new_turn()
    {
        var turn = TurnDates.Starting(TurnStep.SixMonths, At("2000-01-01"));
        var next = turn.Next;

        Assert.Equal(At("2000-07-01"), next.Start);
        Assert.False(turn.Contains(turn.End));
        Assert.True(next.Contains(turn.End));
        Assert.Equal(1, next.PhaseContaining(turn.End));
        Assert.Equal(3, turn.PhaseContaining(turn.End.AddSeconds(-1)));
        Assert.Throws<ArgumentOutOfRangeException>(() => turn.PhaseContaining(turn.End));
        Assert.Throws<ArgumentOutOfRangeException>(() => turn.PhaseContaining(turn.Start.AddSeconds(-1)));
    }

    [Fact]
    public void Phase_numbers_count_from_1()
    {
        var turn = TurnDates.Starting(TurnStep.OneYear, At("2000-01-01"));

        Assert.Throws<ArgumentOutOfRangeException>(() => turn.Phase(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => turn.Phase(5));
    }

    [Fact]
    public void The_default_is_the_first_50_year_turn_at_the_epoch()
    {
        var turn = default(TurnDates);

        Assert.Equal(TurnStep.FiftyYears, turn.Step);
        Assert.Equal(Instant.Epoch, turn.Start);
        Assert.Equal(TurnDates.Starting(TurnStep.FiftyYears, Instant.Epoch), turn);
        Assert.Equal("-9949-01-01", Text(turn.End));
    }

    [Fact]
    public void Every_second_of_a_turn_lies_in_exactly_one_phase()
    {
        var gen =
            from turn in TurnGen()
            from offset in Gen.Choose(0, int.MaxValue)
            select (turn, instant: turn.Start.AddSeconds(offset % turn.Span.Seconds));

        TimeGen.Check(Prop.ForAll(Arb.From(gen), c =>
        {
            var holding = Enumerable.Range(1, c.turn.PhaseCount).Where(p => c.turn.Phase(p).Contains(c.instant)).ToArray();
            return holding.Length == 1 && c.turn.PhaseContaining(c.instant) == holding[0];
        }));
    }

    [Fact]
    public void Phases_tile_their_turn_without_gaps_or_overlaps()
    {
        TimeGen.Check(Prop.ForAll(Arb.From(TurnGen()), turn =>
        {
            var phases = Enumerable.Range(1, turn.PhaseCount).Select(turn.Phase).ToArray();
            return phases[0].Start == turn.Start
                && phases[^1].End == turn.End
                && phases.Zip(phases.Skip(1)).All(p => p.First.End == p.Second.Start)
                && phases.All(p => p.Seconds > 0)
                && phases.Select((p, i) => turn.PhaseContaining(p.Start) == i + 1).All(ok => ok)
                && TurnDates.IsValidStart(turn.Step, turn.End)
                && turn.Next.Start == turn.End;
        }));
    }

    // A random step and a random valid start for it, over a wide range of years.
    private static Gen<TurnDates> TurnGen() =>
        from step in Gen.Elements(StepTable.All.ToArray())
        from year in Gen.Choose(-2_000_000, 2_000_000)
        from month in Gen.Choose(1, 12)
        let startMonth = step == TurnStep.OneMonth ? month
            : step == TurnStep.SixMonths ? (month <= 6 ? 1 : 7)
            : 1
        select TurnDates.Starting(step, Instant.FromDate(new CalendarDate(year, startMonth, 1)));

    private static Instant At(string date)
    {
        var negative = date.StartsWith('-');
        var parts = (negative ? date[1..] : date).Split('-');
        var year = long.Parse(parts[0], CultureInfo.InvariantCulture) * (negative ? -1 : 1);
        return Instant.FromDate(new CalendarDate(
            year,
            int.Parse(parts[1], CultureInfo.InvariantCulture),
            int.Parse(parts[2], CultureInfo.InvariantCulture)));
    }

    private static string Text(Instant instant)
    {
        Assert.Equal(0, instant.SecondOfDay);
        var date = instant.Date;
        return string.Create(CultureInfo.InvariantCulture, $"{date.Year}-{date.Month:D2}-{date.Day:D2}");
    }
}
