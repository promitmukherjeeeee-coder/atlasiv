using Atlasiv.Core.Events;
using Atlasiv.Core.Time;

namespace Atlasiv.Core.Scheduling;

/// <summary>
/// A system the scheduler runs: it registers in one stage with a natural period, runs when that
/// period is due, and resolves the events it created.
/// </summary>
public interface ISimulationSystem
{
    /// <summary>The system's ID, stage and natural period. Read once, when the run starts.</summary>
    SystemRegistration Registration { get; }

    /// <summary>
    /// Runs the system once, in its stage, covering <paramref name="covered"/>: all the time since
    /// its last run (or since the epoch), in one pass (spec clarification 8).
    /// </summary>
    void Run(SystemContext context, TimeInterval covered);

    /// <summary>Resolves <paramref name="resolved"/>, an event this system scheduled, at its timestamp and stage.</summary>
    void Resolve(SimulationEvent<EventPayload> resolved, SystemContext context);
}
