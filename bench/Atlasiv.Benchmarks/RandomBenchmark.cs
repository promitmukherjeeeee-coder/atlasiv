using Atlasiv.Core.Numerics;
using Atlasiv.Core.Randomness;
using Atlasiv.Core.Time;
using BenchmarkDotNet.Attributes;

namespace Atlasiv.Benchmarks;

/// <summary>
/// Cost of the counter-based generator and its draw helpers (J2). Recorded now so J10's
/// baseline and later regression checks have a reference point.
/// </summary>
[MemoryDiagnoser]
public class RandomBenchmark
{
    private readonly DrawAddress _address = new(0x5EED_0000_0000_0001UL, 12, TurnSlot.Phase(3), 0, 7, 42);
    private readonly Q32 _halfChance = Q32.FromRatio(1, 2);
    private readonly Q32 _poissonMean = Q32.FromRatio(5, 2);
    private readonly Q32 _rate = Q32.FromRatio(3, 100);
    private RandomDraws _draws = null!;
    private ulong _index;

    /// <summary>A fresh stream for each benchmark.</summary>
    [GlobalSetup]
    public void Setup() => _draws = new RandomDraws(_address);

    /// <summary>One Philox4x64-10 block (four draws' worth).</summary>
    [Benchmark]
    public Block4x64 PhiloxBlock() => Philox4x64.Generate(new Block4x64(_index++, 42, 12, 3UL << 32), 1, 7);

    /// <summary>One keyed draw by address, with no block reuse.</summary>
    [Benchmark]
    public ulong AddressDraw() => _address.Draw(_index++);

    /// <summary>One draw from a stream, reusing each block for four draws.</summary>
    [Benchmark]
    public ulong StreamDraw() => _draws.NextUInt64();

    /// <summary>Creating a stream for one entity and taking one draw: the per-entity cost.</summary>
    [Benchmark]
    public ulong NewStreamAndDraw() => new RandomDraws(_address with { EntityId = _index++ }).NextUInt64();

    /// <summary>A Bernoulli draw.</summary>
    [Benchmark]
    public bool Bernoulli() => _draws.NextBernoulli(_halfChance);

    /// <summary>A Poisson count with mean 2.5.</summary>
    [Benchmark]
    public long Poisson() => _draws.NextPoisson(_poissonMean);

    /// <summary>A time-to-event draw at 3% per year.</summary>
    [Benchmark]
    public long? TimeToEvent() => _draws.NextTimeToEventSeconds(_rate);
}
