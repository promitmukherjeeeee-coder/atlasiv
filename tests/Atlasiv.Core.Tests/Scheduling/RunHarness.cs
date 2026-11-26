using System;
using System.Collections.Generic;
using Atlasiv.Core.Events;
using Atlasiv.Core.Replay;
using Atlasiv.Core.Scheduling;
using Atlasiv.Core.Time;

namespace Atlasiv.Core.Tests.Scheduling;

/// <summary>Shared helpers for driving the simulation run in tests.</summary>
internal static class RunHarness
{
    public const ulong Seed = 0x5EED;

    public static Instant Date(long year, int month, int day) => Instant.FromDate(new CalendarDate(year, month, day));

    public static TimeInterval Span(Instant start, Instant end) => new(start, end);

    /// <summary>A strategic time whose candidate is <paramref name="step"/>: exactly two of its turns.</summary>
    public static long TwoTurnsOf(TurnStep step) => 2 * StepTable.NominalSeconds(step);

    /// <summary>A run on one step throughout, with the given systems and an empty input log.</summary>
    public static SimulationRun Start(TurnStep step, params ISimulationSystem[] systems) =>
        Start(new ScriptedLeadingEdge(step), new InputLog(), systems);

    public static SimulationRun Start(ILeadingEdge edge, InputLog log, params ISimulationSystem[] systems) =>
        new(new SimulationSetup(Seed, edge, ReferenceUndertaking.Uncalibrated, systems, log));
}

/// <summary>
/// A leading edge that reports a fixed candidate step, which the test can change from a given
/// instant onward. Each reading is answered from the latest change at or before it.
/// </summary>
internal sealed class ScriptedLeadingEdge : ILeadingEdge
{
    private readonly List<(Instant From, TurnStep Step)> _script = [];

    public ScriptedLeadingEdge(TurnStep initial) => _script.Add((Instant.Epoch, initial));

    /// <summary>From <paramref name="from"/> on, the candidate is <paramref name="step"/>.</summary>
    public ScriptedLeadingEdge Then(Instant from, TurnStep step)
    {
        _script.Add((from, step));
        return this;
    }

    public long StrategicSeconds(ReferenceUndertaking undertaking, Instant asOf)
    {
        var step = _script[0].Step;
        foreach (var (from, s) in _script)
        {
            if (from <= asOf)
            {
                step = s;
            }
        }

        return RunHarness.TwoTurnsOf(step);
    }
}

/// <summary>
/// A test system whose run and event resolution are given by the test. With neither given it does
/// nothing when it runs; the timeline records its runs.
/// </summary>
internal sealed class ScriptedSystem(
    uint id,
    ResolutionStage stage,
    NaturalPeriod period,
    Action<SystemContext, TimeInterval>? onRun = null,
    Action<SimulationEvent<EventPayload>, SystemContext>? onResolve = null) : ISimulationSystem
{
    public SystemRegistration Registration { get; } = new(id, stage, period);

    public void Run(SystemContext context, TimeInterval covered) => onRun?.Invoke(context, covered);

    public void Resolve(SimulationEvent<EventPayload> resolved, SystemContext context)
    {
        if (onResolve is null)
        {
            throw new InvalidOperationException("This system was given no way to resolve events.");
        }

        onResolve(resolved, context);
    }
}

/// <summary>A test event payload carrying a label.</summary>
internal sealed record Note(string Text) : EventPayload;
