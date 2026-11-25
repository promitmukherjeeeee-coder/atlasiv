using System;
using Atlasiv.Core.Numerics;

namespace Atlasiv.Core.Randomness;

/// <summary>
/// Reads one keyed stream's draws in order (draw 0, 1, 2 and so on) and turns them into
/// the values simulation rules need (spec section 9). Every value depends only on the
/// stream's address and the draws taken from it so far, never on other streams.
/// </summary>
/// <remarks>
/// A system makes one stream per (entity, phase or turn end, substep) it draws for. The
/// stream is a class, not a struct, so it cannot be copied by accident: a copied struct
/// would hand out the same draws twice (ENG-008).
/// </remarks>
public sealed class RandomDraws
{
    // Block numbers are at most 2^62 - 1, so this marks "no block yet".
    private ulong _blockNumber = ulong.MaxValue;
    private Block4x64 _block;

    /// <summary>Creates a stream that starts at <paramref name="firstDrawIndex"/>.</summary>
    /// <param name="key">The stream's address.</param>
    /// <param name="firstDrawIndex">The index of the first draw; 0 unless resuming a stream.</param>
    /// <exception cref="ArgumentException"><paramref name="key"/> is the default address, which is not valid.</exception>
    public RandomDraws(DrawAddress key, ulong firstDrawIndex = 0)
    {
        if (key.Turn < 1)
        {
            throw new ArgumentException("The default DrawAddress is not a valid address: turns count from 1.", nameof(key));
        }

        Key = key;
        NextDrawIndex = firstDrawIndex;
    }

    /// <summary>The stream's address.</summary>
    public DrawAddress Key { get; }

    /// <summary>The index of the next draw: the number of draws used so far, when the stream started at 0.</summary>
    public ulong NextDrawIndex { get; private set; }

    /// <summary>The next draw: 64 uniformly distributed bits.</summary>
    public ulong NextUInt64()
    {
        // Each Philox call yields four draws (DrawAddress layout); keep the block for the next three.
        var index = NextDrawIndex;
        var input = Key.GeneratorInput(index);
        var blockNumber = input.Counter.W0;
        if (blockNumber != _blockNumber)
        {
            _block = Philox4x64.Generate(input.Counter, input.Key0, input.Key1);
            _blockNumber = blockNumber;
        }

        NextDrawIndex = checked(index + 1);
        return _block[input.Word];
    }

    /// <summary>
    /// A whole number from 0 to <paramref name="bound"/> - 1, each exactly equally likely.
    /// Uses Lemire's multiply-and-reject method: usually one draw; a further draw only in the
    /// rare case (probability below bound / 2^64) that the first would bias the result.
    /// </summary>
    /// <exception cref="SimulationDomainException"><paramref name="bound"/> is zero.</exception>
    public ulong NextBelow(ulong bound)
    {
        if (bound == 0)
        {
            throw new SimulationDomainException(nameof(bound), bound, "A uniform integer needs a bound of at least 1.");
        }

        // The high word of draw * bound is uniform on [0, bound) once draws whose low word
        // falls below 2^64 mod bound are rejected.
        var product = (UInt128)NextUInt64() * bound;
        if (LowWord(product) < bound)
        {
            var threshold = (ulong.MaxValue - bound + 1) % bound; // 2^64 mod bound
            while (LowWord(product) < threshold)
            {
                product = (UInt128)NextUInt64() * bound;
            }
        }

        return (ulong)(product >> 64);
    }

    /// <summary>A whole number from <paramref name="minInclusive"/> to <paramref name="maxInclusive"/>, each exactly equally likely.</summary>
    /// <exception cref="SimulationDomainException"><paramref name="minInclusive"/> is above <paramref name="maxInclusive"/>.</exception>
    public long NextInRange(long minInclusive, long maxInclusive)
    {
        if (minInclusive > maxInclusive)
        {
            throw new SimulationDomainException(nameof(maxInclusive), maxInclusive, "The range is empty: its maximum is below its minimum.");
        }

        var span = (UInt128)((Int128)maxInclusive - minInclusive) + 1; // 1 to 2^64
        var offset = span > ulong.MaxValue ? NextUInt64() : NextBelow((ulong)span);
        return (long)((Int128)minInclusive + offset);
    }

    /// <summary>
    /// A fixed-point value uniform on [0, 1): one of the 2^32 Q32 values from 0 to
    /// 1 - 2^-32, each exactly equally likely. Uses the top 32 bits of one draw.
    /// </summary>
    public Q32 NextUnit() => Q32.FromRaw((long)(NextUInt64() >> Q32.FractionBits));

    /// <summary>
    /// True with probability exactly <paramref name="probability"/>: true when a uniform
    /// fixed-point draw is below it. Uses one draw.
    /// </summary>
    /// <exception cref="SimulationDomainException"><paramref name="probability"/> is outside 0 to 1.</exception>
    public bool NextBernoulli(Q32 probability)
    {
        if (probability < Q32.Zero || probability > Q32.One)
        {
            throw new SimulationDomainException(nameof(probability), probability, "A probability must lie between 0 and 1.");
        }

        return NextUnit() < probability;
    }

    /// <summary>
    /// <paramref name="value"/> rounded down or up to a neighbouring whole number, going up
    /// with probability equal to its fractional part, so the expected result is the value
    /// itself (spec section 8). Uses one draw, through <see cref="Rounding.Stochastic"/>.
    /// </summary>
    public long RoundStochastic(Q32 value) => Rounding.Stochastic(value, NextUInt64());

    /// <summary>
    /// <paramref name="count"/> times <paramref name="factor"/>, stochastically rounded on the
    /// exact product, for example turning a hazard into whole people. Uses one draw, through
    /// <see cref="Rounding.MultiplyStochastic"/>.
    /// </summary>
    /// <exception cref="SimulationOverflowException">The result is outside the 64-bit range.</exception>
    public long MultiplyStochastic(long count, Q32 factor) =>
        Rounding.MultiplyStochastic(count, factor, NextUInt64());

    /// <summary>
    /// A Poisson-distributed count with the given mean, for example the number of random
    /// events at a rate over a duration (spec section 10: rate × duration).
    /// </summary>
    /// <remarks>
    /// The mean is split into pieces of at most <see cref="PoissonPieceMean"/>, and each piece
    /// is drawn by inversion from one draw, with probabilities held at 64 fractional bits. A sum
    /// of independent Poisson counts is Poisson with the summed mean, so the split does not
    /// change the distribution. A mean of 0 uses no draw; otherwise one draw per piece, so the
    /// work grows with the mean (ENG-009).
    /// </remarks>
    /// <exception cref="SimulationDomainException"><paramref name="mean"/> is negative.</exception>
    public long NextPoisson(Q32 mean)
    {
        if (mean < Q32.Zero)
        {
            throw new SimulationDomainException(nameof(mean), mean, "A Poisson mean cannot be negative.");
        }

        var pieceRaw = PoissonPieceMean.Raw;
        var count = 0L;
        for (var remaining = mean.Raw; remaining > 0; remaining -= pieceRaw)
        {
            count += NextPoissonPiece(remaining < pieceRaw ? remaining : pieceRaw);
        }

        return count;
    }

    /// <summary>The largest part of a Poisson mean drawn from a single draw: 16.</summary>
    public static Q32 PoissonPieceMean => Q32.FromInt(16);

    /// <summary>
    /// A unit exponential value, -ln U with U uniform on (0, 1]: the integrated hazard an
    /// event waits for. An event with a varying rate happens when its integrated hazard
    /// (see <see cref="IntegratedHazard"/>) reaches this value. Uses one draw.
    /// </summary>
    /// <remarks>
    /// U takes the 2^32 values 2^-32 to 1, so results run from 0 to about 22.18; the cut-off
    /// tail beyond that has probability 2^-32. Accuracy is that of <see cref="FixedMath.Log"/>.
    /// </remarks>
    public Q32 NextUnitExponential()
    {
        var u = Q32.FromRaw((long)(NextUInt64() >> Q32.FractionBits) + 1); // 2^-32 to 1
        return -FixedMath.Log(u);
    }

    /// <summary>
    /// The waiting time, in seconds, until an event with a constant per-year rate happens:
    /// a unit exponential divided by the rate, rounded half to even to whole seconds (ADR-012).
    /// A system schedules the event at that time and draws again only when the rate changes.
    /// Uses one draw, even when the rate is 0.
    /// </summary>
    /// <param name="ratePerYear">Events per rate year; must not be negative.</param>
    /// <returns>The waiting time in seconds, or null when the rate is 0 and the event never happens.</returns>
    /// <exception cref="SimulationDomainException"><paramref name="ratePerYear"/> is negative.</exception>
    public long? NextTimeToEventSeconds(Q32 ratePerYear)
    {
        if (ratePerYear < Q32.Zero)
        {
            throw new SimulationDomainException(nameof(ratePerYear), ratePerYear, "A hazard rate cannot be negative.");
        }

        var exponential = NextUnitExponential();
        if (ratePerYear == Q32.Zero)
        {
            return null;
        }

        // At most about 22.18 * 2^32 * 31,556,952 (about 3.0e18) seconds, for the smallest rate: within 64 bits.
        return WideMath.ToInt64(
            WideMath.DivideHalfToEven((Int128)exponential.Raw * RateYear.Seconds, ratePerYear.Raw),
            "RandomDraws.NextTimeToEventSeconds");
    }

    /// <summary>Inversion: the smallest k with U below P(K ≤ k), where U = draw / 2^64.</summary>
    private long NextPoissonPiece(long meanRaw)
    {
        UInt128 u = NextUInt64();
        var p = FixedMath.ExpNegativeQ64(Q32.FromRaw(meanRaw)); // P(K = 0) at 64 fractional bits
        var cumulative = p;
        var k = 0L;

        // P(K = k) = P(K = k - 1) * mean / k. If P(K = k) underflows to 0 first, the tiny
        // remaining tail (below about 2^-58) goes to that k.
        while (u >= cumulative && p != UInt128.Zero)
        {
            k++;
            p = WideMath.DivideHalfToEven(p * (ulong)meanRaw, (UInt128)(ulong)k << Q32.FractionBits);
            cumulative += p;
        }

        return k;
    }

    /// <summary>The low 64 bits; the truncation is intended (ENG-003).</summary>
    private static ulong LowWord(UInt128 value) => unchecked((ulong)value);
}
