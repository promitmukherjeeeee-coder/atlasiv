using System;

namespace Atlasiv.Core.Numerics;

/// <summary>
/// A simulation error: an authoritative calculation was given an impossible input, such as
/// the logarithm of zero, the square root of a negative number, or a negative rate or time.
/// Like <see cref="SimulationOverflowException"/>, it must stop the commit of the phase that
/// raised it (owner decision, 25 Nov 2026); the input is never clipped to a valid value.
/// </summary>
/// <remarks>
/// It derives from <see cref="ArgumentOutOfRangeException"/>, because the bad value is an
/// argument outside the function's domain. A commit boundary (Pass 2 onward) stops on this
/// type and on <see cref="OverflowException"/> alike (ENG-004).
/// </remarks>
public sealed class SimulationDomainException : ArgumentOutOfRangeException
{
    /// <summary>Creates the error with no message.</summary>
    public SimulationDomainException()
    {
    }

    /// <summary>Creates the error with a message describing the impossible input.</summary>
    public SimulationDomainException(string message)
        : base(null, message)
    {
    }

    /// <summary>Creates the error with a message and the error that caused it.</summary>
    public SimulationDomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>Creates the error naming the argument, its value and the rule it broke.</summary>
    public SimulationDomainException(string paramName, object? actualValue, string message)
        : base(paramName, actualValue, message)
    {
    }
}
