using System;
using System.Globalization;

namespace Atlasiv.Core.Numerics;

/// <summary>
/// A running total of base-unit amounts held in a 128-bit integer: the type for sums,
/// aggregates and exposures (Pass 1 spec section 8, clarification 14). Single amounts are
/// 64-bit, but totals of many of them, and exposures (quantity × price), are not bounded by
/// 64 bits, so they accumulate here exactly and are read back as 64 bits only where a
/// 64-bit value is required.
/// </summary>
/// <remarks>
/// Integer addition is exact, so the total does not depend on the order of the entries or on
/// how partial totals are grouped and combined (for example across threads). Leaving the
/// 128-bit range is a <see cref="SimulationOverflowException"/>; with 64-bit amounts that
/// takes more than 2^64 entries, and with exposures at least two products near 2^126.
/// The default value, <see cref="Zero"/>, is an empty total.
/// </remarks>
public readonly struct Accumulator : IEquatable<Accumulator>
{
    private readonly Int128 _total;

    private Accumulator(Int128 total) => _total = total;

    /// <summary>An empty total.</summary>
    public static Accumulator Zero => default;

    /// <summary>The exact total.</summary>
    public Int128 Total => _total;

    /// <summary>Adds an amount; a negative amount (a debit or a liability) subtracts.</summary>
    /// <exception cref="SimulationOverflowException">The total leaves the 128-bit range.</exception>
    public Accumulator Add(long amount) => Sum(amount, "Accumulator.Add");

    /// <summary>Adds the exact product <paramref name="quantity"/> × <paramref name="price"/>, for example an exposure.</summary>
    /// <exception cref="SimulationOverflowException">The total leaves the 128-bit range.</exception>
    public Accumulator AddProduct(long quantity, long price) =>
        Sum((Int128)quantity * price, "Accumulator.AddProduct"); // at most 2^126 in size, always exact

    /// <summary>Adds another total, for example a partial total built separately.</summary>
    /// <exception cref="SimulationOverflowException">The total leaves the 128-bit range.</exception>
    public Accumulator Add(Accumulator other) => Sum(other._total, "Accumulator.Add");

    /// <summary>The total as a 64-bit value.</summary>
    /// <exception cref="SimulationOverflowException">The total is outside the 64-bit range.</exception>
    public long ToInt64() => WideMath.ToInt64(_total, "Accumulator.ToInt64");

    private Accumulator Sum(Int128 value, string operation)
    {
        // A sum leaves the range exactly when both terms have the same sign and the
        // wrapped result has the other sign. The wrap is computed only to test that.
        var sum = unchecked(_total + value);
        if (((_total ^ sum) & (value ^ sum)) < 0)
        {
            throw new SimulationOverflowException(string.Create(
                CultureInfo.InvariantCulture,
                $"{operation} overflowed the 128-bit range: {_total} plus {value}."));
        }

        return new Accumulator(sum);
    }

    /// <inheritdoc/>
    public bool Equals(Accumulator other) => _total == other._total;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Accumulator other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => _total.GetHashCode();

    /// <summary>The exact total in decimal.</summary>
    public override string ToString() => _total.ToString(CultureInfo.InvariantCulture);

    /// <summary>True when both totals are exactly equal.</summary>
    public static bool operator ==(Accumulator left, Accumulator right) => left.Equals(right);

    /// <summary>True when the totals differ.</summary>
    public static bool operator !=(Accumulator left, Accumulator right) => !left.Equals(right);
}
