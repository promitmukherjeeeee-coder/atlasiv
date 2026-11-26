namespace Atlasiv.Core.Integration;

/// <summary>
/// Computes a system's propagator, such as a decay factor or a transition matrix, for one regime
/// over an exact elapsed time (spec section 10, clarification 18). The real propagators arrive
/// with their systems.
/// </summary>
/// <typeparam name="TRegime">The system's regime and parameters: everything the propagator depends on besides time.</typeparam>
/// <typeparam name="TPropagator">The propagator.</typeparam>
/// <remarks>
/// It must be a pure function of its two arguments, computed with the authoritative integer
/// algorithms, and must never be built by combining propagators for shorter times: fixed-point
/// rounding makes three 2-year steps differ from one 6-year step (clarification 18).
/// </remarks>
public interface IPropagatorSource<in TRegime, out TPropagator>
{
    /// <summary>The propagator for <paramref name="regime"/> over <paramref name="elapsedSeconds"/> seconds.</summary>
    TPropagator Compute(TRegime regime, long elapsedSeconds);
}
