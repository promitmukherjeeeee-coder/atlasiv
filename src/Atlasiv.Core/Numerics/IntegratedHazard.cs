using System;

namespace Atlasiv.Core.Numerics;

/// <summary>
/// The integrated hazard H = Σ λᵢΔtᵢ of a rate that varies piece by piece over time.
/// Each piece adds its exact product of raw rate and seconds to a 128-bit sum, and H is
/// rounded once, at the end. So any split of the same interval, in any order, gives
/// identical results: the time-step consistency the spec asks for (section 13).
/// </summary>
/// <remarks>The default value, <see cref="None"/>, is an empty interval with H = 0.</remarks>
public readonly struct IntegratedHazard : IEquatable<IntegratedHazard>
{
    // Above 30 the survival exp(-H) is below 2^-43, which rounds to 0 in Q32.
    private const long CertainRaw = 30L << Q32.FractionBits;

    // Sum of rate.Raw * seconds: H in units of 2^-32 rate-years times RateYear.Seconds.
    private readonly Int128 _rateSeconds;

    private IntegratedHazard(Int128 rateSeconds) => _rateSeconds = rateSeconds;

    /// <summary>No hazard: H = 0.</summary>
    public static IntegratedHazard None => default;

    /// <summary>H, the integrated hazard in rate-years times events per year, rounded half to even.</summary>
    /// <exception cref="SimulationOverflowException">H is above <see cref="Q32.MaxValue"/>.</exception>
    public Q32 Value => Q32.FromRaw(WideMath.ToInt64(RoundedRaw, "IntegratedHazard.Value"));

    /// <summary>exp(-H): the probability that the event does not happen.</summary>
    public Q32 Survival
    {
        get
        {
            var h = RoundedRaw;
            return h >= CertainRaw ? Q32.Zero : FixedMath.Exp(Q32.FromRaw(-(long)h));
        }
    }

    /// <summary>1 - exp(-H): the probability that the event happens at least once.</summary>
    public Q32 Probability => Q32.One - Survival;

    private Int128 RoundedRaw => WideMath.DivideHalfToEven(_rateSeconds, RateYear.Seconds);

    /// <summary>Adds a piece with a constant rate over an exact elapsed time.</summary>
    /// <param name="ratePerYear">λ, events per rate year; must not be negative.</param>
    /// <param name="elapsedSeconds">Δt in seconds; must not be negative.</param>
    /// <exception cref="ArgumentOutOfRangeException">The rate or the time is negative.</exception>
    /// <exception cref="SimulationOverflowException">The running sum leaves the 128-bit range.</exception>
    public IntegratedHazard Add(Q32 ratePerYear, long elapsedSeconds)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(ratePerYear, Q32.Zero);
        ArgumentOutOfRangeException.ThrowIfNegative(elapsedSeconds);

        var piece = (Int128)ratePerYear.Raw * elapsedSeconds; // below 2^126
        if (_rateSeconds > Int128.MaxValue - piece)
        {
            throw new SimulationOverflowException("IntegratedHazard.Add overflowed the 128-bit running sum.");
        }

        return new IntegratedHazard(_rateSeconds + piece);
    }

    /// <inheritdoc/>
    public bool Equals(IntegratedHazard other) => _rateSeconds == other._rateSeconds;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is IntegratedHazard other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => _rateSeconds.GetHashCode();

    /// <summary>True when both hold exactly the same integrated hazard.</summary>
    public static bool operator ==(IntegratedHazard left, IntegratedHazard right) => left.Equals(right);

    /// <summary>True when the integrated hazards differ.</summary>
    public static bool operator !=(IntegratedHazard left, IntegratedHazard right) => !left.Equals(right);
}
