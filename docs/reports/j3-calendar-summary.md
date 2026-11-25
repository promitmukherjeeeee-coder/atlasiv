# Calendar summary (Pass 1, J3)

How the game's clock and calendar work, for the owner checkpoint after J3. Every date on this page is checked by an automated test.

## The clock

- The clock counts **seconds since 1 January 10,000 BCE, 00:00**. That moment is the epoch, and the clock reads 0 there.
- It is one whole number, never a fraction, and it never ticks second by second. The simulation jumps from one date to the next.
- The calendar is the **Gregorian** one, used backwards over the whole timeline as if it had always existed (the "proleptic" Gregorian calendar).
- A full 12,450-year campaign needs about 390 billion seconds. The clock can hold more than 9 billion billion, so it will never run out.
- Some fixed points:

| Date | Clock reading (seconds) |
| --- | --- |
| 1 January 10,000 BCE | 0 |
| 1 January 1 CE | 315,569,520,000 |
| 1 January 2000 CE | 378,651,801,600 |

From 10,000 BCE to 1 CE is exactly 10,000 average years (25 full 400-year cycles of the calendar). There is no year zero, so this is 10,000 years, not 10,001.

## Years: BCE and CE, no year zero

Players always see BCE and CE. Inside, the simulation counts years like astronomers do, with a year 0, so that subtracting years works:

| Players see | Inside the simulation |
| --- | --- |
| 10,000 BCE | −9,999 |
| 2 BCE | −1 |
| 1 BCE | 0 |
| 1 CE | 1 |
| 2000 CE | 2000 |

The year after 1 BCE is 1 CE. Years of five or more digits get a comma ("10,000 BCE"), shorter ones do not ("1066 CE"). Dates read like "1 January 10,000 BCE" and moments like "20 July 1969 CE, 20:17:40".

## Leap years

The usual Gregorian rule applies in every year, BCE included: a year divisible by 4 is a leap year, except century years, which must also be divisible by 400.

- 1600 and 2000 are leap years; 1700 and 1900 are not.
- 1 BCE (year 0 inside) is a leap year, and so is 401 BCE. 101 BCE is not. The first year, 10,000 BCE, is not.

## Turns and phases

There are eight steps (turn lengths). Every turn and phase starts at midnight on an exact calendar date, so month lengths and leap days come out right. Turns of a year or longer start on any 1 January, not only in round years. A 50-year turn may start in 1901.

| Step | Phases | Example turn | Phase start dates (then the next turn) |
| --- | --- | --- | --- |
| 50 years | 20 | from 1 Jan 1900 | 1 Jan 1900, 1 Jul 1902, 1 Jan 1905, 1 Jul 1907, … 1 Jan 1945, 1 Jul 1947; next turn 1 Jan 1950 |
| 20 years | 10 | from 1 Jan 1980 | 1 Jan of 1980, 1982, … 1998; next turn 1 Jan 2000 |
| 10 years | 10 | from 1 Jan 1995 | 1 Jan of each year 1995 to 2004; next turn 1 Jan 2005 |
| 5 years | 5 | from 1 Jan 1898 | 1 Jan of 1898, 1899, 1900, 1901, 1902; next turn 1 Jan 1903 |
| 2 years | 4 | from 1 Jan 1999 | 1 Jan 1999, 1 Jul 1999, 1 Jan 2000, 1 Jul 2000; next turn 1 Jan 2001 |
| 1 year | 4 | from 1 Jan 2000 | 1 Jan, 1 Apr, 1 Jul, 1 Oct 2000; next turn 1 Jan 2001 |
| 6 months | 3 | from 1 Jul 1999 | 1 Jul, 1 Sep, 1 Nov 1999; next turn 1 Jan 2000 |
| 1 month | 5 | February 2000 | 1, 7, 13, 19, 25 Feb 2000; next turn 1 Mar 2000 |

**The very first turn**, if it uses the 50-year step, runs from 1 January 10,000 BCE: 1 Jan 10,000 BCE, 1 Jul 9998 BCE, 1 Jan 9995 BCE, and so on, ending on 1 January 9950 BCE.

**6-month turns** start only on 1 January or 1 July.

**1-month turns** always split at days 1, 7, 13 and 19, giving four 6-day phases. The last phase starts on the 25th and runs to the end of the month, so its length depends on the month:

| Month | Last phase |
| --- | --- |
| February 1900 (not a leap year) | 4 days |
| February 2000 (leap year) | 5 days |
| April | 6 days |
| January or December | 7 days |

**Leap days land where they should.** The first quarter of 2000 (1 January to 1 April) is 91 days long; the first quarter of 1900 is 90.

## Which phase owns a boundary moment

Each turn and phase **includes its first moment but not its last**. Something that happens exactly on a boundary belongs to the phase that starts there:

- An event at 00:00 on 1 July 2000, in a 1-year turn, is in phase 3 (the one starting 1 July). One second earlier, 23:59:59 on 30 June, is still phase 2.
- An event at 00:00 on the first day of a new turn belongs to phase 1 of the new turn, not to the old one. The old turn ends first.

A randomised test checks this across thousands of turns of every step, over billions of years. Every second belongs to exactly one phase, and the phases fit together with no gaps and no overlaps.

## Rate time

Rates are stored per year. When a rate is applied over a stretch of time, the time is measured as **exact seconds ÷ 31,556,952**, which is the average Gregorian year of 365.2425 days. So:

| Elapsed | Counts as |
| --- | --- |
| 365 days | 0.99934 years |
| 366 days (leap year) | 1.00207 years |
| 1 day | 0.00274 years |

A leap day therefore counts in full, and splitting the same time into different turns gives the same total.

## Not covered by J3

- **Seasons in the header** (northern or southern, by the selected location) are display only. They arrive with the screen that shows them.
- **Choosing and switching steps** is J4, the next job.
