namespace Atlasiv.Core.Events;

/// <summary>
/// Whether an event resolves on its own or halts Advance Until (Pass 1 spec section 5). The
/// player otherwise acts only at phase boundaries or at rare, formally defined interruption
/// points.
/// </summary>
/// <remarks>
/// There is no value 0, so an event whose class was never set is refused rather than quietly
/// treated as informational.
/// </remarks>
public enum InterruptionClass
{
    /// <summary>Resolves automatically; Advance Until does not stop for it unless the player's filters say so.</summary>
    Informational = 1,

    /// <summary>Pauses Advance Until. Its phase still finishes, and control returns at the next phase boundary (spec clarification 9).</summary>
    DecisionCritical = 2,
}
