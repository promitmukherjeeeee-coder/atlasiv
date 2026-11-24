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
