using System.Collections.Generic;
using Atlasiv.Core.Events;
using Atlasiv.Core.Integration;
using Atlasiv.Core.Orders;
using Atlasiv.Core.Randomness;
using Atlasiv.Core.Time;

namespace Atlasiv.Core.Scheduling;

/// <summary>
/// What a system sees while it runs or resolves one of its events: where the run is, the phase's
/// orders, its random streams, and a way to schedule events.
/// </summary>
/// <remarks>A context is valid only during the call it is passed to.</remarks>
public sealed class SystemContext
{
    private readonly SimulationRun _run;

    internal SystemContext(
        SimulationRun run,
        long callNumber,
        uint systemId,
        long turn,
        TurnSlot slot,
        TurnStep step,
        TimeInterval turnSpan,
        TimeInterval slotSpan,
        Instant now,
        ResolutionStage stage,
        IReadOnlyList<Order> orders,
        ulong worldSeed)
    {
        _run = run;
        CallNumber = callNumber;
        SystemId = systemId;
        Turn = turn;
        Slot = slot;
        Step = step;
        TurnSpan = turnSpan;
        SlotSpan = slotSpan;
        Now = now;
        Stage = stage;
        Orders = orders;
        WorldSeed = worldSeed;
    }

    // Which call into a system this context belongs to; the run refuses it once that call has returned.
    internal long CallNumber { get; }

    /// <summary>The system being called.</summary>
    public uint SystemId { get; }

    /// <summary>The current turn number.</summary>
    public long Turn { get; }

    /// <summary>The current phase, or the turn end.</summary>
    public TurnSlot Slot { get; }

    /// <summary>The current turn's step.</summary>
    public TurnStep Step { get; }

    /// <summary>The current turn's dates. Its end is where turn-end-stage events must be timestamped.</summary>
    public TimeInterval TurnSpan { get; }

    /// <summary>The current phase's dates, or at the turn end the whole turn's.</summary>
    public TimeInterval SlotSpan { get; }

    /// <summary>
    /// The instant being resolved: the phase's start while systems run in a phase, the turn's end
    /// at the turn end, or the event's timestamp while resolving an event.
    /// </summary>
    public Instant Now { get; }

    /// <summary>The stage being resolved: the system's own stage while it runs, or the event's while resolving one.</summary>
    public ResolutionStage Stage { get; }

    /// <summary>
    /// Every order for the current phase, all at once and in canonical order (by issuer, then
    /// sequence number), whatever order they were logged in. Empty at the turn end.
    /// </summary>
    public IReadOnlyList<Order> Orders { get; }

    /// <summary>The world seed.</summary>
    public ulong WorldSeed { get; }

    /// <summary>The random stream for <paramref name="entityId"/> in this system, turn and slot (substep 0).</summary>
    public DrawAddress Draws(ulong entityId) => new(WorldSeed, Turn, Slot, 0, SystemId, entityId);

    /// <summary>
    /// Splits <paramref name="interval"/>, all or part of the time this run covers, into <paramref name="count"/> substeps of whole days (see
    /// <see cref="Integration.SubstepDriver"/>), each with its own random streams. Asking for more than the
    /// run's substep budget is allowed and is logged in the timeline (spec section 10, clarification 13).
    /// </summary>
    /// <exception cref="System.InvalidOperationException">Not while this system runs in a turn-end stage (for example, while it resolves an event), or this context's call has finished.</exception>
    /// <exception cref="System.ArgumentException">The interval is not within the time this run covers, or does not start and end at 00:00.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="count"/> is below 1 or above the interval's number of days.</exception>
    public IReadOnlyList<Substep> Substeps(TimeInterval interval, int count) => _run.Substeps(this, interval, count);

    /// <summary>
    /// Runs <paramref name="problem"/> through the solver interface (spec section 10). If it reaches the
    /// iteration cap without converging, the result is the problem's fallback, the outcome says so, and
    /// the run logs it in the timeline.
    /// </summary>
    /// <exception cref="System.InvalidOperationException">This context's call has finished.</exception>
    /// <exception cref="Numerics.SimulationDomainException">The problem gave a negative residual.</exception>
    public SolverResult<TState> Solve<TState>(IIterativeProblem<TState> problem, SolverSettings settings) =>
        _run.Solve(this, problem, settings);

    /// <summary>
    /// Schedules an event created by this system and returns its stable ID. The event resolves
    /// when the run reaches its (timestamp, stage, priority, ID), and this system resolves it.
    /// </summary>
    /// <param name="at">When it happens. A turn-end-stage event must be at the current turn's end.</param>
    /// <param name="stage">The stage it resolves in.</param>
    /// <param name="priority">Lower numbers resolve first among events at the same instant and stage.</param>
    /// <param name="interruption">Whether it is informational or decision-critical.</param>
    /// <param name="payload">What it carries.</param>
    /// <exception cref="System.ArgumentException">The event would resolve before the point the run has reached, or is a turn-end-stage event not at the current turn's end.</exception>
    public StableEventId Schedule(Instant at, ResolutionStage stage, int priority, InterruptionClass interruption, EventPayload payload) =>
        _run.Schedule(this, at, stage, priority, interruption, payload);
}
