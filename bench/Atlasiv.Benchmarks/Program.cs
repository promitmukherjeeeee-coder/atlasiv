using BenchmarkDotNet.Running;

namespace Atlasiv.Benchmarks;

/// <summary>Entry point: runs the benchmarks chosen on the command line.</summary>
public static class Program
{
    /// <summary>Runs BenchmarkDotNet's switcher over every benchmark in this assembly.</summary>
    public static void Main(string[] args) =>
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
}
