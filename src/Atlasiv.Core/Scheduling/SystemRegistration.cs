using System;

namespace Atlasiv.Core.Scheduling;

/// <summary>
/// What a system declares to the scheduler: its ID, its one stage and its natural period (spec
/// section 6). The scheduler calls the system only in that stage, and only when the period is
/// due.
/// </summary>
/// <remarks>
/// The spec also lists an update cadence. It is per-entity level of detail (Vision A2), and Pass 1
/// has no entities, so it waits for Pass 2 (owner ruling, 26 Nov 2026; ENG-013).
/// </remarks>
public sealed record SystemRegistration
{
    /// <summary>The system ID reserved for the clock, which creates era-transition events. No registered system may use it.</summary>
    public const uint ClockSystemId = 0;

    /// <summary>Registers system <paramref name="systemId"/> in <paramref name="stage"/> with <paramref name="period"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The ID is the clock's, or the stage or period is the default value.</exception>
    public SystemRegistration(uint systemId, ResolutionStage stage, NaturalPeriod period)
    {
        if (systemId == ClockSystemId)
        {
            throw new ArgumentOutOfRangeException(nameof(systemId), systemId, "System ID 0 is reserved for the clock.");
        }

        if (!stage.IsValid)
        {
            throw new ArgumentOutOfRangeException(nameof(stage), stage, "A system must register in one of the 21 stages.");
        }

        if (!period.IsValid)
        {
            throw new ArgumentOutOfRangeException(nameof(period), period, "A system needs a natural period; use NaturalPeriod.TurnOf or PhaseOf.");
        }

        SystemId = systemId;
        Stage = stage;
        Period = period;
    }

    /// <summary>The system's ID, used in stable event IDs and draw addresses. Systems in one stage run in ascending ID order.</summary>
    public uint SystemId { get; }

    /// <summary>The one stage the system runs in.</summary>
    public ResolutionStage Stage { get; }

    /// <summary>How much calendar time passes between the system's runs.</summary>
    public NaturalPeriod Period { get; }
}
