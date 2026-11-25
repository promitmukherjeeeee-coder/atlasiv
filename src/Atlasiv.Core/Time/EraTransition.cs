using System.Collections.Generic;

namespace Atlasiv.Core.Time;

/// <summary>
/// An era-transition event: the announcement, with its preview, of a change of step (Pass 1
/// spec section 4, clarification 10). It is issued when the switch is decided, for faster and
/// slower switches alike, and the switch is then committed. It is informational, so Advance
/// Until does not stop for it unless the player adds it to the filters.
/// </summary>
/// <remarks>
/// The preview lists the old and new step, the date the switch takes effect, the new phase
/// count and the first new turn's phase dates. Later passes add game consequences. Pass 1 has
/// no event queue yet: the scheduler (J6) puts this on the queue (J5) with the informational
/// interruption class.
/// </remarks>
/// <param name="OldStep">The step before the switch.</param>
/// <param name="AnnouncedAt">The turn boundary at which the switch was decided and announced.</param>
/// <param name="EffectiveTurn">The number of the first turn on the new step.</param>
/// <param name="FirstTurn">The first turn on the new step.</param>
public sealed record EraTransition(TurnStep OldStep, Instant AnnouncedAt, long EffectiveTurn, TurnDates FirstTurn)
{
    /// <summary>The step after the switch.</summary>
    public TurnStep NewStep => FirstTurn.Step;

    /// <summary>When the switch takes effect: the first new turn's start.</summary>
    public Instant EffectiveAt => FirstTurn.Start;

    /// <summary>The number of phases in each turn on the new step.</summary>
    public int NewPhaseCount => FirstTurn.PhaseCount;

    /// <summary>The first new turn's phases, in order, each as [start, end).</summary>
    public IReadOnlyList<TimeInterval> FirstTurnPhases
    {
        get
        {
            var phases = new TimeInterval[FirstTurn.PhaseCount];
            for (var i = 0; i < phases.Length; i++)
            {
                phases[i] = FirstTurn.Phase(i + 1);
            }

            return phases;
        }
    }
}
