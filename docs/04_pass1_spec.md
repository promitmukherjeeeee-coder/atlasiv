# Pass 1 Spec: Time and Resolution

Nov 24, 2026 · @Promit

The buildable specification for the simulation clock, phases, event ordering, determinism, adaptive integration and step transitions. Language: C#. Engine: Godot 4. Pass 1 code is an engine-free C# library, so it is tested headless.

**How to read this.** Each rule is tagged. **\[Confirmed\]** repeats a decision already in the Final Master Vision (mainly 4.1, 8 and 9.1) or the Turns and Time specification. **\[Proposed\]** is a new implementation choice made here, needed to make the spec buildable. Every Proposed item needs your yes or no, and nothing here adds new game design.

## 1. Scope

**In Pass 1:** the clock and calendar, turn and phase boundaries, step transitions, the event queue and resolution order, standing-order and Advance Until hooks, fixed-point numerics, seeded random streams, adaptive integration hooks, the input log, the replay hash, and tests.

**Not in Pass 1:** the authoritative state store and identifiers (Pass 2), ledgers and the dependency solver (Pass 3), any real game system (Pass 4 onward). Pass 1 uses small dummy systems to test the machinery. \[Confirmed\] Governing principle: simulate the maximum meaningful complexity with the minimum necessary computation. In Pass 1 this means systems run only when their natural period is due, events exist only when due, and slow processes use closed-form integration rather than fixed subticks.

## 2. Clock and calendar

- **\[Confirmed\]** One authoritative integer clock counts elapsed seconds from the start. It is event-driven, so nothing ticks every second. Turn and phase boundaries are exact calendar dates.
- **\[Confirmed****\]** The clock is a signed 64-bit integer. A full run of about 12,450 years is about 3.9 x 10^11 seconds, far inside the 64-bit range.
- **\[Confirmed****\]** Epoch: 1 January 10,000 BCE, 00:00, on the proleptic Gregorian calendar.
- **\[Confirmed\]** Internal years use astronomical numbering (1 BCE = year 0, 10,000 BCE = year -9,999). Display uses BCE/CE with no year zero. Converting between the two is a tested function.
- **\[Confirmed\]** Leap years follow the Gregorian rule (divisible by 4, except centuries not divisible by 400) for the whole timeline.
- **\[Confirmed\]** Seasons shown in the header are meteorological and depend on the hemisphere of the selected location. They are display only.

## 3. Turn steps and phases

**\[Confirmed\]** Eight steps with their phase counts:

| Step | Phases per turn | Nominal phase length |
| --- | --- | --- |
| 50 years | 20 | 2.5 years |
| 20 years | 10 | 2 years |
| 10 years | 10 | 1 year |
| 5 years | 5 | 1 year |
| 2 years | 4 | 6 months |
| 1 year | 4 | 3 months |
| 6 months | 3 | 2 months |
| 1 month | 5 | about 6 days |

**Boundary rules \[Confirmed\].** Phases are defined by calendar dates, not fixed second counts, so leap years and month lengths are handled exactly. Every turn and phase is a half-open interval \[start, end): it includes its start instant and excludes its end instant (clarified 24 Nov 2026). An event timestamped exactly on a boundary therefore belongs to the phase that begins there; an event at 00:00 on 1 July belongs to the phase starting 1 July. When that boundary is also a turn boundary, the old turn's turn-end stages run first and the event resolves in the first phase of the new turn.

- Steps of a year or longer: every turn starts on 1 January.
- 50-year step: phase boundaries alternate between 1 January and 1 July, giving 2.5-year phases.
- 20, 10 and 5-year steps: phase boundaries on 1 January.
- 2-year step: 1 January and 1 July.
- 1-year step: 1 January, 1 April, 1 July, 1 October.
- 6-month step: turns start on 1 January or 1 July. Phases start on the 1st of the turn's first, third and fifth months.
- 1-month step: turns start on the 1st of a month. Phases start on days 1, 7, 13, 19 and 25, and the last phase runs to the end of the month (4 to 7 days).

**Elapsed time for rates \[Confirmed\].** All rates are stored per year \[Confirmed\]. A per-year rate applied over an interval uses the exact elapsed seconds divided by the mean Gregorian year of 31,556,952 seconds \[Confirmed\], so a leap day counts correctly and results do not depend on how a period is split into turns.

## 4. Step transitions

- **\[Confirmed\]** Leading edge: the fastest transport capability that is operationally usable at meaningful capacity, held by any entity with at least 0.5% of world population.
- **\[Confirmed\]** Target: a substantial continental undertaking takes about 2 turns of strategic operational pacing. For each step, compare its length with (strategic time for the reference undertaking) / 2 on a log scale and pick the closest as the candidate.
- **\[Confirmed\]** Switch only when the candidate differs for 10 consecutive turns (faster) or 20 (slower), with at least 20 turns between any two switches, one step at a time. A switch takes effect at a turn boundary, is announced as an era-transition event with a preview, and steps can slow again after a collapse. Eras never trigger switches.
- **\[Confirmed\]** Standing orders keep their real elapsed-time requirements across a switch.
- **\[Confirmed\] Alignment.** A switch takes effect at the first turn boundary that is a valid start for the new step (section 3). Switching to a faster step always aligns immediately. Switching to a slower step can wait up to one year for 1 January or 1 July.
- **\[Confirmed****\] Placeholder transport input.** Pass 1 has no transport system yet, so the step controller reads the leading-edge strategic time from an interface. Tests feed it scripted values. The reference undertaking and distance are calibrated later (data phase).

## 5. Event queue

- **\[Confirmed\]** Events carry timestamps and enter a deterministic priority queue ordered by (timestamp, resolution stage, event priority, stable event ID).
- **\[Confirmed****\] Stable event ID.** Built from (creating system ID, turn, phase, per-system sequence number). Systems create events while iterating entities in ascending stable-ID order, so the sequence numbers are reproducible. Pass 2 replaces this with the permanent event identity, keeping the same ordering guarantee.
- **\[Confirmed\]** Interruption classes: informational events resolve automatically, decision-critical events pause Advance Until, and the player otherwise acts only at phase boundaries or rare, formally defined interruption points.
- **\[Confirmed\]** Events are generated only when due, and enough of them are kept for exact replay.
- **\[Confirmed\]** The queue is a binary heap keyed by the full ordering tuple. Two events can never compare as equal, because the stable ID is unique.

## 6. Resolution order

**\[Confirmed\] Each phase:**

1. Snapshot state.
2. Collect and validate orders (player and AI against the same snapshot).
3. Commit standing orders.
4. Scheduled events and communications.
5. Diplomacy and covert actions.
6. Movement, transport and logistics.
7. Encounters, battles, sieges and occupation.
8. Character, political and unrest events.
9. Immediate stock transfers.
10. Record deltas, events and provenance.
11. Publish the player-visible state.

**\[Confirmed\] Each turn end:** climate and water; ecology and biomes; demographic reconciliation; agriculture, extraction, production and consumption; trade, markets and logistics accounting; finance and ownership; knowledge and diffusion; culture, religion and institutions; derived statistics and scoring samples; records, summaries and snapshots.

**Terms (clarified 24 Nov 2026; no rule changes).** A *step* is a turn length: one of the eight values in section 3 (50 years down to 1 month). A *phase* is one subdivision of a turn. A *stage* is a numbered resolution slot: phase stages 1 to 11 run in every phase, and turn-end stages 1 to 10 (climate and water through records, summaries and snapshots, in the order listed) run once at the end of every turn. A *substep* is one of the calendar-exact subintervals (budget 4 by default) the adaptive driver may use inside a turn-end stage (section 10). A *time step* (Δt) is any legal elapsed interval used to look up the propagator cache.

- **\[Confirmed\]** Simultaneous orders resolve against the same snapshot by domain contest or allocation rules, never by internal list order.
- **\[Confirmed\]** Military operational and engagement events resolve at their own timestamps inside a phase.
- **\[Confirmed\]** In Pass 1 every system registers with a declared stage (one of the 11 phase stages or one of the 10 turn-end stages listed above), natural period and update cadence. The scheduler calls systems only in their stage, and only when their period is due.

## 7. Standing orders and Advance Until (hooks only)

- **\[Confirmed\]** Standing-order fields: issuer, authority, target, limits, start condition, constraints, priority, suspension rules, exception policy. States: active, paused, partial, failed, completed. No irreversible action continues without authority, and committed costs are not refunded.
- **\[Confirmed\]** Advance Until stops by default on war declaration or incursion, succession crisis, first contact, major disaster, famine threshold, hostile plot, treaty needing action, standing-order failure, project milestone, major discovery, and imminent rebellion or constitutional crisis. Filters can be customized but never disable consistency checks.
- **\[Confirmed\]** Conditional military interruptions exist, but their exact rules are still open.
- **\[Confirmed\]** Pass 1 builds the data structures, state machine and stop-filter mechanism. The real triggers arrive with their systems.

## 8. Numerics

- **\[Confirmed\]** Fixed-point math for conserved and authoritative quantities, exact integers for population and ledgers, seeded stochastic rounding where hazards convert to whole people. Floating point is not used in authoritative state.
- **\[Confirmed\]** No `float` or `double` in authoritative state or its update rules, except under a deterministic floating-point profile approved in the Architectural Decision Registry after benchmarks (ADR-005). Presentation, forecasts, analysis and labelled approximations may use floating point. Transcendental functions (exponential, logarithm, roots) use integer algorithms or precomputed fixed-point tables with test vectors, because library math functions can differ between platforms and runtimes.
- **\[Confirmed\]** Per-quantity scaled integers (ADR-004): every quantity declares its unit and scale in the data schema. Counts, money and mass use 64-bit integer base units. Rates, probabilities and other dimensionless values use scaled fixed point, with Q32.32 as the default format for them. A single format is not used for everything, because 32.32 overflows above about 2.1 billion, which world population and economic totals exceed. Multiplication and division use 128-bit intermediates (`System.Int128`) with checked overflow.
- **\[Confirmed\]** Money is stored as integer minor units. Population counts are 64-bit integers. Sums, aggregates and exposures accumulate in 128-bit integers. Overflow tests cover denomination change, high inflation, redenomination and aggregate financial exposure; whether a per-currency scale is also needed is decided in Pass 2 using those tests (added 25 Nov 2026).
- **\[Confirmed\]** Deterministic rounding is round-half-to-even unless a rule specifies stochastic rounding.
- **\[Confirmed\]** Overflow is a simulation error that stops the commit, never a silent wrap.

## 9. Random streams

- **\[Confirmed\]** Random draws come from seeded streams keyed by turn, phase, system and entity, never by iteration order.
- **\[Confirmed\]** A counter-based generator: each draw is a pure function of (world seed, turn, phase, system ID, entity ID, draw index). Candidates are Philox or a SplitMix64-style mixing function. The final choice is fixed in code with published test vectors.
- **\[Confirmed\]** `System.Random` is not used, because its algorithm is not guaranteed to stay the same across .NET versions.
- **\[Confirmed\]** Helpers built on the generator: uniform integers, fixed-point uniform values, Bernoulli and Poisson draws, stochastic rounding.

## 10. Adaptive integration (hooks)

- **\[Confirmed\]** Slow systems integrate in closed form over the full turn. Seasonal cycles use precomputed annual profiles per biome. Random events are Poisson draws (rate x duration).
- **\[Confirmed\]** Provinces flagged volatile (famine, flood, siege, epidemic) may use adaptive substeps at the turn-end tick. Four is the initial engineering budget, not an accuracy ceiling: an unstable system may use more under a deterministic escalation rule, and every escalation is logged for benchmarking (revised 25 Nov 2026).
- **\[Confirmed\]** Circular dependencies get explicit solvers with convergence criteria, iteration caps and fallbacks. Outcomes never depend on execution order. The solvers themselves are Pass 3.
- **\[Confirmed\]** Pass 1 provides the substep driver (splits an interval into 1 to N calendar-exact subintervals, N configurable with a default budget of 4) and the solver interface (iteration cap, tolerance, fallback). Tolerances are set during calibration.

* **\[Confirmed\]** Hazard and rate primitives: integrate per-year rates and hazards over exact elapsed time, with probability within an interval = 1 − exp(−λΔt) and integrated hazards for varying λ. A time-to-event draw schedules rare events in the event queue and is resampled only when the hazard changes (ADR-012).
* **\[Confirmed\]** A propagator cache keyed by legal time step: because there are only eight turn steps and a small set of phase lengths, decay factors and transition matrices are computed once per legal Δt and reused (ADR-011). Pass 1 builds the cache mechanism; real matrices arrive with their systems.

## 11. Input log and replay hash

- **\[Confirmed\]** Identical conditions, rules, seeds and decisions give identical authoritative states. Parallelism, UI, zoom, logging, rendering, the player's view and which country the player controls never change computational detail or results. Each run records a campaign manifest: core build identifier, ruleset version, schema version, active first-party expansions with exact versions and load order, content hashes, execution configuration, world seed and initial-conditions hash (added 25 Nov 2026). Replay requires an identical manifest.
- **\[Confirmed\]** Every phase writes its validated orders (player and AI) to an ordered input log. A replay is the initial state plus the world seed plus the input log.
- **\[Confirmed\]** After each phase and each turn end, the authoritative state is serialized canonically (fixed field order, entities sorted by stable ID, integers in little-endian) and hashed. Candidate: XxHash128 from the System.IO.Hashing package for per-phase hashes, and SHA-256 for save integrity.
- **\[Confirmed\]** A replay passes only if every per-phase hash matches the recorded run.

## 12. Code deliverables (C#)

**\[Confirmed\]** A .NET class library with no Godot dependency, plus a test project. The .NET version is the long-term-support version supported by the chosen Godot 4.x release, confirmed at setup.

| Module | Contents |
| --- | --- |
| Time | Clock, calendar conversion, step table, boundary calculator, step controller |
| Numerics | Fixed-point type, integer math functions, rounding |
| Random | Counter-based generator, draw helpers |
| Events | Event record, ordering key, priority queue, interruption classes |
| Scheduling | System registration, phase and turn-end runners, substep driver, solver interface |
| Orders | Standing-order model and state machine, Advance Until filters |
| Replay | Input log, canonical serializer, hashing |
| Test systems | A counter system, a random-walk system and two mutually dependent dummy systems |

Also in Pass 1 **\[Confirmed\]**: the hazard, rate and propagator-cache primitives (section 10) in the Numerics and Random modules, and a BenchmarkDotNet harness that records a first performance baseline on the reference hardware in Computational Doctrine section 8.

## 13. Tests and exit criteria

**Unit tests \[Confirmed\]:**

- Calendar: BCE/CE round trips, no year zero, leap years at 1600, 1700, 1900 and 2000, epoch conversions.
- Boundaries: every step produces the right phase dates across leap years and month ends.
- Transitions: faster switches align at once, slower switches wait for alignment, hysteresis counts (10, 20, 20) behave as specified.
- Event queue: the same events inserted in shuffled orders always resolve in the same order.
- Random: fixed test vectors, and draws unchanged when unrelated entities are added.
- Numerics: arithmetic test vectors, overflow detection, rounding cases.

* Time-step consistency: a dummy hazard and a dummy decay advanced over the same elapsed time in different legal subdivisions give identical integer results, or documented, bounded differences.
* Property-based tests (FsCheck) for calendar round trips, event ordering and fixed-point arithmetic.
* Parallel consistency: integer reductions give identical results with 1, 2 and 8 worker threads.
* Performance baseline recorded, and later runs fail if they regress beyond a set threshold.

**Exit test (from the roadmap) \[Confirmed\]:** identical replay hashes on two machines.

**Exit test details \[Confirmed\]:**

- Run a synthetic 1,000-turn scenario covering all eight steps and several transitions, with the dummy systems.
- Automated runs on Windows, Linux and macOS (including an ARM processor) in GitHub Actions must produce identical per-phase hashes.
- Running with and without parallelism gives identical hashes.
- Replay from the input log reproduces every recorded hash.
- Convergence: a dummy slow system run at 50-year steps matches the same scenario at 1-year steps within a set tolerance.
- Pass 1 scheduling overhead stays a small fraction of the confirmed 0.2-second ordinary phase budget (target to be set at benchmarking).

## 14. Decisions Taken

1. Date labels: all documents now use BCE/CE, replacing BC/AD and the concept screens' "A.E.". The Game is confirmed to display BCE and CE. Conditional military interruption rules stay open. Pass 1 only builds the hook.
2. The reference undertaking for step transitions is set in the data phase.

Engineering decisions in this spec are recorded in the Architectural Decision Registry: ADR-004 (number formats), ADR-005 (floating-point policy), ADR-006 (random generator), ADR-011 (propagator cache) and ADR-012 (time-to-event sampling). The Computational Doctrine tab is binding for this pass.

## 15. Clarifications (24 Nov 2026)

Answers to build questions. They follow from the rules above and add no new game design.

1. **Step versus stage.** A step is a turn length; a stage is a numbered resolution slot (Terms, section 6).
2. **Boundary instants.** Turns and phases are half-open intervals \[start, end); an event exactly on a boundary belongs to the phase that begins there (section 3).
3. **Turn end in keys.** The turn end has its own fixed slot after the last phase in random-draw keys and stable event IDs, and the substep index is part of the key, so no two substeps share a draw.
4. **Step at the epoch.** The first turn uses the candidate step at once, with no waiting. The 20-turn minimum gap counts from the epoch, which is treated as the first switch.
5. **Hysteresis counting.** The count tracks direction (faster or slower), not the exact candidate. It resets when the candidate equals the current step or changes direction. A switch is always one step. A completed count that meets the 20-turn gap fires on the first turn the gap allows, provided the count still holds then.
6. **Committed switches.** Once the era-transition event is announced, the switch is committed even if the candidate drifts during the alignment wait. The 20-turn gap counts from the turn the switch takes effect.
7. **Ties.** A target exactly halfway between two steps on the log scale goes to the slower step. Targets beyond the table use 50 years or 1 month. The comparison uses nominal lengths in whole seconds (target squared against the product of the two neighbouring steps), so no logarithm is needed.
8. **When a system is due.** Due follows the calendar: a system with a 1-year period runs at the first turn end on or after each 1 January (every 12th turn on the 1-month step) and covers all time since its last run in one closed-form pass (one 50-year pass on the 50-year step). Natural periods are restricted to legal step or phase lengths, so every elapsed interval has a cached propagator.
9. **Advance Until on a mid-phase event.** The phase finishes and control returns at the next phase boundary. The rare in-phase interruption points and conditional military interruptions remain empty hooks in Pass 1.
10. **Era-transition preview in Pass 1.** The preview lists the old and new step, the date the switch takes effect, the new phase count and the first turn's phase dates under the new step. Later passes add game consequences. It is issued when the switch is decided, for faster and slower switches alike. It is an informational event, so Advance Until does not stop for it unless the player adds it to the filters.
11. **Advance Until, turn ends, summaries and auto-skip.** Advance Until runs straight through turn ends and stops only on a stop condition. Turn-end stage 10 (records, summaries and snapshots) produces the turn summary as data in Pass 1; the screen that shows it comes in Pass 10. "Auto-skip" never skips computation: every phase is still resolved and hashed, and a quiet phase simply does not hand control to the player. A run that is stopped and resumed gives the same hashes as an uninterrupted run.
12. **Observer-independent cadence (25 Nov 2026).** System and entity cadence depends only on simulation state. Changing the observed or selected entity, or the player's country, never changes which updates run or their results. Tested in J9.
13. **Substep budget (25 Nov 2026).** The substep driver accepts any N; 4 is the default budget. Escalation beyond the budget follows a deterministic rule based on state and is logged.
14. **Money and aggregates (25 Nov 2026).** 128-bit accumulation for sums and exposures, with overflow tests for denomination change, high inflation and redenomination in J1.
15. **Campaign manifest (25 Nov 2026).** The input log starts with the manifest (section 11). There are no third-party mods; a build from modified source has its own build identifier.
