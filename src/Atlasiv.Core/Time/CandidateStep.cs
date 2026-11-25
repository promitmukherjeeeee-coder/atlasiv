using System;
using Atlasiv.Core.Numerics;

namespace Atlasiv.Core.Time;

/// <summary>
/// The candidate step (Pass 1 spec section 4, clarification 7): the step whose nominal length
/// is closest on a log scale to half the leading edge's strategic time for the reference
/// undertaking, so that the undertaking takes about 2 turns.
/// </summary>
/// <remarks>
/// <para>
/// On a log scale the point halfway between two neighbouring steps of nominal lengths
/// L_slower and L_faster is their geometric mean, so a target T = S / 2 is closer to the
/// slower step when T² is above L_slower × L_faster. Multiplying both sides by 4 compares
/// S² with 4 × L_slower × L_faster in 128-bit integers: exact, with no logarithm and no
/// rounding of S / 2.
/// </para>
/// <para>
/// An exact tie goes to the slower step. Targets beyond the table pick 50 years or 1 month.
/// </para>
/// </remarks>
public static class CandidateStep
{
    /// <summary>The candidate step for a leading-edge strategic time of <paramref name="strategicSeconds"/> whole seconds.</summary>
    /// <exception cref="SimulationDomainException">The strategic time is negative.</exception>
    public static TurnStep For(long strategicSeconds)
    {
        if (strategicSeconds < 0)
        {
            throw new SimulationDomainException(nameof(strategicSeconds), strategicSeconds, "A strategic time cannot be negative.");
        }

        return ForSquaredStrategicTime((Int128)strategicSeconds * strategicSeconds);
    }

    /// <summary>
    /// The candidate for a squared strategic time S². Separate from <see cref="For"/> only so
    /// the tie rule can be tested: no whole-second S lands exactly on a tie.
    /// </summary>
    internal static TurnStep ForSquaredStrategicTime(Int128 strategicSquared)
    {
        // Slowest first: the first neighbour pair whose midpoint the target reaches picks the slower step.
        var steps = StepTable.All;
        for (var i = 0; i + 1 < steps.Count; i++)
        {
            var tie = 4 * (Int128)StepTable.NominalSeconds(steps[i]) * StepTable.NominalSeconds(steps[i + 1]);
            if (strategicSquared >= tie)
            {
                return steps[i];
            }
        }

        return steps[^1];
    }
}
