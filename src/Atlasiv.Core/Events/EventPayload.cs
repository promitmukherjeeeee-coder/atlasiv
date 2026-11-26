namespace Atlasiv.Core.Events;

/// <summary>
/// What an event carries. Each system defines its own payload records by deriving from this one,
/// so the simulation run's single event queue holds every system's events while keeping each
/// payload typed.
/// </summary>
public abstract record EventPayload;
