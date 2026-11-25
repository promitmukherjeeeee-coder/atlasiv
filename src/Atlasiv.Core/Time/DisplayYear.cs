using System;
using System.Globalization;
using Atlasiv.Core.Numerics;

namespace Atlasiv.Core.Time;

/// <summary>The era of a displayed year.</summary>
public enum CalendarEra
{
    /// <summary>Before the Common Era: astronomical year 0 and earlier.</summary>
    Bce = 0,

    /// <summary>Common Era: astronomical year 1 and later.</summary>
    Ce = 1,
}

/// <summary>
/// A year as players read it: a number from 1 upward and an era, BCE or CE, with no year
/// zero (Pass 1 spec section 2). The simulation itself uses astronomical years, in which
/// 1 BCE is year 0 and 10,000 BCE is year -9,999; this type converts between the two.
/// </summary>
/// <remarks>
/// The number is stored as number - 1, so the default value is 1 BCE (astronomical year 0),
/// a real year, rather than an impossible "year 0".
/// </remarks>
public readonly record struct DisplayYear
{
    private readonly long _numberMinusOne;

    /// <summary>Creates a displayed year.</summary>
    /// <param name="number">The year number, 1 or more.</param>
    /// <param name="era">BCE or CE.</param>
    /// <exception cref="ArgumentOutOfRangeException">The number is below 1, or the era is not BCE or CE.</exception>
    public DisplayYear(long number, CalendarEra era)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(number, 1);
        if (era is not (CalendarEra.Bce or CalendarEra.Ce))
        {
            throw new ArgumentOutOfRangeException(nameof(era), era, "The era must be BCE or CE.");
        }

        _numberMinusOne = number - 1;
        Era = era;
    }

    /// <summary>The year number, 1 or more.</summary>
    public long Number => _numberMinusOne + 1;

    /// <summary>BCE or CE.</summary>
    public CalendarEra Era { get; }

    /// <summary>The astronomical year: CE years unchanged, n BCE as 1 - n.</summary>
    public long Astronomical => Era == CalendarEra.Ce ? Number : -_numberMinusOne;

    /// <summary>The displayed year for astronomical year <paramref name="astronomicalYear"/>.</summary>
    /// <exception cref="SimulationOverflowException">The year is <see cref="long.MinValue"/>, whose BCE number does not fit in 64 bits.</exception>
    public static DisplayYear FromAstronomical(long astronomicalYear) =>
        astronomicalYear >= 1
            ? new DisplayYear(astronomicalYear, CalendarEra.Ce)
            : new DisplayYear(WideMath.ToInt64(1 - (Int128)astronomicalYear, "DisplayYear.FromAstronomical"), CalendarEra.Bce);

    /// <summary>
    /// The year as text, such as "1066 CE" or "10,000 BCE". Numbers of five or more digits
    /// group their thousands with commas; shorter ones do not (ENG-010).
    /// </summary>
    public override string ToString() =>
        FormatNumber(Number) + (Era == CalendarEra.Ce ? " CE" : " BCE");

    private static string FormatNumber(long number) =>
        number.ToString(number >= 10_000 ? "N0" : "D", CultureInfo.InvariantCulture);
}
