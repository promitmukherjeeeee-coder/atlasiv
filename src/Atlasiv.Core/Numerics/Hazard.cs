using System;

namespace Atlasiv.Core.Numerics;

/// <summary>
/// The hazard primitive: the probability that an event with a per-year rate λ happens
/// within an exact elapsed time Δt is 1 - exp(-λΔt) (Pass 1 spec section 10).
/// </summary>
public static class Hazard
{
    /// <summary>1 - exp(-λΔt) for a constant rate over an exact elapsed time.</summary>
    /// <param name="ratePerYear">λ, events per rate year; must not be negative.</param>
    /// <param name="elapsedSeconds">Δt in seconds; must not be negative.</param>
    /// <exception cref="SimulationDomainException">The rate or the time is negative.</exception>
    public static Q32 Probability(Q32 ratePerYear, long elapsedSeconds) =>
        IntegratedHazard.None.Add(ratePerYear, elapsedSeconds).Probability;
}
