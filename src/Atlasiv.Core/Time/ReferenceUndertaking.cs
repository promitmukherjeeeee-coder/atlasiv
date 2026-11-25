namespace Atlasiv.Core.Time;

/// <summary>
/// The reference undertaking that sets the step: a substantial continental undertaking, which
/// should take about 2 turns of strategic operational pacing (Pass 1 spec section 4). The
/// Final Master Vision (4.1) describes it as a continent crossing for people, armies and goods.
/// Which crossing, and its distance, are calibrated in the data phase (spec section 14,
/// decision 2), so Pass 1 carries only an uncalibrated placeholder.
/// </summary>
/// <param name="DistanceMetres">The undertaking's distance in metres.</param>
public readonly record struct ReferenceUndertaking(long DistanceMetres)
{
    /// <summary>
    /// The reference distance in metres. The spec leaves it open; 0 marks "not yet set", so any
    /// real transport system that is handed the placeholder gives an obviously wrong answer.
    /// </summary>
    public const long PlaceholderDistanceMetres = 0; // CALIBRATE: reference continent crossing, data phase.

    /// <summary>The placeholder undertaking used until calibration.</summary>
    public static ReferenceUndertaking Uncalibrated => new(PlaceholderDistanceMetres); // CALIBRATE: which undertaking, data phase.
}
