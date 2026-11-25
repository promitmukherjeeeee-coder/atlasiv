using System.Numerics;
using Atlasiv.Core.Numerics;
using FsCheck;
using FsCheck.Fluent;
using Xunit;

namespace Atlasiv.Core.Tests.Numerics;

/// <summary>
/// exp(-mu) at 64 fractional bits, the internal function behind Poisson draws (ENG-009).
/// Hard rule 4: a transcendental function in authoritative code needs test vectors.
/// </summary>
public sealed class ExpNegativeQ64Tests
{
    // Stated accuracy, in units of 2^-64 (ENG-009): 4, that is 2^-62. Observed at most 2.
    private const int Bound = 4;

    [Theory]
    // mu as a Q32 raw value; exp(-mu) * 2^64 rounded half to even, computed in Python with
    // 90-digit decimals.
    [InlineData(0L, "18446744073709551616")]                  // exactly 1
    [InlineData(1L, "18446744069414584320")]                  // the smallest mean, 2^-32
    [InlineData(65_536L, "18446462600880313685")]             // 2^-16
    [InlineData(2_147_483_648L, "11188515852577165300")]      // 0.5
    [InlineData(4_294_967_296L, "6786177901268885275")]       // 1
    [InlineData(2_977_044_472L, "9223372036467309782")]       // ln 2 rounded to Q32: the k = 1 / k = 2 boundary
    [InlineData(10_737_418_240L, "1514200961905861799")]      // 2.5
    [InlineData(42_949_672_960L, "837480885292947")]          // 10
    [InlineData(68_719_476_736L, "2075907567336")]            // 16, the Poisson piece size
    [InlineData(94_489_280_512L, "5145660401")]               // 22
    [InlineData(188_978_561_024L, "1")]                       // 44
    [InlineData(197_568_495_616L, "0")]                       // 46
    public void Exp_negative_Q64_matches_the_published_vectors(long muRaw, string expected)
    {
        var actual = (BigInteger)FixedMath.ExpNegativeQ64(Q32.FromRaw(muRaw));

        Assert.InRange(BigInteger.Abs(actual - BigInteger.Parse(expected, System.Globalization.CultureInfo.InvariantCulture)), BigInteger.Zero, (BigInteger)Bound);
    }

    [Fact]
    public void Exp_negative_Q64_is_within_its_stated_bound_of_a_160_bit_reference()
    {
        var mu = Gen.Choose(0, 48 * 1_024).SelectMany(whole => Gen.Choose(0, 4_194_303).Select(frac => ((long)whole << 22) + frac));

        NumericGen.Check(Prop.ForAll(Arb.From(mu), raw =>
        {
            var actual = (BigInteger)FixedMath.ExpNegativeQ64(Q32.FromRaw(raw));
            var expected = ReferenceMath.Exp(-raw, 64);
            return BigInteger.Abs(actual - expected) <= Bound;
        }));
    }

    [Fact]
    public void Exp_negative_Q64_refuses_a_negative_input()
    {
        Assert.Throws<SimulationDomainException>(() => FixedMath.ExpNegativeQ64(-Q32.Epsilon));
    }
}
