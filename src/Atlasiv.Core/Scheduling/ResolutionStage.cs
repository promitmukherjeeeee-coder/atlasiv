using System;
using System.Globalization;

namespace Atlasiv.Core.Scheduling;

/// <summary>
/// A stage: a numbered resolution slot. Phase stages 1 to 11 run in every phase; turn-end
/// stages 1 to 10 run once at the end of every turn (Pass 1 spec section 6, Terms). A stage
/// is not a step (a step is a turn length).
/// </summary>
/// <remarks>
/// <para>
/// Stages order by kind and then number, with every turn-end stage before every phase stage
/// (owner ruling, 26 Nov 2026; ENG-012). Inside the event ordering key this only matters on a
/// turn boundary, the one instant where both kinds can share a timestamp, and there spec
/// section 3 runs the old turn's turn-end stages before the new turn's first phase.
/// </para>
/// <para>
/// The default value is not a valid stage (<see cref="IsValid"/>).
/// </para>
/// </remarks>
public readonly record struct ResolutionStage : IComparable<ResolutionStage>
{
    /// <summary>The number of phase stages, from snapshotting state to publishing the player-visible state.</summary>
    public const int PhaseStageCount = 11;

    /// <summary>The number of turn-end stages, from climate and water to records, summaries and snapshots.</summary>
    public const int TurnEndStageCount = 10;

    // 1 to 10 for turn-end stages 1 to 10, then 11 to 21 for phase stages 1 to 11; 0 is invalid.
    private readonly int _rank;

    private ResolutionStage(int rank) => _rank = rank;

    /// <summary>True for a turn-end stage, false for a phase stage.</summary>
    public bool IsTurnEnd => _rank is >= 1 and <= TurnEndStageCount;

    /// <summary>The stage's number within its kind: 1 to 11 for a phase stage, 1 to 10 for a turn-end stage.</summary>
    public int Number => IsTurnEnd ? _rank : _rank - TurnEndStageCount;

    /// <summary>False only for the default value, which no factory can produce.</summary>
    public bool IsValid => _rank != 0;

    /// <summary>Phase stage <paramref name="number"/>, 1 to 11.</summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="number"/> is outside 1 to 11.</exception>
    public static ResolutionStage Phase(int number)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(number, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(number, PhaseStageCount);
        return new ResolutionStage(TurnEndStageCount + number);
    }

    /// <summary>Turn-end stage <paramref name="number"/>, 1 to 10.</summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="number"/> is outside 1 to 10.</exception>
    public static ResolutionStage TurnEnd(int number)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(number, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(number, TurnEndStageCount);
        return new ResolutionStage(number);
    }

    /// <summary>Orders stages: turn-end stages 1 to 10, then phase stages 1 to 11.</summary>
    public int CompareTo(ResolutionStage other) => _rank.CompareTo(other._rank);

    /// <summary>Whether <paramref name="left"/> resolves before <paramref name="right"/>.</summary>
    public static bool operator <(ResolutionStage left, ResolutionStage right) => left._rank < right._rank;

    /// <summary>Whether <paramref name="left"/> resolves after <paramref name="right"/>.</summary>
    public static bool operator >(ResolutionStage left, ResolutionStage right) => left._rank > right._rank;

    /// <summary>Whether <paramref name="left"/> is the same as or resolves before <paramref name="right"/>.</summary>
    public static bool operator <=(ResolutionStage left, ResolutionStage right) => left._rank <= right._rank;

    /// <summary>Whether <paramref name="left"/> is the same as or resolves after <paramref name="right"/>.</summary>
    public static bool operator >=(ResolutionStage left, ResolutionStage right) => left._rank >= right._rank;

    /// <summary>The stage as text, such as "phase stage 4" or "turn-end stage 10".</summary>
    public override string ToString() =>
        !IsValid ? "no stage"
        : (IsTurnEnd ? "turn-end stage " : "phase stage ") + Number.ToString(CultureInfo.InvariantCulture);
}
