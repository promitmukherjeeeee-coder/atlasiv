using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Atlasiv.Core.Numerics;
using FsCheck;
using FsCheck.Fluent;
using Xunit;

namespace Atlasiv.Core.Tests.Numerics;

/// <summary>
/// Sums, aggregates and exposures accumulate in 128-bit integers (Pass 1 spec section 8,
/// clarification 14). An accumulator holds totals far beyond the 64-bit range exactly,
/// and leaving even the 128-bit range is a simulation overflow, never a wrap.
/// </summary>
public sealed class AccumulatorTests
{
    private static Int128 Parse(string value) => Int128.Parse(value, CultureInfo.InvariantCulture);

    [Fact]
    public void An_empty_accumulator_totals_zero()
    {
        Assert.Equal(Int128.Zero, Accumulator.Zero.Total);
        Assert.Equal(0L, Accumulator.Zero.ToInt64());
    }

    [Fact]
    public void A_sum_beyond_the_64_bit_range_is_exact()
    {
        var total = Accumulator.Zero.Add(long.MaxValue).Add(long.MaxValue).Add(long.MaxValue);
        Assert.Equal(Parse("27670116110564327421"), total.Total);
    }

    [Fact]
    public void Negative_amounts_such_as_liabilities_are_summed_exactly()
    {
        var total = Accumulator.Zero.Add(long.MinValue).Add(long.MinValue).Add(5);
        Assert.Equal(Parse("-18446744073709551611"), total.Total);
    }

    [Fact]
    public void Reading_a_total_as_64_bits_overflows_when_it_does_not_fit()
    {
        var total = Accumulator.Zero.Add(long.MaxValue).Add(1);
        var error = Assert.Throws<SimulationOverflowException>(() => total.ToInt64());
        Assert.Contains("9223372036854775808", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void A_total_that_fits_reads_back_as_64_bits()
    {
        Assert.Equal(long.MaxValue, Accumulator.Zero.Add(long.MaxValue).Add(-1).Add(1).ToInt64());
    }

    [Fact]
    public void An_exposure_adds_quantity_times_price_exactly()
    {
        // A trillion units at ten billion minor units each: 10^22 minor units.
        var exposure = Accumulator.Zero.AddProduct(1_000_000_000_000, 10_000_000_000);
        Assert.Equal(Parse("10000000000000000000000"), exposure.Total);
    }

    [Fact]
    public void The_largest_single_product_is_exact()
    {
        // (-2^63) x (-2^63) = 2^126.
        Assert.Equal(Parse("85070591730234615865843651857942052864"), Accumulator.Zero.AddProduct(long.MinValue, long.MinValue).Total);
    }

    [Fact]
    public void Leaving_the_128_bit_range_upward_is_a_simulation_overflow()
    {
        var once = Accumulator.Zero.AddProduct(long.MinValue, long.MinValue); // 2^126
        Assert.Throws<SimulationOverflowException>(() => once.AddProduct(long.MinValue, long.MinValue)); // 2^127
        Assert.Throws<SimulationOverflowException>(() => once.Add(once));
    }

    [Fact]
    public void Leaving_the_128_bit_range_downward_is_a_simulation_overflow()
    {
        // -2^63 x (2^63 - 1) = -2^126 + 2^63. Twice fits; three times is below -2^127.
        var twice = Accumulator.Zero.AddProduct(long.MinValue, long.MaxValue).AddProduct(long.MinValue, long.MaxValue);
        Assert.Equal(Parse("-170141183460469231713240559642174554112"), twice.Total);
        Assert.Throws<SimulationOverflowException>(() => twice.AddProduct(long.MinValue, long.MaxValue));
    }

    /// <summary>The total is the exact sum of every entry.</summary>
    [Fact]
    public void The_total_is_the_exact_sum() =>
        NumericGen.Check(Prop.ForAll(Arb.From(Entries), entries =>
            (BigInteger)Accumulate(entries).Total == entries.Aggregate(BigInteger.Zero, (sum, e) => sum + Exact(e))));

    /// <summary>
    /// Order and grouping never change the total, so partial totals built in any order
    /// (for example in parallel) and then combined give identical results.
    /// </summary>
    [Fact]
    public void Order_and_grouping_never_change_the_total() =>
        NumericGen.Check(Prop.ForAll(
            Arb.From(from entries in Entries from cut in Gen.Choose(0, entries.Length) select (entries, cut)),
            t =>
            {
                var inOrder = Accumulate(t.entries);
                var reversed = Accumulate(t.entries.Reverse());
                var combined = Accumulate(t.entries.Skip(t.cut)).Add(Accumulate(t.entries.Take(t.cut)));
                return inOrder == reversed && inOrder == combined;
            }));

    [Fact]
    public void Reading_as_64_bits_gives_the_exact_total_or_an_overflow() =>
        NumericGen.Check(Prop.ForAll(Arb.From(Entries), entries =>
        {
            var total = Accumulate(entries);
            var expected = NumericGen.InRange((BigInteger)total.Total);
            try
            {
                return expected == total.ToInt64();
            }
            catch (SimulationOverflowException)
            {
                return expected is null;
            }
        }));

    /// <summary>Up to 30 entries: amounts anywhere in the 64-bit range, and products below 2^120 each, so no total leaves 128 bits.</summary>
    private static Gen<(long A, long? B)[]> Entries =>
        from count in Gen.Choose(0, 30)
        from entries in Gen.ArrayOf(
            Gen.OneOf(
                NumericGen.Raw().Select(a => (a, (long?)null)),
                from a in NumericGen.Raw(60) from b in NumericGen.Raw(60) select (a, (long?)b)),
            count)
        select entries;

    private static BigInteger Exact((long A, long? B) entry) =>
        entry.B is { } b ? (BigInteger)entry.A * b : entry.A;

    private static Accumulator Accumulate(System.Collections.Generic.IEnumerable<(long A, long? B)> entries) =>
        entries.Aggregate(Accumulator.Zero, (sum, e) => e.B is { } b ? sum.AddProduct(e.A, b) : sum.Add(e.A));
}
