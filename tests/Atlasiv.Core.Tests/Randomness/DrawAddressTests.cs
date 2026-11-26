using System;
using System.Collections.Generic;
using System.Linq;
using Atlasiv.Core.Randomness;
using Atlasiv.Core.Time;
using Atlasiv.Core.Tests.Numerics;
using FsCheck;
using FsCheck.Fluent;
using Xunit;

namespace Atlasiv.Core.Tests.Randomness;

/// <summary>
/// The draw address: every draw is a pure function of (world seed, turn, phase or turn-end
/// slot, substep, system ID, entity ID, draw index) (spec section 9, clarification 3).
/// </summary>
public sealed class DrawAddressTests
{
    private const ulong Seed = 0x5EED_0000_0000_0001UL;
    private const uint System = 7;

    private static DrawAddress Key(ulong entity, long turn = 12, int phase = 3, int substep = 0) =>
        new(Seed, turn, TurnSlot.Phase(phase), substep, System, entity);

    // ---- Adding unrelated entities leaves existing draws unchanged ----

    [Fact]
    public void Adding_unrelated_entities_leaves_existing_draws_unchanged()
    {
        // A dummy system draws a varying number of values per entity while iterating entities
        // in ascending ID order. A stateful generator shared by the system would shift every
        // later entity's draws when an entity is inserted; keyed streams must not.
        var original = new ulong[] { 3, 10, 11, 500, 9_000_000_000 };
        var withNewcomers = original.Concat(new ulong[] { 1, 4, 12, 499, 501, ulong.MaxValue }).Order().ToArray();

        var before = RunDummySystem(original);
        var after = RunDummySystem(withNewcomers);

        foreach (var entity in original)
        {
            Assert.Equal(before[entity], after[entity]);
        }
    }

    [Fact]
    public void Iteration_order_does_not_change_any_entitys_draws()
    {
        var ascending = new ulong[] { 1, 2, 3, 40, 41, 1_000 };
        var forward = RunDummySystem(ascending);
        var backward = RunDummySystem(ascending.Reverse().ToArray());

        foreach (var entity in ascending)
        {
            Assert.Equal(forward[entity], backward[entity]);
        }
    }

    // ---- Distinct addresses never share a draw ----

    [Fact]
    public void Two_substeps_never_share_a_draw()
    {
        var draws = new HashSet<ulong>();
        for (var substep = 0; substep < 64; substep++)
        {
            var key = Key(entity: 42, substep: substep);
            for (ulong index = 0; index < 256; index++)
            {
                Assert.True(draws.Add(key.Draw(index)), $"substep {substep}, draw {index} repeated an earlier draw");
            }
        }
    }

    [Fact]
    public void The_turn_end_and_the_last_phase_never_share_a_draw()
    {
        // The 50-year step has 20 phases, the most of any step (spec section 3).
        var lastPhase = new DrawAddress(Seed, 12, TurnSlot.Phase(20), 0, System, 42);
        var turnEnd = new DrawAddress(Seed, 12, TurnSlot.TurnEnd, 0, System, 42);
        var draws = new HashSet<ulong>();
        for (ulong index = 0; index < 1_024; index++)
        {
            Assert.True(draws.Add(lastPhase.Draw(index)));
            Assert.True(draws.Add(turnEnd.Draw(index)));
        }
    }

    [Fact]
    public void The_turn_end_slot_comes_after_every_phase()
    {
        Assert.True(TurnSlot.TurnEnd.IsTurnEnd);
        Assert.False(TurnSlot.Phase(20).IsTurnEnd);
        Assert.True(TurnSlot.TurnEnd.Value > TurnSlot.Phase(int.MaxValue).Value);
    }

    [Fact]
    public void Each_field_of_the_address_changes_the_draw()
    {
        var baseKey = new DrawAddress(Seed, 12, TurnSlot.Phase(3), 1, System, 42);
        var variants = new[]
        {
            baseKey,
            baseKey with { WorldSeed = Seed + 1 },
            baseKey with { Turn = 13 },
            baseKey with { Slot = TurnSlot.Phase(4) },
            baseKey with { Slot = TurnSlot.TurnEnd },
            baseKey with { Substep = 2 },
            baseKey with { SystemId = System + 1 },
            baseKey with { EntityId = 43 },
        };

        var firstDraws = variants.Select(k => k.Draw(0)).Append(baseKey.Draw(1)).ToArray();

        Assert.Equal(firstDraws.Length, firstDraws.Distinct().Count());
    }

    [Fact]
    public void Distinct_addresses_map_to_distinct_generator_inputs()
    {
        // The packing into Philox's counter and key is one-to-one, and Philox is a bijection
        // for a fixed key, so two distinct addresses can never be handed the same output word.
        // Each field takes a few values, including its extremes, so equal and nearly equal
        // addresses come up often.
        var address =
            from seed in Gen.Elements(0UL, 1UL, ulong.MaxValue)
            from turn in Gen.Elements(1L, 2L, long.MaxValue)
            from slot in Gen.Elements(TurnSlot.Phase(1), TurnSlot.Phase(2), TurnSlot.Phase(int.MaxValue), TurnSlot.TurnEnd)
            from substep in Gen.Elements(0, 1, int.MaxValue)
            from system in Gen.Elements(0U, 1U, uint.MaxValue)
            from entity in Gen.Elements(0UL, 1UL, ulong.MaxValue)
            from index in Gen.Elements(0UL, 1UL, 3UL, 4UL, ulong.MaxValue)
            select (Key: new DrawAddress(seed, turn, slot, substep, system, entity), Index: index);

        NumericGen.Check(Prop.ForAll(Arb.From(from a in address from b in address select (a, b)), pair =>
        {
            var (a, b) = pair;
            return (a == b) == (a.Key.GeneratorInput(a.Index) == b.Key.GeneratorInput(b.Index));
        }));
    }

    [Fact]
    public void A_draw_is_a_pure_function_of_its_address()
    {
        var key = Key(entity: 9);
        var first = Enumerable.Range(0, 100).Select(i => key.Draw((ulong)i)).ToArray();
        var again = Enumerable.Range(0, 100).Reverse().Select(i => key.Draw((ulong)i)).Reverse().ToArray();

        Assert.Equal(first, again);
    }

    // ---- Invalid addresses ----

    [Fact]
    public void Turns_count_from_one()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new DrawAddress(Seed, 0, TurnSlot.Phase(1), 0, System, 1));
    }

    [Fact]
    public void Phases_count_from_one()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TurnSlot.Phase(0));
    }

    [Fact]
    public void Substeps_count_from_zero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new DrawAddress(Seed, 1, TurnSlot.Phase(1), -1, System, 1));
    }

    [Fact]
    public void The_default_address_cannot_draw()
    {
        Assert.Throws<InvalidOperationException>(() => default(DrawAddress).Draw(0));
        Assert.Throws<InvalidOperationException>(() => new RandomDraws(default));
    }

    [Fact]
    public void An_address_built_from_the_default_without_a_slot_cannot_draw()
    {
        // `with` sets only the named fields, so the slot keeps the default's invalid 0.
        var noSlot = default(DrawAddress) with { Turn = 5 };

        Assert.Throws<InvalidOperationException>(() => noSlot.Draw(0));
        Assert.Throws<InvalidOperationException>(() => new RandomDraws(noSlot));
    }

    private static Dictionary<ulong, ulong[]> RunDummySystem(IEnumerable<ulong> entities)
    {
        var results = new Dictionary<ulong, ulong[]>();
        foreach (var entity in entities)
        {
            var key = Key(entity);
            var count = 1 + (int)(key.Draw(0) % 7); // varying draw counts per entity
            results[entity] = Enumerable.Range(1, count).Select(i => key.Draw((ulong)i)).ToArray();
        }

        return results;
    }
}
