# Pass 1 build plan: Time and Resolution

This plan splits the confirmed Pass 1 spec (`docs/04_pass1_spec.md`) into eleven jobs. It adds nothing to the spec. Each job is one branch, one pull request and one green CI run. The jobs run in this order, because each one uses the one before it.

Every job follows the same loop:

1. Claude Code reads the spec sections named in the job.
2. It writes the tests first, then the code.
3. It runs the tests locally, then pushes the branch so CI runs them on Windows, Linux and macOS.
4. It gives the owner a plain-language summary: what was built, what the tests prove, anything it could not decide.
5. The owner merges once CI is green.

| Job | Name | Spec sections | Needs |
| --- | --- | --- | --- |
| J0 | Repository and CI | 12 | nothing |
| J1 | Numerics | 8, 10 (math primitives) | J0 |
| J2 | Random streams | 9 | J1 |
| J3 | Calendar and clock | 2, 3 | J1 |
| J4 | Step controller | 4 | J3 |
| J5 | Event queue | 5 | J3 |
| J6 | Scheduler and resolution order | 6, 10 | J2, J4, J5 |
| J7 | Standing orders and Advance Until | 7 | J6 |
| J8 | Input log, serializer and replay hash | 11 | J6 |
| J9 | Test systems and the 1,000-turn exit run | 13 | J7, J8 |
| J10 | Benchmark baseline and Pass 1 sign-off | 12, 13 | J9 |

## J0 Repository and CI

- A .NET solution containing three projects: `Atlasiv.Core` (class library, no Godot reference), `Atlasiv.Core.Tests` (xUnit with FsCheck) and `Atlasiv.Benchmarks` (BenchmarkDotNet).
- Choose the .NET version: the long-term-support version that the current stable Godot 4.x .NET build supports. Record it, with the source checked, in `docs/decisions-log.md`.
- A GitHub Actions workflow that builds and tests on Windows, Linux, macOS x64 and macOS ARM.
- Deterministic build settings (warnings as errors, invariant culture in tests).
- Done when one placeholder test passes on all four runners.

## J1 Numerics

- 64-bit integer base units for counts, money and mass. A Q32.32 fixed-point type for rates, probabilities and other dimensionless values. Multiplication and division use `Int128` intermediates with checked overflow.
- Round-half-to-even, plus stochastic rounding as a hook that J2 completes.
- Overflow throws a simulation error. It never wraps silently.
- Sums, aggregates and exposures accumulate in `Int128`. Overflow tests cover money through denomination change, high inflation, redenomination and large aggregate exposures (spec section 8, clarification 14).
- Integer or table-based exp, log and sqrt, and the hazard primitive 1 − exp(−λΔt), all with published test vectors.
- Done when arithmetic test vectors, overflow tests, rounding cases and FsCheck properties (commutativity, inverses within stated error) pass.

## J2 Random streams

- A counter-based generator: each draw is a pure function of (world seed, turn, phase, system ID, entity ID, draw index). Pick Philox or a SplitMix64-style mixer, record the reason in `docs/decisions-log.md`, and publish its test vectors.
- Helpers: uniform integer, fixed-point uniform, Bernoulli, Poisson, stochastic rounding, time-to-event draw.
- No `System.Random` anywhere (add an analyzer or test that fails the build if it appears).
- Done when the test vectors match, and adding unrelated entities leaves existing draws unchanged.

## J3 Calendar and clock

- Turn numbers count from 1 at the epoch and never reset; phases are numbered from 1 within each turn (spec clarification 17).
- A signed 64-bit clock in seconds from the epoch, 1 January 10,000 BCE 00:00, proleptic Gregorian.
- Astronomical year numbering internally. BCE/CE display with no year zero. Gregorian leap rule over the whole timeline.
- The step table (8 steps and their phase counts) and the boundary calculator for every step's phase dates. Turns and phases are half-open intervals [start, end), so a boundary instant belongs to the phase that starts there.
- Rate time = elapsed seconds ÷ 31,556,952.
- Done when events exactly on a boundary land in the starting phase, BCE/CE round trips, leap years at 1600, 1700, 1900 and 2000, every step's phase dates across month ends and leap years, and FsCheck round-trip properties all pass.

## J4 Step controller

- Reads leading-edge strategic time from an interface (tests feed scripted values).
- Picks the candidate step on a log scale (about 2 turns per reference undertaking).
- Hysteresis of 10 turns before going faster, 20 before going slower and 20 between switches, one step at a time. Faster switches align immediately; slower switches wait for 1 January or 1 July.
- Emits an era-transition event with a preview.
- Done when every hysteresis and alignment case in spec section 13 passes.

## J5 Event queue

- An event record with an ordering key of (timestamp, resolution stage, priority, stable event ID).
- Stable ID = (creating system ID, turn, phase, per-system sequence number).
- A binary heap and interruption classes (informational, decision-critical).
- Done when the same events inserted in shuffled orders always resolve in the same order (FsCheck property).

## J6 Scheduler and resolution order

- System registration with a declared stage, natural period and cadence. Systems run only in their stage, and only when due.
- The phase runner (phase stages 1 to 11) and the turn-end runner (turn-end stages 1 to 10), in the order fixed by spec section 6. "Step" always means turn length; "stage" always means a resolution slot (spec section 6, Terms).
- A substep driver (1 to N calendar-exact subintervals, N configurable, default budget 4; escalation beyond the budget is deterministic and logged) and a solver interface (iteration cap, tolerance, fallback).
- The propagator cache keyed by exact elapsed interval: the eight steps and phase lengths precomputed, any other interval computed on first use and kept, never composed from shorter propagators (spec clarification 18).
- Done when stage order, due-period logic and time-step consistency (the same elapsed time split differently gives identical results, or documented bounded differences) pass; a 5-year system on the 2-year step runs after years 6 and 10, covering 6 then 4 years; and a cold cache and a warm cache give identical hashes.

## J7 Standing orders and Advance Until

- The standing-order model and its state machine: active, paused, partial, failed, completed. No refunds of committed costs.
- Standing-order fields include expiry (spec section 7, clarification 16), used later by development orders.
- Advance Until with the default stop list and customisable filters that can never disable consistency checks.
- Hooks only. Real triggers arrive with their systems.
- Done when state-machine and stop-filter tests pass.

## J8 Input log, serializer and replay hash

- A campaign manifest at the start of the input log: core build identifier, ruleset version, schema version, active first-party expansions with versions and load order, content hashes, execution configuration, world seed and initial-conditions hash. Replay refuses a different manifest.
- An ordered input log of validated orders for each phase.
- Canonical serialization: fixed field order, entities sorted by stable ID, little-endian integers.
- XxHash128 per phase and turn end, and SHA-256 for save integrity.
- A replay verifier: initial state + seed + input log reproduces every hash.
- Done when replay reproduces the hashes, and parallel integer reductions match on 1, 2 and 8 threads.

## J9 Test systems and the exit run

- Dummy systems: a counter, a random walk and two mutually dependent systems.
- A synthetic 1,000-turn scenario covering all eight steps and several transitions.
- CI compares per-phase hashes across Windows, Linux, macOS x64 and macOS ARM, with and without parallelism.
- Observer independence: runs that differ only in the observed entity, the selected entity or the player's controlled country (same orders) give identical hashes (clarification 12, test RC-05).
- A convergence test: a dummy slow system at 50-year steps matches the 1-year run within a set tolerance.
- Done when this, the confirmed exit test (identical replay hashes on two machines), passes in CI.

## J10 Benchmark baseline and sign-off

- BenchmarkDotNet records scheduling overhead against the 0.2-second ordinary phase budget on the reference hardware (Computational Doctrine section 8). Later runs fail if they regress past a set threshold.
- Write `docs/PASS1_REPORT.md`: what was built, every exit criterion with its evidence, decisions taken, and anything deferred.
- Done when the owner reads the report and signs off Pass 1.

## Owner checkpoints

- After J0: confirm CI is green on all four runners.
- After J3: read the calendar summary (dates are where mistakes are most visible).
- After J9: the exit test.
- After J10: Pass 1 sign-off, then Pass 2 begins in the chat with a new spec.
