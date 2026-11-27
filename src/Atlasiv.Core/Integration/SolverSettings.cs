using Atlasiv.Core.Numerics;

namespace Atlasiv.Core.Integration;

/// <summary>
/// A solver's iteration cap and convergence tolerance (spec section 10). Tolerances are set during
/// calibration, so Pass 1 carries only uncalibrated placeholders.
/// </summary>
public sealed record SolverSettings
{
    /// <summary>The placeholder iteration cap until calibration.</summary>
    public const int PlaceholderIterationCap = 100; // CALIBRATE: per solver, Pass 3 and calibration.

    /// <summary>Creates the settings.</summary>
    /// <param name="iterationCap">The most iterations to run; at least 1.</param>
    /// <param name="tolerance">The largest residual that counts as converged; not negative.</param>
    /// <exception cref="SimulationDomainException">The cap is below 1 or the tolerance is negative (both are calibration data).</exception>
    public SolverSettings(int iterationCap, Q32 tolerance)
    {
        if (iterationCap < 1)
        {
            throw new SimulationDomainException(nameof(iterationCap), iterationCap, "A solver needs an iteration cap of at least 1.");
        }

        if (tolerance < Q32.Zero)
        {
            throw new SimulationDomainException(nameof(tolerance), tolerance, "A tolerance cannot be negative.");
        }

        IterationCap = iterationCap;
        Tolerance = tolerance;
    }

    /// <summary>The placeholder tolerance until calibration: the smallest Q32 step, 2^-32.</summary>
    public static Q32 PlaceholderTolerance => Q32.Epsilon; // CALIBRATE: per solver, Pass 3 and calibration.

    /// <summary>The placeholder settings used until calibration.</summary>
    public static SolverSettings Uncalibrated => new(PlaceholderIterationCap, PlaceholderTolerance); // CALIBRATE: per solver, Pass 3 and calibration.

    /// <summary>The most iterations to run.</summary>
    public int IterationCap { get; }

    /// <summary>The largest residual that counts as converged.</summary>
    public Q32 Tolerance { get; }
}
