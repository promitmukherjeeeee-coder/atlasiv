namespace Atlasiv.Core.Time;

/// <summary>
/// The leading edge: the fastest transport capability that is operationally usable at
/// meaningful capacity, held by any entity with at least 0.5% of world population (Pass 1
/// spec section 4). The step controller reads it through this interface. Pass 1 has no
/// transport system, so tests and the dummy scenario supply scripted values.
/// </summary>
public interface ILeadingEdge
{
    /// <summary>
    /// The leading edge's strategic time for <paramref name="undertaking"/>, in whole seconds,
    /// as it stands at <paramref name="asOf"/> (a turn boundary). Never negative.
    /// </summary>
    long StrategicSeconds(ReferenceUndertaking undertaking, Instant asOf);
}
