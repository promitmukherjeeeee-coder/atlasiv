using Atlasiv.Core.Events;

namespace Atlasiv.Core.Orders;

/// <summary>
/// Decides which resolved events stop a run. A run that resolves a stopping event finishes the
/// phase (or turn end) it is in and then stops, so control always returns at a phase boundary
/// (spec clarification 9). J7 builds Advance Until's default stop list and filters on this.
/// </summary>
public interface IStopFilter
{
    /// <summary>Whether <paramref name="resolved"/> stops the run at the end of the current phase or turn end.</summary>
    bool StopsOn(SimulationEvent<EventPayload> resolved);
}
