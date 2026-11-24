using BenchmarkDotNet.Attributes;

namespace Atlasiv.Benchmarks;

/// <summary>
/// Placeholder so the harness has something to run. J10 replaces it with the
/// scheduling-overhead baseline.
/// </summary>
[MemoryDiagnoser]
public class PlaceholderBenchmark
{
    /// <summary>Sums 0..999 with integer arithmetic.</summary>
    [Benchmark]
    public long IntegerSum()
    {
        long total = 0;
        for (long i = 0; i < 1000; i++)
        {
            total += i;
        }

        return total;
    }
}
