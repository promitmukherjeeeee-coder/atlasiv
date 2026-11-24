# ADR-039: Pass 1 time-rule clarifications

- **Status:** Proposed. These are owner rulings from the 24 Nov 2026 grilling session, to be copied into the master spec at its next revision.
- **Context:** The Pass 1 spec (`docs/04_pass1_spec.md`) leaves several behaviours of the clock, step controller, scheduler and Advance Until open. The code has to pick one reading. The owner chose these, and every Pass 1 job cites this record.

## Rulings

1. **Stage, not step (section 6).** Each system registers in exactly one stage: one of the 11 phase stages or one of the 10 turn-end stages. It runs only in that stage, and only when its natural period is due. The spec's phrase "one of the steps above" is a wording error. A **substep** is one of 1 to 4 calendar-exact slices that a volatile province can use within a turn-end stage. Example: a climate system registers in turn-end stage 1 with a yearly period. A diplomacy system registers in phase stage 5 and runs every phase.
2. **Boundaries are half-open.** Every turn and phase contains its start instant but not its end instant, so every whole-second timestamp belongs to exactly one phase. An event at 00:00 on 1 July belongs to the phase that starts then. At a turn boundary, the ending turn completes all its turn-end stages before any event of the new turn's first phase resolves.
3. **Turn numbering.** Turns count from 1 at the epoch and never reset, including across step switches. Phases are numbered 1 to N within their turn. Pass 1 does not enforce the 10,000-turn cap, which is a design budget, not a clock rule.
4. **Random and event keys at the turn end.** The turn end has its own slot after the last phase in the (turn, phase) part of random-stream keys and stable event IDs. Each substep is part of the key, so no two substeps share a draw. The exact encoding is fixed in J2.
5. **Initial step.** The first turn starts on the candidate step immediately, with no hysteresis wait. The 20-turn minimum gap before the first switch counts from the epoch.
6. **Hysteresis counting (section 4).**
   - (a) The count tracks direction (faster or slower than the current step), not the exact candidate. It resets whenever the candidate equals the current step or changes direction. A switch moves one step only.
   - (b) If the count completes inside the 20-turn gap, the switch fires at the first turn the gap allows, but only if the count still holds then. If the candidate has returned to the current step, the count resets and there is no switch.
   - (c) Once the era-transition event is announced, the switch is committed. A slower switch that waits for 1 January or 1 July is not cancelled if the candidate drifts back. The 20-turn gap counts from the date the switch takes effect.
7. **Ties go to the slower step.** If the target falls exactly halfway between two steps on the log scale, the slower step is the candidate. Targets beyond either end of the step table use the end step (50 years or 1 month). The comparison uses nominal step lengths and exact integer arithmetic, with no logarithm.
8. **"Due" follows the calendar.** A system with natural period P runs at the first opportunity in its stage on or after each calendar boundary of P (for example, a yearly turn-end system runs at the first turn end on or after each 1 January). When it runs, it covers all the time since its last run in one pass. A natural period must be one of the legal step or phase lengths.
9. **Advance Until stops at phase boundaries.** A decision-critical event that fires mid-phase stops Advance Until at the end of that phase. Interruption points inside a phase, and the conditional military interruptions, stay empty hooks in Pass 1.
10. **Year length for rates.** Elapsed seconds ÷ 31,556,952 is Confirmed as part of the confirmed Pass 1 spec. A 365-day year counts as 0.99934 years and a 366-day year as 1.00207 years. The heading "[Confirmed and Confirmed]" in spec section 3 is a typo for the owner to fix.
11. **Era-transition preview in Pass 1.** The preview lists the old and new step, the date the switch takes effect, the new number of phases per turn, and the phase dates of the first turn under the new step. For a slower switch, the preview goes out the moment the switch is decided. Later passes add game consequences.
12. **Advance Until never skips computation.** Every phase is fully resolved and hashed. "Auto-skip" in the vision only means a quiet phase does not hand control to the player. Advance Until runs through turn ends and stops only on its stop list. The turn summary data is built in Pass 1 as turn-end stage 10 ("records, summaries and snapshots"). Only its display screen comes later (Pass 10). A run that is stopped and resumed must give the same hashes as an uninterrupted run.

## Alternatives considered

- Exact-candidate hysteresis counting, which resets when the candidate changes between two faster steps. Rejected: slower to react, with no gain in stability.
- Cancelling an announced slower switch. Rejected: an announced era transition that never happens confuses the player.
- Ties to the faster step. Rejected: it costs more turns, and hysteresis already guards against flapping.
- Elapsed-time "due" (period counted from the last run). Rejected: drifts against the calendar across step switches.

## Related

ADR-006 (random generator keys), ADR-011 (propagator cache per legal time step), ADR-012 (time-to-event sampling).
