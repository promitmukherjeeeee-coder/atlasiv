using Atlasiv.Core.Numerics;
using BenchmarkDotNet.Attributes;

namespace Atlasiv.Benchmarks;

/// <summary>
/// Cost of the integer numerics primitives (J1). Recorded now so J10's baseline and later
/// regression checks have a reference point.
/// </summary>
[MemoryDiagnoser]
public class NumericsBenchmark
{
    private readonly Q32 _a = Q32.FromRatio(355, 113);
    private readonly Q32 _b = Q32.FromRatio(-27, 7);
    private readonly Q32 _rate = Q32.FromRatio(3, 100);

    /// <summary>Q32 multiplication with a 128-bit intermediate and half-to-even rounding.</summary>
    [Benchmark]
    public Q32 Multiply() => _a * _b;

    /// <summary>Q32 division with a 128-bit intermediate and half-to-even rounding.</summary>
    [Benchmark]
    public Q32 Divide() => _a / _b;

    /// <summary>Integer exp.</summary>
    [Benchmark]
    public Q32 Exp() => FixedMath.Exp(_b);

    /// <summary>Integer natural logarithm.</summary>
    [Benchmark]
    public Q32 Log() => FixedMath.Log(_a);

    /// <summary>Integer square root.</summary>
    [Benchmark]
    public Q32 Sqrt() => FixedMath.Sqrt(_a);

    /// <summary>The hazard primitive over one calendar month.</summary>
    [Benchmark]
    public Q32 HazardProbability() => Hazard.Probability(_rate, 2_629_746);
}
