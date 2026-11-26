# ATLASIV

The shared language for ATLASIV, a single-player grand-strategy simulation of Earth from 10,000 BCE to the first faster-than-light test. Terms here mean exactly one thing across the design documents, code and tests.

## Time

**Epoch**:
The instant the simulation clock reads zero: 1 January 10,000 BCE, 00:00, proleptic Gregorian calendar.
_Avoid_: Start date, game start

**Turn**:
One full cycle of play: its phases in order, followed by the turn end. Every turn starts on a calendar date that is valid for its step.
_Avoid_: Round, tick

**Phase**:
One numbered slice of a turn, bounded by exact calendar dates, in which orders are given and fast systems resolve. It contains its start instant but not its end instant, so every moment belongs to exactly one phase.
_Avoid_: Sub-turn, segment

**Step**:
The calendar length of a turn (50 years, 20, 10, 5, 2, 1 year, 6 months or 1 month), which also fixes how many phases the turn has.
_Avoid_: Turn length, time scale, tempo

**Phase stage**:
One of the 11 numbered resolution slots that run, in fixed order, inside every phase, from snapshotting state to publishing the player-visible state.
_Avoid_: Step (a step is a turn length), phase step

**Turn end**:
The close of a turn, after its last phase, in which the 10 turn-end stages run once.
_Avoid_: Turn-end tick, end of turn processing

**Turn-end stage**:
One of the 10 numbered resolution slots that run, in fixed order, at each turn end, from climate and water to records, summaries and snapshots.
_Avoid_: Step

**Stage**:
Either a phase stage or a turn-end stage. Every system belongs to exactly one stage.
_Avoid_: Step, slot

**Substep**:
One of 1 to 4 smaller calendar-exact time slices that a volatile province can use within a turn-end stage.
_Avoid_: Subtick, step

**Natural period**:
How much calendar time passes between a system's runs. A system runs only in its own stage, and only when its natural period is due.
_Avoid_: Frequency, tick rate

**Leading edge**:
The fastest transport capability that is usable at meaningful capacity by any entity holding at least 0.5% of world population.

**Candidate step**:
The step whose length is closest on a log scale to half the leading edge's strategic time for the reference undertaking.
_Avoid_: Target step

**Era-transition event**:
The announced event, with a preview, that marks a change of step.
_Avoid_: Era change (eras are descriptive and never change the step)

**Slot**:
Where in a turn something happens: one of its phases, or the turn end, which has its own slot after the last phase. Used in random-draw keys and stable event IDs. Not a stage.

**Event queue**:
The single queue that resolves events in order of timestamp, stage, event priority and stable event ID. Events at the same instant resolve turn-end stages first, then phase stages; a lower priority number resolves first.

**Stable event ID**:
An event's unique identity: the system that created it, the turn, the slot, and that system's sequence number within the slot. It breaks every remaining tie in the event queue.
_Avoid_: Event index, event number

## Orders

**Standing order**:
A persistent instruction that keeps acting across phases and turns until it completes, fails or is paused.
_Avoid_: Automation, macro

**Advance Until**:
The player control that keeps resolving phases without input until a stop condition is met. Every phase is still fully resolved; a quiet phase simply does not hand control to the player.
_Avoid_: Fast-forward, auto-play, auto-skip (nothing is skipped in the computation)

**Turn summary**:
The record of what happened in a turn, produced in the last turn-end stage, separate from any screen that displays it.

**Interruption class**:
Whether an event resolves on its own (informational) or halts Advance Until (decision-critical).

## Numerics

**Base unit**:
The smallest whole unit in which a count, an amount of money or a mass is stored, as a 64-bit integer (for example one person, or one minor unit of currency).

**Q32**:
The Q32.32 fixed-point format for rates, probabilities and other dimensionless values: a 64-bit integer read as that integer divided by 2^32.
_Avoid_: Fixed (ambiguous), float, decimal

**Round half to even**:
The default rounding: a value goes to the nearest whole number, and an exact half goes to the even neighbour.
_Avoid_: Banker's rounding (same thing; use one name)

**Stochastic rounding**:
Rounding down or up at random, going up with probability equal to the fractional part, so the expected result equals the exact value. Used only where a rule says so.

**Accumulator**:
A running total of base-unit amounts held in a 128-bit integer, used for sums, aggregates and exposures. Read back as 64 bits only where a 64-bit value is needed.
_Avoid_: Sum (too generic), aggregate (overloaded)

**Exposure**:
The value of a position: quantity × price, in minor units. Exposures are added up in an accumulator, because a single one can exceed the 64-bit range.

**Simulation overflow**:
A number leaving its representable range. It is an error that stops the commit, never a wrap or a clip.

**Rate year**:
31,556,952 seconds (365.2425 days). Per-year rates apply to elapsed seconds divided by this length.

**Integrated hazard**:
The sum of rate × elapsed time over the pieces of an interval, H. The chance that the event happens within the interval is 1 − exp(−H).
_Avoid_: Cumulative probability (H is not a probability)

## Randomness

**World seed**:
The 64-bit number, recorded in the campaign manifest, from which every random draw in a campaign follows.
_Avoid_: Random seed (there is only one)

**Draw address**:
The (world seed, turn, draw slot, substep, system, entity) that names one random stream. Each draw is a pure function of its address and its draw index, never of iteration order.
_Avoid_: RNG state, generator (there is no per-system state)

**Draw slot**:
Where in a turn a draw is made: phase 1 to N, or the turn end, which has its own fixed slot after every phase.
_Avoid_: Stage (a stage is a resolution slot; a draw slot is a phase or the turn end)

**Draw index**:
The position of a draw within one address's stream, counting from 0.

**Time-to-event draw**:
A random waiting time until an event with a known rate, drawn once and scheduled in the event queue, and redrawn only when the rate changes.
_Avoid_: Per-phase check
