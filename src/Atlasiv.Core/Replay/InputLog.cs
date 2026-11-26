using System;
using System.Collections.Generic;
using System.Globalization;
using Atlasiv.Core.Orders;

namespace Atlasiv.Core.Replay;

/// <summary>
/// The ordered log of validated orders for each phase (spec section 11). The simulation run reads
/// a phase's orders from it, so a replay is the initial state plus the world seed plus this log.
/// </summary>
/// <remarks>
/// <para>
/// Whatever order orders are added in, a phase's orders always come back in canonical order (by
/// issuer, then sequence number), so no system ever sees them in an order that depends on how the
/// log was written (spec section 6: simultaneous orders never resolve by internal list order).
/// </para>
/// <para>
/// J6a builds only what the run needs. J8 adds the campaign manifest at the head of the log, its
/// serialization and the replay verifier.
/// </para>
/// </remarks>
public sealed class InputLog
{
    private static readonly IReadOnlyList<Order> NoOrders = [];

    // Looked up by (turn, phase) only, never iterated (hard rule 5).
    private readonly Dictionary<(long Turn, int Phase), List<Order>> _byPhase = [];

    /// <summary>Records <paramref name="order"/> for phase <paramref name="phase"/> of turn <paramref name="turn"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The turn or phase is below 1.</exception>
    /// <exception cref="ArgumentException">The phase already has an order with the same issuer and sequence number.</exception>
    public void Add(long turn, int phase, Order order)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(turn, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(phase, 1);
        ArgumentNullException.ThrowIfNull(order);

        if (!_byPhase.TryGetValue((turn, phase), out var orders))
        {
            orders = [];
            _byPhase.Add((turn, phase), orders);
        }

        var index = orders.BinarySearch(order, CanonicalOrder.Instance);
        if (index >= 0)
        {
            throw new ArgumentException(
                string.Create(
                    CultureInfo.InvariantCulture,
                    $"Turn {turn}, phase {phase} already has order {order.Sequence} from issuer {order.IssuerId}."),
                nameof(order));
        }

        orders.Insert(~index, order);
    }

    /// <summary>The orders for phase <paramref name="phase"/> of turn <paramref name="turn"/>, by issuer and then sequence number.</summary>
    public IReadOnlyList<Order> OrdersFor(long turn, int phase) =>
        _byPhase.TryGetValue((turn, phase), out var orders) ? orders.AsReadOnly() : NoOrders;

    private sealed class CanonicalOrder : IComparer<Order>
    {
        public static readonly CanonicalOrder Instance = new();

        public int Compare(Order? x, Order? y)
        {
            ArgumentNullException.ThrowIfNull(x);
            ArgumentNullException.ThrowIfNull(y);
            var c = x.IssuerId.CompareTo(y.IssuerId);
            return c != 0 ? c : x.Sequence.CompareTo(y.Sequence);
        }
    }
}
