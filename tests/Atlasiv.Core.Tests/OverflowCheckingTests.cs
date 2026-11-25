using System;
using Xunit;

namespace Atlasiv.Core.Tests;

/// <summary>
/// Hard rule 2: overflow is an error, never a silent wrap. The whole build
/// compiles with checked arithmetic (ENG-003), so plain integer operations
/// throw on overflow unless code explicitly opts out with <c>unchecked</c>.
/// </summary>
public sealed class OverflowCheckingTests
{
    // Read from a field so the compiler cannot fold the overflow at compile time.
    private static long _max = long.MaxValue;
    private static int _intMax = int.MaxValue;

    [Fact]
    public void Adding_past_the_largest_long_throws()
    {
        Assert.Throws<OverflowException>(() => _max + 1);
    }

    [Fact]
    public void Multiplying_past_the_largest_int_throws()
    {
        Assert.Throws<OverflowException>(() => _intMax * 2);
    }

    [Fact]
    public void Narrowing_a_too_large_long_to_int_throws()
    {
        Assert.Throws<OverflowException>(() => (int)_max);
    }
}
