using System;
using Atlasiv.Core.Randomness;
using Atlasiv.Core.Time;

namespace Atlasiv.Core.Scheduling;

/// <summary>
/// One substep of a system's turn-end run: its number, its dates and its random streams
/// (spec section 10, clarification 3).
/// </summary>
public sealed class Substep
{
    private readonly Func<ulong, DrawAddress> _unsplitDraws;

    internal Substep(int index, TimeInterval span, Func<ulong, DrawAddress> unsplitDraws)
    {
        Index = index;
        Span = span;
        _unsplitDraws = unsplitDraws;
    }

    /// <summary>The substep's number, counting from 0 (ENG-008 decision 4).</summary>
    public int Index { get; }

    /// <summary>The substep's dates: whole days, contiguous with its neighbours.</summary>
    public TimeInterval Span { get; }

    /// <summary>
    /// The random stream for <paramref name="entityId"/> in this substep: the turn end's stream
    /// with this substep's number, so no two substeps share a draw. Substep 0's streams are the
    /// unsplit turn end's (<see cref="SystemContext.Draws"/>), as ENG-008 decision 4 fixes, so a
    /// system that draws both ways in one run must use different draw indices or entity IDs.
    /// </summary>
    public DrawAddress Draws(ulong entityId) => _unsplitDraws(entityId) with { Substep = Index };
}
