namespace Atlasiv.Core.Orders;

/// <summary>
/// One validated order (player or AI) for one phase, as recorded in the input log (spec section
/// 11). Within a phase an order is identified by (issuer, sequence number), which also fixes its
/// place in the canonical order every system sees: by issuer, then sequence number.
/// </summary>
/// <param name="IssuerId">The entity that gave the order (a placeholder 64-bit entity ID until Pass 2).</param>
/// <param name="Sequence">The order's number among its issuer's orders in the phase.</param>
/// <param name="Payload">What the order asks for.</param>
public sealed record Order(ulong IssuerId, ulong Sequence, OrderPayload Payload);
