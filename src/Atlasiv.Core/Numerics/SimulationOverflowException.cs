using System;

namespace Atlasiv.Core.Numerics;

/// <summary>
/// A simulation error: an authoritative number left its representable range.
/// It must stop the commit of the phase that raised it; it is never caught and ignored,
/// and the value is never wrapped or clipped (Pass 1 spec section 8).
/// </summary>
/// <remarks>
/// It derives from <see cref="OverflowException"/>, so a commit boundary that stops on
/// <see cref="OverflowException"/> also stops on the plain checked-arithmetic overflows
/// that the whole build raises (ENG-003).
/// </remarks>
public sealed class SimulationOverflowException : OverflowException
{
    /// <summary>Creates the error with no message.</summary>
    public SimulationOverflowException()
    {
    }

    /// <summary>Creates the error with a message naming the operation that overflowed.</summary>
    public SimulationOverflowException(string message)
        : base(message)
    {
    }

    /// <summary>Creates the error with a message and the error that caused it.</summary>
    public SimulationOverflowException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
