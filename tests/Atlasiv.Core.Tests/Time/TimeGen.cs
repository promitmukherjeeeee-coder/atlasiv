using FsCheck;
using FsCheck.Fluent;

namespace Atlasiv.Core.Tests.Time;

/// <summary>Random inputs for the calendar properties.</summary>
internal static class TimeGen
{
    /// <summary>Number of random cases each calendar property checks.</summary>
    public const int Cases = 2000;

    /// <summary>
    /// The largest day count the properties use: about 2.7 billion years either side of the
    /// epoch, far beyond any campaign and still well inside the clock's 64-bit range.
    /// </summary>
    public const long MaxDays = 1_000_000_000_000;

    /// <summary>Seconds in a day.</summary>
    public const long Day = 86_400;

    /// <summary>Checks <paramref name="property"/> on <see cref="Cases"/> random inputs; throws on the first failure.</summary>
    public static void Check(Property property) =>
        FsCheck.Check.One(Config.QuickThrowOnFailure.WithMaxTest(Cases), property);

    /// <summary>
    /// A day count from the epoch, either side, spread so that small counts (the real
    /// timeline) and huge ones are both drawn often.
    /// </summary>
    public static Gen<long> Days() =>
        Gen.OneOf(
            Gen.Choose(-1_000_000, 10_000_000).Select(d => (long)d),
            from hi in Gen.Choose(-232_830, 232_830)
            from lo in Gen.Choose(0, int.MaxValue)
            select (long)hi * 4_294_967L + lo % 4_294_967L);

    /// <summary>A clock reading in seconds from the epoch, within <see cref="MaxDays"/> days either side.</summary>
    public static Gen<long> Seconds() =>
        from days in Days()
        from second in Gen.Choose(0, (int)Day - 1)
        select days * Day + second;
}
