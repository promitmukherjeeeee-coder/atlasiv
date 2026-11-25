using System;

namespace Atlasiv.Core.Time;

/// <summary>
/// The step controller (Pass 1 spec section 4): at every turn boundary it reads the leading
/// edge, picks the candidate step, and decides whether the next turn runs on a new step.
/// </summary>
/// <remarks>
/// <para>Rules (spec section 4 and clarifications 4 to 7, 10 and 16):</para>
/// <list type="bullet">
/// <item>Turn 1 starts at the epoch on the candidate step at once. The epoch counts as the first switch.</item>
/// <item>The count tracks direction, not the exact candidate: it grows while the candidate stays
/// faster (or stays slower) than the current step, and resets when the candidate equals the
/// current step or changes direction.</item>
/// <item>A switch is decided when the count reaches 10 (faster) or 20 (slower) and at least 20
/// turns have run since the last switch took effect. A count that completes sooner fires at the
/// first turn the gap allows, if it still holds then. A switch is always one step.</item>
/// <item>A faster switch takes effect at once. A slower one takes effect at the first turn
/// boundary that is a valid start for the new step (1 January or 1 July); turns until then
/// continue on the old step.</item>
/// <item>Once announced, a switch is committed. The gap counts from the turn it takes effect.</item>
/// <item>Turns count from 1 at the epoch and never reset.</item>
/// </list>
/// </remarks>
public sealed class StepController
{
    /// <summary>Consecutive turns the candidate must stay faster before the step goes faster.</summary>
    public const int TurnsBeforeFaster = 10;

    /// <summary>Consecutive turns the candidate must stay slower before the step goes slower.</summary>
    public const int TurnsBeforeSlower = 20;

    /// <summary>The fewest turns that run on a step before another switch may be announced.</summary>
    public const int MinimumTurnsBetweenSwitches = 20;

    private readonly ILeadingEdge _leadingEdge;
    private readonly ReferenceUndertaking _undertaking;

    // The turn on which the current step took effect (1 for the epoch).
    private long _lastSwitchTurn = 1;

    // Which way the candidate has pointed, and for how many consecutive turns: -1 slower, +1 faster, 0 neither.
    private int _direction;
    private int _count;

    /// <summary>Starts at the epoch with the uncalibrated reference undertaking.</summary>
    public StepController(ILeadingEdge leadingEdge)
        : this(leadingEdge, ReferenceUndertaking.Uncalibrated)
    {
    }

    /// <summary>Starts at the epoch: turn 1 runs on the candidate step at once (clarification 4).</summary>
    public StepController(ILeadingEdge leadingEdge, ReferenceUndertaking undertaking)
    {
        ArgumentNullException.ThrowIfNull(leadingEdge);
        _leadingEdge = leadingEdge;
        _undertaking = undertaking;
        TurnNumber = 1;
        Turn = TurnDates.Starting(Candidate(Instant.Epoch), Instant.Epoch);
    }

    /// <summary>The current turn's number, counting from 1 at the epoch and never reset.</summary>
    public long TurnNumber { get; private set; }

    /// <summary>The current turn's step and dates.</summary>
    public TurnDates Turn { get; private set; }

    /// <summary>The announced switch still waiting to take effect, if any.</summary>
    public EraTransition? PendingSwitch { get; private set; }

    /// <summary>
    /// Ends the current turn and moves to the next: reads the leading edge at the boundary,
    /// updates the count, decides any switch, and starts the next turn on the step it runs on.
    /// </summary>
    /// <returns>The era-transition event announced at this boundary, if any.</returns>
    public EraTransition? EndTurn()
    {
        var boundary = Turn.End;
        var nextTurn = TurnNumber + 1;
        EraTransition? announced = null;

        if (PendingSwitch is null)
        {
            Observe(Candidate(boundary));
            if (CountComplete() && nextTurn - _lastSwitchTurn >= MinimumTurnsBetweenSwitches)
            {
                announced = Announce(boundary, nextTurn);
                PendingSwitch = announced;
            }
        }

        TurnNumber = nextTurn;
        if (PendingSwitch is { } pending && pending.EffectiveAt == boundary)
        {
            Turn = pending.FirstTurn;
            _lastSwitchTurn = TurnNumber;
            PendingSwitch = null;
        }
        else
        {
            Turn = Turn.Next;
        }

        return announced;
    }

    private void Observe(TurnStep candidate)
    {
        var direction = int.Sign(candidate - Turn.Step);
        _count = direction == 0 ? 0 : direction == _direction ? _count + 1 : 1;
        _direction = direction;
    }

    private bool CountComplete() =>
        _direction != 0 && _count >= (_direction > 0 ? TurnsBeforeFaster : TurnsBeforeSlower);

    private EraTransition Announce(Instant boundary, long nextTurn)
    {
        var newStep = Turn.Step + _direction;
        _direction = 0;
        _count = 0;

        // Walk the old step's turns to the first boundary that is a valid start for the new step.
        // A faster step accepts every boundary of a slower one, so it never waits; a slower one
        // waits at most 5 turns (1 month to 6 months, from August to January).
        var effective = boundary;
        var effectiveTurn = nextTurn;
        while (!TurnDates.IsValidStart(newStep, effective))
        {
            effective = TurnDates.Starting(Turn.Step, effective).End;
            effectiveTurn++;
        }

        return new EraTransition(Turn.Step, boundary, effectiveTurn, TurnDates.Starting(newStep, effective));
    }

    private TurnStep Candidate(Instant asOf) =>
        CandidateStep.For(_leadingEdge.StrategicSeconds(_undertaking, asOf));
}
