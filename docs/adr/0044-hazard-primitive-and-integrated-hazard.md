# ADR-044: Hazard primitive and exact integrated hazard

- **Status:** Proposed (Pass 1, J1). Awaiting owner review.
- **Context:** Spec section 10 asks for the hazard primitive, probability within an interval = 1 - exp(-λΔt), and "integrated hazards for varying λ", integrating per-year rates over exact elapsed time. Spec section 13 asks that the same elapsed time split into different legal subdivisions gives identical integer results, or documented, bounded differences. ADR-039 ruling 10 fixes rate time as elapsed seconds ÷ 31,556,952.

## Decisions

1. **Rates are per rate year; time is exact seconds.** λ is a non-negative `Q32` (events per rate year) and Δt is a non-negative `long` count of seconds. One rate year is `RateYear.Seconds` = 31,556,952 s. J3's clock can use the same constant.
2. **Varying λ is integrated piece by piece, exactly.** `IntegratedHazard` keeps the sum of λ_raw × seconds for each piece in a 128-bit integer. It divides by 31,556,952 and rounds half to even only once, when the hazard H is read. Because integer addition is exact and order-free, any split of the same interval (for example twelve calendar months against one year), in any order, gives bit-identical H, survival and probability. This meets spec section 13 with no bounded differences to document. A rate that changes within a piece is approximated by its value for that piece. Systems choose the pieces, for example one per substep (spec section 10).
3. **Outputs.** `Value` is H rounded to Q32. `Survival` is exp(-H). `Probability` is 1 - exp(-H). The convenience function `Hazard.Probability(λ, Δt)` handles one constant piece.
4. **Huge hazards are certain, not errors.** When H ≥ 30, exp(-H) is below 2^-43 and rounds to 0, so Survival = 0 and Probability = 1 exactly. This holds even when H is too large to fit in Q32. Reading `Value` in that case throws `SimulationOverflowException`, because the number itself cannot be represented.
5. **Negative rates or times are invalid** and throw `ArgumentOutOfRangeException`.

## Alternatives considered

- **Multiplying per-piece survival factors.** Rejected: each product rounds, so the result would depend on how the interval is split.
- **Converting each piece to rate years before adding.** Rejected: each conversion rounds, with the same problem.
- **Integrating a continuously varying λ (for example trapezoids).** Not needed yet. The spec says rates are integrated "over exact elapsed time", and piecewise-constant pieces cover substeps and event-driven rate changes. Later passes can add a closed form for a specific rate curve if a mechanic needs one.
- **Throwing an error when H is too large.** Rejected: a certain event is a valid outcome, not a numerical failure.

## Consequences

- Time-step consistency for hazards is exact, so J6's and J9's consistency tests should see identical results for any legal subdivision.
- Precision: the probability is within 1 raw unit (about 2.3 × 10^-10) of the true 1 - exp(-H). For very rare events per phase, Q32 cannot resolve the probability finely. Those events should use the time-to-event draws of ADR-012 (J2).

## Related

ADR-011 (propagator cache), ADR-012 (time-to-event sampling), ADR-039 ruling 10 (rate year), ADR-042, ADR-043.
