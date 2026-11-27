using System;
using System.Collections.Generic;
using Atlasiv.Core.Numerics;

namespace Atlasiv.Core.Integration;

/// <summary>
/// The propagator cache (spec section 10, clarification 18, ADR-011): propagators keyed by regime
/// and exact elapsed seconds. The first time a regime is used, every legal turn and phase length
/// (<see cref="LegalLengths"/>) is computed for it in advance; any other length a run covers, such
/// as 6 years on the 2-year step, is computed on first use. Each is then kept.
/// </summary>
/// <remarks>
/// Because the source is a pure function, a kept value and a fresh computation are identical, so
/// whether the cache is cold, warm or cleared never changes results or hashes. The cache is not
/// part of the world state. It is only looked up, never iterated (hard rule 5).
/// </remarks>
/// <typeparam name="TRegime">The regime and parameters; equal regimes must give equal propagators.</typeparam>
/// <typeparam name="TPropagator">The propagator.</typeparam>
public sealed class PropagatorCache<TRegime, TPropagator>
    where TRegime : notnull, IEquatable<TRegime>
{
    private readonly IPropagatorSource<TRegime, TPropagator> _source;
    private readonly Dictionary<(TRegime Regime, long Seconds), TPropagator> _kept = [];
    private readonly HashSet<TRegime> _precomputed = [];

    /// <summary>Creates an empty cache over <paramref name="source"/>.</summary>
    public PropagatorCache(IPropagatorSource<TRegime, TPropagator> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        _source = source;
    }

    /// <summary>How many propagators are kept.</summary>
    public int Count => _kept.Count;

    /// <summary>The propagator for <paramref name="regime"/> over <paramref name="elapsedSeconds"/> seconds.</summary>
    /// <exception cref="SimulationDomainException"><paramref name="elapsedSeconds"/> is negative.</exception>
    public TPropagator For(TRegime regime, long elapsedSeconds)
    {
        if (elapsedSeconds < 0)
        {
            throw new SimulationDomainException(nameof(elapsedSeconds), elapsedSeconds, "An elapsed time cannot be negative.");
        }

        if (_precomputed.Add(regime))
        {
            foreach (var length in LegalLengths.Seconds)
            {
                Keep(regime, length);
            }
        }

        return Keep(regime, elapsedSeconds);
    }

    /// <summary>Forgets every kept propagator. Results stay the same; they are computed again when next needed.</summary>
    public void Clear()
    {
        _kept.Clear();
        _precomputed.Clear();
    }

    private TPropagator Keep(TRegime regime, long elapsedSeconds)
    {
        if (!_kept.TryGetValue((regime, elapsedSeconds), out var propagator))
        {
            propagator = _source.Compute(regime, elapsedSeconds);
            _kept.Add((regime, elapsedSeconds), propagator);
        }

        return propagator;
    }
}
