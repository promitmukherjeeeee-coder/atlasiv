using System;
using System.Collections.Generic;
using System.Globalization;
using Atlasiv.Core.Events;
using Atlasiv.Core.Orders;
using Atlasiv.Core.Time;

namespace Atlasiv.Core.Scheduling;

/// <summary>
/// The simulation run: the scheduler that resolves phases and turn ends in order, running each
/// registered system in its stage when its natural period is due and resolving the event queue,
/// and records what happened as a <see cref="SimulationTimeline"/> (spec sections 3 to 6).
/// </summary>
/// <remarks>
/// <para>Each phase [start, end):</para>
/// <list type="number">
/// <item>Phase stages 1 to 11 run in order at the phase's start. In each stage, the due systems run
/// in ascending ID order, then the events queued for that instant and stage (or an earlier stage)
/// resolve.</item>
/// <item>Then every event timestamped later in the phase resolves, in event-key order.</item>
/// </list>
/// <para>
/// At the turn end, turn-end stages 1 to 10 run the same way at the turn's end instant. Only then
/// does the step controller start the next turn, so the ending turn always finishes its turn end
/// before any event of the next turn resolves (spec section 3).
/// </para>
/// <para>
/// A system is due when its period has a boundary after the end of the time it last covered and
/// no later than the end of this opportunity: the phase's end in a phase stage, the turn's end in
/// a turn-end stage. When it runs it covers everything since its last run in one pass (spec
/// clarification 8; owner ruling of 26 Nov 2026 for phase stages, ENG-013).
/// </para>
/// <para>
/// The run can be stopped and continued: <see cref="RunTurns"/> may stop early, after a phase or
/// turn end, and the next call carries on from there.
/// </para>
/// </remarks>
public sealed class SimulationRun
{
    private static readonly IReadOnlyList<Order> NoOrders = [];

    private readonly SimulationSetup _setup;
    private readonly StepController _controller;

    // Systems in ascending ID order, with what the run tracks for each (same index).
    private readonly ISimulationSystem[] _systems;
    private readonly SystemRegistration[] _registrations;
    private readonly uint[] _ids;
    private readonly Instant[] _coveredUntil;
    private readonly Instant[] _nextBoundary;

    // Indices into _systems for each stage, ascending ID. [n - 1] is stage n.
    private readonly int[][] _phaseStageMembers;
    private readonly int[][] _turnEndStageMembers;

    private readonly EventQueue<EventPayload> _queue = new();

    // One event-ID sequence per system for the current turn and slot, made on first use; [0] is the clock's.
    private readonly EventIdSequence?[] _sequences;

    // The next phase of the current turn to run; PhaseCount + 1 means the turn end is next.
    private int _nextPhase = 1;

    // Where the current slot is.
    private long _slotTurn;
    private TurnSlot _slotKind;
    private TurnDates _slotTurnDates;

    // The earliest point a new event may be scheduled at: (time, stage) while systems run, or after
    // the event being resolved.
    private Instant _cursorTime;
    private ResolutionStage _cursorStage;
    private EventKey? _cursorEvent;

    // What the current slot has done.
    private List<SystemRun> _runs = [];
    private List<SimulationEvent<EventPayload>> _resolved = [];
    private IStopFilter? _stopFilter;
    private SimulationEvent<EventPayload>? _stoppedBy;

    /// <summary>Starts a run at the epoch, on the candidate step the leading edge gives there.</summary>
    /// <exception cref="ArgumentException">Two systems share an ID.</exception>
    public SimulationRun(SimulationSetup setup)
    {
        ArgumentNullException.ThrowIfNull(setup);
        ArgumentNullException.ThrowIfNull(setup.LeadingEdge);
        ArgumentNullException.ThrowIfNull(setup.Systems);
        ArgumentNullException.ThrowIfNull(setup.InputLog);
        _setup = setup;

        // Read each registration once, then order everything by ascending system ID (hard rule 5).
        var count = setup.Systems.Count;
        var given = new (ISimulationSystem System, SystemRegistration Registration)[count];
        _ids = new uint[count];
        for (var i = 0; i < count; i++)
        {
            var system = setup.Systems[i] ?? throw new ArgumentException("A registered system is null.", nameof(setup));
            var registration = system.Registration ?? throw new ArgumentException("A system has no registration.", nameof(setup));
            given[i] = (system, registration);
            _ids[i] = registration.SystemId;
        }

        Array.Sort(_ids, given);
        _systems = new ISimulationSystem[count];
        _registrations = new SystemRegistration[count];
        for (var i = 0; i < count; i++)
        {
            (_systems[i], _registrations[i]) = given[i];
            if (i > 0 && _ids[i] == _ids[i - 1])
            {
                throw new ArgumentException(
                    "Two systems share ID " + _ids[i].ToString(CultureInfo.InvariantCulture) + ".", nameof(setup));
            }
        }

        _coveredUntil = new Instant[count];
        _nextBoundary = new Instant[count];
        for (var i = 0; i < count; i++)
        {
            _coveredUntil[i] = Instant.Epoch;
            _nextBoundary[i] = _registrations[i].Period.NextBoundaryAfter(Instant.Epoch);
        }

        _phaseStageMembers = Members(stage => !stage.IsTurnEnd, ResolutionStage.PhaseStageCount);
        _turnEndStageMembers = Members(stage => stage.IsTurnEnd, ResolutionStage.TurnEndStageCount);
        _sequences = new EventIdSequence?[count + 1];
        _controller = new StepController(setup.LeadingEdge, setup.Undertaking);
    }

    /// <summary>The current turn's number.</summary>
    public long TurnNumber => _controller.TurnNumber;

    /// <summary>The current turn's step and dates.</summary>
    public TurnDates Turn => _controller.Turn;

    /// <summary>
    /// Runs phases and turn ends until <paramref name="turnEnds"/> more turn ends have completed,
    /// or until <paramref name="stopFilter"/> stops on a resolved event, in which case the run
    /// finishes that phase or turn end and stops there. Returns what happened.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="turnEnds"/> is negative.</exception>
    public SimulationTimeline RunTurns(long turnEnds, IStopFilter? stopFilter = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(turnEnds);
        _stopFilter = stopFilter;
        _stoppedBy = null;

        var entries = new List<TimelineEntry>();
        long completed = 0;
        while (completed < turnEnds && _stoppedBy is null)
        {
            var turn = _controller.Turn;
            if (_nextPhase <= turn.PhaseCount)
            {
                entries.Add(RunPhase(_controller.TurnNumber, turn, _nextPhase));
                _nextPhase++;
            }
            else
            {
                entries.Add(RunTurnEnd(_controller.TurnNumber, turn));
                _nextPhase = 1;
                completed++;
            }
        }

        _stopFilter = null;
        return new SimulationTimeline(entries, _stoppedBy);
    }

    internal StableEventId Schedule(
        SystemContext context, Instant at, ResolutionStage stage, int priority, InterruptionClass interruption, EventPayload payload)
    {
        if (context.Turn != _slotTurn || context.Slot != _slotKind)
        {
            throw new InvalidOperationException("This SystemContext belongs to a call that has finished.");
        }

        if (!stage.IsValid)
        {
            throw new ArgumentException("An event needs one of the 21 stages.", nameof(stage));
        }

        if (stage.IsTurnEnd && at != _slotTurnDates.End)
        {
            throw new ArgumentException(
                "A turn-end-stage event must be timestamped at the current turn's end (" + _slotTurnDates.End + "), not " + at + ".",
                nameof(at));
        }

        var key = new EventKey(at, stage, priority, NextId(context.SystemId));
        if (_cursorEvent is { } resolving ? key <= resolving : (at, stage).CompareTo((_cursorTime, _cursorStage)) < 0)
        {
            throw new ArgumentException(
                "Event " + key + " would resolve before the point the run has reached; events can only be scheduled for later.",
                nameof(at));
        }

        _queue.Enqueue(new SimulationEvent<EventPayload>(key, interruption, payload));
        return key.Id;
    }

    private TimelineEntry RunPhase(long turnNumber, TurnDates turn, int phase)
    {
        var span = turn.Phase(phase);
        BeginSlot(turnNumber, TurnSlot.Phase(phase), turn, span.Start);
        var orders = _setup.InputLog.OrdersFor(turnNumber, phase);

        for (var n = 1; n <= ResolutionStage.PhaseStageCount; n++)
        {
            var stage = ResolutionStage.Phase(n);
            RunStage(stage, _phaseStageMembers[n - 1], span, span.Start, span.End, orders);
            var start = span.Start;
            ResolveWhile(key => key.Timestamp == start && key.Stage <= stage, span, orders);
        }

        var end = span.End;
        ResolveWhile(key => key.Timestamp < end, span, orders);
        return EndSlot(span, null);
    }

    private TimelineEntry RunTurnEnd(long turnNumber, TurnDates turn)
    {
        var end = turn.End;
        BeginSlot(turnNumber, TurnSlot.TurnEnd, turn, end);

        for (var n = 1; n <= ResolutionStage.TurnEndStageCount; n++)
        {
            var stage = ResolutionStage.TurnEnd(n);
            RunStage(stage, _turnEndStageMembers[n - 1], turn.Span, end, end, NoOrders);
            ResolveWhile(key => key.Timestamp == end && key.Stage <= stage, turn.Span, NoOrders);
        }

        // The turn end is over: the controller reads the leading edge at the boundary and starts the next turn.
        var announced = _controller.AdvanceTurn();
        if (announced is not null)
        {
            // The announcement resolves in phase stage 4 (scheduled events and communications) of the turn that starts here.
            var key = new EventKey(announced.AnnouncedAt, ResolutionStage.Phase(4), 0, NextId(SystemRegistration.ClockSystemId));
            _queue.Enqueue(new SimulationEvent<EventPayload>(key, InterruptionClass.Informational, new EraTransitionAnnounced(announced)));
        }

        return EndSlot(turn.Span, announced);
    }

    private void BeginSlot(long turnNumber, TurnSlot slot, TurnDates turn, Instant now)
    {
        if (_queue.TryPeek(out var next) && next.Key.Timestamp < now)
        {
            throw new InvalidOperationException("Event " + next.Key + " was left unresolved before " + now + ".");
        }

        _slotTurn = turnNumber;
        _slotKind = slot;
        _slotTurnDates = turn;
        Array.Clear(_sequences);
        _runs = [];
        _resolved = [];
    }

    private TimelineEntry EndSlot(TimeInterval span, EraTransition? announced) =>
        new(_slotTurn, _slotKind, _slotTurnDates.Step, span, _runs, _resolved, announced);

    private void RunStage(
        ResolutionStage stage, int[] members, TimeInterval slotSpan, Instant now, Instant coverEnd, IReadOnlyList<Order> orders)
    {
        _cursorTime = now;
        _cursorStage = stage;
        _cursorEvent = null;

        foreach (var index in members)
        {
            if (_nextBoundary[index] > coverEnd)
            {
                continue;
            }

            var covered = new TimeInterval(_coveredUntil[index], coverEnd);
            var context = Context(_ids[index], slotSpan, now, stage, orders);
            _systems[index].Run(context, covered);
            _runs.Add(new SystemRun(stage, _ids[index], covered));
            _coveredUntil[index] = coverEnd;
            _nextBoundary[index] = _registrations[index].Period.NextBoundaryAfter(coverEnd);
        }
    }

    private void ResolveWhile(Func<EventKey, bool> due, TimeInterval slotSpan, IReadOnlyList<Order> orders)
    {
        while (_queue.TryPeek(out var next) && due(next.Key))
        {
            _queue.Dequeue();
            _cursorEvent = next.Key;
            _resolved.Add(next);
            if (_stoppedBy is null && _stopFilter is not null && _stopFilter.StopsOn(next))
            {
                _stoppedBy = next;
            }

            var systemId = next.Key.Id.SystemId;
            if (systemId != SystemRegistration.ClockSystemId)
            {
                var context = Context(systemId, slotSpan, next.Key.Timestamp, next.Key.Stage, orders);
                _systems[IndexOf(systemId)].Resolve(next, context);
            }
        }
    }

    private SystemContext Context(uint systemId, TimeInterval slotSpan, Instant now, ResolutionStage stage, IReadOnlyList<Order> orders) =>
        new(this, systemId, _slotTurn, _slotKind, _slotTurnDates.Step, _slotTurnDates.Span, slotSpan, now, stage, orders, _setup.WorldSeed);

    private StableEventId NextId(uint systemId)
    {
        var slot = systemId == SystemRegistration.ClockSystemId ? 0 : IndexOf(systemId) + 1;
        var sequence = _sequences[slot] ??= new EventIdSequence(systemId, _slotTurn, _slotKind);
        return sequence.Next();
    }

    private int IndexOf(uint systemId)
    {
        var index = Array.BinarySearch(_ids, systemId);
        if (index < 0)
        {
            throw new InvalidOperationException("No system with ID " + systemId.ToString(CultureInfo.InvariantCulture) + " is registered.");
        }

        return index;
    }

    private int[][] Members(Func<ResolutionStage, bool> ofKind, int stageCount)
    {
        var lists = new List<int>[stageCount];
        for (var n = 0; n < stageCount; n++)
        {
            lists[n] = [];
        }

        for (var i = 0; i < _registrations.Length; i++)
        {
            var stage = _registrations[i].Stage;
            if (ofKind(stage))
            {
                lists[stage.Number - 1].Add(i);
            }
        }

        var members = new int[stageCount][];
        for (var n = 0; n < stageCount; n++)
        {
            members[n] = lists[n].ToArray();
        }

        return members;
    }
}
