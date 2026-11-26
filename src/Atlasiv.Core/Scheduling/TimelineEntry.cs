using System;
using System.Collections.Generic;
using Atlasiv.Core.Events;
using Atlasiv.Core.Time;

namespace Atlasiv.Core.Scheduling;

/// <summary>
/// What happened in one phase or one turn end of a simulation run: where it sits in the turn,
/// its dates, the systems that ran (in order), the events that resolved (in order), any solver
/// fallbacks and, at a turn end, any substep escalations and any step switch announced there.
/// </summary>
/// <remarks>A class, not a record: record equality would compare its lists by reference, not by contents.</remarks>
public sealed class TimelineEntry
{
    internal TimelineEntry(
        long turn,
        TurnSlot slot,
        TurnStep step,
        TimeInterval span,
        IReadOnlyList<SystemRun> runs,
        IReadOnlyList<SimulationEvent<EventPayload>> events,
        IReadOnlyList<SubstepEscalation> substepEscalations,
        IReadOnlyList<SolverFallback> solverFallbacks,
        EraTransition? announced)
    {
        Turn = turn;
        Slot = slot;
        Step = step;
        Span = span;
        Runs = runs;
        Events = events;
        SubstepEscalations = substepEscalations;
        SolverFallbacks = solverFallbacks;
        Announced = announced;
    }

    /// <summary>The turn number, counting from 1 at the epoch and never reset (spec clarification 17).</summary>
    public long Turn { get; }

    /// <summary>The phase (1 to <see cref="PhaseCount"/>) or the turn end.</summary>
    public TurnSlot Slot { get; }

    /// <summary>True for a turn end, false for a phase.</summary>
    public bool IsTurnEnd => Slot.IsTurnEnd;

    /// <summary>The turn's step.</summary>
    public TurnStep Step { get; }

    /// <summary>The number of phases in the turn.</summary>
    public int PhaseCount => StepTable.PhaseCount(Step);

    /// <summary>For a phase, the phase's dates; for a turn end, the whole turn's.</summary>
    public TimeInterval Span { get; }

    /// <summary>
    /// The instant at which the phase's stages start (the phase's start), or at which the turn
    /// end happens (the turn's end).
    /// </summary>
    public Instant At => IsTurnEnd ? Span.End : Span.Start;

    /// <summary>Every system run, in the order they ran.</summary>
    public IReadOnlyList<SystemRun> Runs { get; }

    /// <summary>Every event resolved, in the order it resolved.</summary>
    public IReadOnlyList<SimulationEvent<EventPayload>> Events { get; }

    /// <summary>Every request for more substeps than the budget, in the order they were made; always empty for a phase.</summary>
    public IReadOnlyList<SubstepEscalation> SubstepEscalations { get; }

    /// <summary>Every solver that reached its iteration cap and fell back, in the order they ran.</summary>
    public IReadOnlyList<SolverFallback> SolverFallbacks { get; }

    /// <summary>At a turn end, the step switch decided at the boundary after it, if any; always null for a phase.</summary>
    public EraTransition? Announced { get; }

    /// <summary>The state hash after this phase or turn end. Empty until J8 adds the canonical serializer.</summary>
    public UInt128? StateHash { get; internal set; }
}
