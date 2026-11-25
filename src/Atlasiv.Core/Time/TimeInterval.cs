using System;
using Atlasiv.Core.Numerics;

namespace Atlasiv.Core.Time;

/// <summary>
/// A stretch of clock time [start, end): it includes its start instant and excludes its end
/// instant (Pass 1 spec section 3, clarification 2). Back-to-back intervals therefore never
/// overlap, and an instant on a shared boundary belongs to the interval that starts there.
/// </summary>
/// <remarks>The default value is the empty interval at the epoch.</remarks>
public readonly record struct TimeInterval
{
    /// <summary>Creates the interval from <paramref name="start"/> (included) to <paramref name="end"/> (excluded).</summary>
    /// <exception cref="ArgumentOutOfRangeException">The end is before the start.</exception>
    public TimeInterval(Instant start, Instant end)
    {
        if (end < start)
        {
            throw new ArgumentOutOfRangeException(nameof(end), end, "An interval cannot end before it starts (" + start + ").");
        }

        Start = start;
        End = end;
    }

    /// <summary>The first instant in the interval.</summary>
    public Instant Start { get; }

    /// <summary>The first instant after the interval.</summary>
    public Instant End { get; }

    /// <summary>The exact length in seconds.</summary>
    /// <exception cref="SimulationOverflowException">The length exceeds the 64-bit range.</exception>
    public long Seconds => End.SecondsSince(Start);

    /// <summary>The length in rate years (<see cref="RateYear.FromSeconds"/>).</summary>
    /// <exception cref="SimulationOverflowException">The length exceeds the 64-bit or Q32 range.</exception>
    public Q32 RateYears => RateYear.FromSeconds(Seconds);

    /// <summary>Whether <paramref name="instant"/> is in [start, end).</summary>
    public bool Contains(Instant instant) => Start <= instant && instant < End;
}
