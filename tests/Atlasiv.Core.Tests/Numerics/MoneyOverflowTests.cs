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
/// Money stored as 64-bit minor units under the stress cases named in Pass 1 spec section 8
/// and clarification 14: denomination change, high inflation, redenomination and large
/// aggregate exposures. Where a result fits, it is exact or rounded half to even; where it
/// does not, the result is a simulation overflow, never a wrap or a clip.
/// </summary>
/// <remarks>
/// The historical figures are test data that put the limits of the format on record for the
/// Pass 2 decision on per-currency scales (registry ADR-042). They are not game values.
/// A conversion by the ratio b ÷ c is <see cref="Rounding.MultiplyDivideHalfToEven"/>.
/// </remarks>
public sealed class MoneyOverflowTests
{
    // ---- Denomination change ----

    /// <summary>UK decimalisation, 1971: 240 old pence became 100 new pence, so 1d = 5/12 p.</summary>
    [Theory]
    [InlineData(240L, 100L)]   // one pound
    [InlineData(12L, 5L)]      // one shilling
    [InlineData(6L, 2L)]       // 2.5 rounds to the even 2
    [InlineData(18L, 8L)]      // 7.5 rounds to the even 8
    [InlineData(30L, 12L)]     // 12.5 rounds to the even 12
    [InlineData(3L, 1L)]       // 1.25
    [InlineData(1L, 0L)]       // 0.4166...
    [InlineData(-6L, -2L)]     // a debt converts the same way
    public void Decimalisation_converts_old_pence_to_new_pence_half_to_even(long oldPence, long newPence)
    {
        Assert.Equal(newPence, Rounding.MultiplyDivideHalfToEven(oldPence, 5, 12));
    }

    /// <summary>A minor unit ten times finer multiplies every amount by 10; past the 64-bit range it overflows.</summary>
    [Fact]
    public void A_finer_minor_unit_overflows_at_the_top_of_the_range()
    {
        Assert.Equal(9_223_372_036_854_775_800L, Rounding.MultiplyDivideHalfToEven(922_337_203_685_477_580L, 10, 1));
        Assert.Throws<SimulationOverflowException>(() => Rounding.MultiplyDivideHalfToEven(922_337_203_685_477_581L, 10, 1));
        Assert.Throws<SimulationOverflowException>(() => Rounding.MultiplyDivideHalfToEven(-922_337_203_685_477_581L, 10, 1));
    }

    // ---- High inflation ----

    /// <summary>
    /// Hungary, 1946: the 100 million b.-pengő note was worth 10^20 pengő (1 b.-pengő = 10^12 pengő),
    /// which is 10^22 fillér. A single banknote does not fit in 64-bit minor units, so converting it
    /// is a simulation overflow; a 128-bit aggregate still holds it exactly.
    /// </summary>
    [Fact]
    public void Hungarian_1946_banknote_overflows_64_bit_minor_units()
    {
        const long notes = 100_000_000;                  // 100 million b.-pengő
        const long fillerPerBPengo = 100_000_000_000_000; // 10^12 pengő x 100 fillér

        Assert.Throws<SimulationOverflowException>(() => Rounding.MultiplyDivideHalfToEven(notes, fillerPerBPengo, 1));
        Assert.Equal(
            Int128.Parse("10000000000000000000000", CultureInfo.InvariantCulture),
            Accumulator.Zero.AddProduct(notes, fillerPerBPengo).Total);
    }

    /// <summary>
    /// Zimbabwe, 2009: the 100 trillion dollar note is 10^16 cents. 922 of them fit in 64 bits;
    /// 923 do not.
    /// </summary>
    [Fact]
    public void Zimbabwean_100_trillion_dollar_notes_reach_the_64_bit_limit_at_923()
    {
        const long centsPerNote = 10_000_000_000_000_000;

        Assert.Equal(9_220_000_000_000_000_000L, Rounding.MultiplyDivideHalfToEven(922, centsPerNote, 1));
        Assert.Throws<SimulationOverflowException>(() => Rounding.MultiplyDivideHalfToEven(923, centsPerNote, 1));
        Assert.Equal(9_230_000_000_000_000_000UL, (ulong)Accumulator.Zero.AddProduct(923, centsPerNote).Total);
    }

    /// <summary>
    /// A price compounded by an inflation factor every period matches the exact half-to-even
    /// result at every step, and the first step whose true value passes the 64-bit range is a
    /// simulation overflow: the price never wraps to a small or negative number.
    /// </summary>
    [Fact]
    public void Compounding_inflation_is_exact_until_it_overflows() =>
        NumericGen.Check(Prop.ForAll(
            Arb.From(from price in Gen.Choose(10, 1_000_000)
                     from factor in Gen.Choose(110, 100_000) // 1.10 to 1000 per period
                     select (Price: (long)price, Factor: Q32.FromRatio(factor, 100))),
            t =>
            {
                var price = t.Price;
                while (true)
                {
                    var exact = NumericGen.DivideHalfToEven((BigInteger)price * t.Factor.Raw, BigInteger.One << 32);
                    if (exact > long.MaxValue)
                    {
                        return NumericGen.RawOrOverflow(() => Q32.FromRaw(Rounding.MultiplyHalfToEven(price, t.Factor))) is null;
                    }

                    var next = Rounding.MultiplyHalfToEven(price, t.Factor);
                    if (next != exact || next <= price)
                    {
                        return false;
                    }

                    price = next;
                }
            }));

    /// <summary>Doubling from one minor unit fits 62 times; the 63rd doubling overflows.</summary>
    [Fact]
    public void Doubling_a_price_overflows_on_the_63rd_doubling()
    {
        var price = 1L;
        for (var i = 0; i < 62; i++)
        {
            price = Rounding.MultiplyHalfToEven(price, Q32.FromInt(2));
        }

        Assert.Equal(1L << 62, price);
        Assert.Throws<SimulationOverflowException>(() => Rounding.MultiplyHalfToEven(price, Q32.FromInt(2)));
    }

    // ---- Redenomination ----

    /// <summary>Turkey, 2005: 1 new lira = 1,000,000 old lira.</summary>
    [Theory]
    [InlineData(2_500_000L, 2L)]                            // 2.5 rounds to the even 2
    [InlineData(3_500_000L, 4L)]                            // 3.5 rounds to the even 4
    [InlineData(1_500_000L, 2L)]
    [InlineData(500_000L, 0L)]                              // 0.5 rounds to the even 0
    [InlineData(1_499_999L, 1L)]
    [InlineData(-2_500_000L, -2L)]
    [InlineData(long.MaxValue, 9_223_372_036_855L)]         // 9,223,372,036,854.775807
    [InlineData(long.MinValue, -9_223_372_036_855L)]
    public void Turkish_2005_redenomination_removes_six_zeros_half_to_even(long oldAmount, long newAmount)
    {
        Assert.Equal(newAmount, Rounding.MultiplyDivideHalfToEven(oldAmount, 1, 1_000_000));
    }

    /// <summary>Zimbabwe, August 2008: ten zeros removed (10^10 old dollars = 1 new dollar).</summary>
    [Fact]
    public void Zimbabwean_2008_redenomination_removes_ten_zeros()
    {
        Assert.Equal(1_000_000L, Rounding.MultiplyDivideHalfToEven(10_000_000_000_000_000L, 1, 10_000_000_000));
        Assert.Equal(2L, Rounding.MultiplyDivideHalfToEven(15_000_000_000L, 1, 10_000_000_000)); // 1.5 rounds to the even 2
        Assert.Equal(2L, Rounding.MultiplyDivideHalfToEven(25_000_000_000L, 1, 10_000_000_000)); // 2.5 rounds to the even 2
    }

    /// <summary>
    /// Redenominating each balance rounds each one, so the sum of the new balances can differ
    /// from the redenominated old total. The difference is at most half a new unit per balance
    /// plus half for the total, and is always a whole number of units that a ledger has to post.
    /// </summary>
    [Fact]
    public void Redenominating_balances_one_by_one_moves_the_total_by_at_most_half_a_unit_each() =>
        NumericGen.Check(Prop.ForAll(
            Arb.From(from count in Gen.Choose(1, 50)
                     from balances in Gen.ArrayOf(NumericGen.Raw(), count)
                     from zeros in Gen.Choose(1, 12)
                     select (balances, Divisor: (long)BigInteger.Pow(10, zeros))),
            t =>
            {
                var newBalances = t.balances.Aggregate(
                    Accumulator.Zero, (sum, b) => sum.Add(Rounding.MultiplyDivideHalfToEven(b, 1, t.Divisor)));
                var oldTotal = t.balances.Aggregate(Accumulator.Zero, (sum, b) => sum.Add(b));
                var newTotal = NumericGen.DivideHalfToEven((BigInteger)oldTotal.Total, t.Divisor);
                var difference = BigInteger.Abs((BigInteger)newBalances.Total - newTotal);
                return 2 * difference <= t.balances.Length + 1;
            }));

    // ---- Large aggregate exposures ----

    /// <summary>A million balances of 10^16 minor units each total 10^22: exact in 128 bits, too large for 64.</summary>
    [Fact]
    public void A_million_large_balances_total_exactly()
    {
        var total = Accumulator.Zero;
        for (var i = 0; i < 1_000_000; i++)
        {
            total = total.Add(10_000_000_000_000_000);
        }

        Assert.Equal(Int128.Parse("10000000000000000000000", CultureInfo.InvariantCulture), total.Total);
        Assert.Throws<SimulationOverflowException>(() => total.ToInt64());
    }

    /// <summary>Long and short positions in the same asset net to zero exactly, however large.</summary>
    [Fact]
    public void Offsetting_exposures_net_to_zero()
    {
        var net = Accumulator.Zero
            .AddProduct(long.MaxValue, 1_000_000_000_000)
            .AddProduct(-long.MaxValue, 1_000_000_000_000);
        Assert.Equal(Int128.Zero, net.Total);
        Assert.Equal(0L, net.ToInt64());
    }

    /// <summary>The largest possible exposures, quantity and price both at the 64-bit limit, add up exactly.</summary>
    [Fact]
    public void Maximum_exposures_sum_exactly_within_128_bits()
    {
        var gross = Accumulator.Zero
            .AddProduct(long.MaxValue, long.MaxValue)
            .AddProduct(long.MaxValue, long.MaxValue);
        Assert.Equal((BigInteger)long.MaxValue * long.MaxValue * 2, (BigInteger)gross.Total);
    }
}
