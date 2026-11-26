using System.Linq;
using Atlasiv.Core.Events;
using Atlasiv.Core.Scheduling;
using Atlasiv.Core.Time;
using FsCheck;
using FsCheck.Fluent;

namespace Atlasiv.Core.Tests.Events;

/// <summary>Random inputs for the event-ordering properties.</summary>
internal static class EventGen
{
    /// <summary>Number of random cases each event property checks.</summary>
    public const int Cases = 1000;

    /// <summary>Checks <paramref name="property"/> on <see cref="Cases"/> random inputs; throws on the first failure.</summary>
    public static void Check(Property property) =>
        FsCheck.Check.One(Config.QuickThrowOnFailure.WithMaxTest(Cases), property);

    /// <summary>Any of the 21 stages: phase stages 1 to 11 and turn-end stages 1 to 10.</summary>
    public static Gen<ResolutionStage> Stage() =>
        Gen.OneOf(
            Gen.Choose(1, ResolutionStage.PhaseStageCount).Select(ResolutionStage.Phase),
            Gen.Choose(1, ResolutionStage.TurnEndStageCount).Select(ResolutionStage.TurnEnd));

    /// <summary>
    /// A slot: phases 1 to 20 (20 is the most any step has) or the turn end.
    /// </summary>
    public static Gen<TurnSlot> Slot() =>
        Gen.Frequency(
            (4, Gen.Choose(1, 20).Select(TurnSlot.Phase)),
            (1, Gen.Constant(TurnSlot.TurnEnd)));

    /// <summary>
    /// An ID drawn from a small range of every field, so equal fields (and equal IDs) are common.
    /// </summary>
    public static Gen<StableEventId> Id() =>
        from system in Gen.Choose(0, 3)
        from turn in Gen.Choose(1, 3)
        from slot in Slot()
        from sequence in Gen.Choose(0, 3)
        select new StableEventId((uint)system, turn, slot, (ulong)sequence);

    /// <summary>
    /// A key whose timestamp, stage and priority come from small ranges, so ties on the earlier
    /// fields are common and the later fields have to break them.
    /// </summary>
    public static Gen<EventKey> Key() =>
        from seconds in Gen.Choose(-2, 2)
        from stage in Stage()
        from priority in Gen.Choose(-2, 2)
        from id in Id()
        select new EventKey(new Instant(seconds), stage, priority, id);

    /// <summary>Between 0 and 200 keys with no two sharing a stable ID.</summary>
    public static Gen<EventKey[]> KeysWithDistinctIds() =>
        Gen.ListOf(Key()).Select(keys => keys.DistinctBy(k => k.Id).Take(200).ToArray());
}
