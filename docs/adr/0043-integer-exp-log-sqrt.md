# ADR-043: Integer exp, log and sqrt with stated accuracy

- **Status:** Proposed (Pass 1, J1). Awaiting owner review.
- **Context:** Spec section 8 requires transcendental functions to use "integer algorithms or precomputed fixed-point tables with test vectors", because library math functions can differ between platforms. The spec does not say which algorithm, or how accurate the functions must be.

## Decisions

1. **Square root: digit-by-digit integer square root, correctly rounded.** sqrt(x) in Q32 is the integer square root of raw × 2^32, rounded to the nearest integer. The result is exactly the true value rounded (an exact tie is impossible).
2. **Exponential: range reduction plus a Taylor series, at 62 fractional bits.** x = k·ln 2 + r with 0 ≤ r < ln 2, worked out with ln 2 to 96 bits; exp(r) is an 18-term Taylor series by Horner's rule at 62 fractional bits; the result is exp(r)·2^k rounded half to even to Q32. Inputs of 22 or more overflow (exp 22 is above the Q32 maximum). Inputs below -24 return 0, which is the correctly rounded result, not a clip: exp(-24) is below 2^-34, under half of the smallest Q32 step.
3. **Natural logarithm: normalisation plus an atanh series, at 64 fractional bits.** x = m·2^e with m in [1, 2); ln m = 2·atanh((m - 1)/(m + 1)), a 20-term series; ln x = ln m + e·ln 2, rounded half to even to Q32.
4. **Stated accuracy, in raw units of 2^-32:**
   - sqrt: correctly rounded (error at most 1/2).
   - log: within 1 of the correctly rounded value.
   - exp: within 1 of the correctly rounded value, plus 2^-56 relative for large results. So it is within 1 for every result below 2^24, and in particular for all of (0, 1], the range hazards use.
   - Inverses: exp(log x) is within x·2^-31 + 2 of x; log(exp y) is within 2 + 2^32/exp(y)_raw of y; sqrt(x)² is within sqrt(x) + 2 of x; (a·b)/b is within 0.5/|b| + 0.5 of a; and (a/b)·b is within 0.5·|b| + 0.5 of a.
   These bounds follow from the internal precision and are verified, not tuned. Each is checked by FsCheck against a separate 160-bit `BigInteger` reference in the tests (2,000 random cases per run; 100,000 cases were also run once while building).
5. **Published test vectors.** The tests list sqrt, exp, log and hazard inputs with the true results rounded half to even to Q32, computed independently with 70-digit decimal arithmetic. The library reproduces every one exactly, except exp 21 and exp 21.48 (results above 10^9), which come out 1 raw unit high, inside the stated bound.

## Measured cost (Release, x64 development machine, BenchmarkDotNet short run)

| Operation | Mean |
| --- | --- |
| Q32 multiply | 11 ns |
| Q32 divide | 7 ns |
| exp | 111 ns |
| log | 69 ns |
| sqrt | 73 ns |
| hazard probability | 134 ns |

The first exp design divided by a 128-bit number at each series step and took 528 ns. Dividing r by the small step number n first, in 64 bits, cut this to 111 ns with the same accuracy.

## Alternatives considered

- **Lookup tables (for example exp of each 1/64 of ln 2).** Rejected for now: they add precomputed constants that would themselves need verifying, for a gain that matters only if exp becomes hot. ADR-011's propagator cache already avoids repeated exp calls for the legal time steps.
- **The binary-logarithm squaring method for log.** Rejected: it needs about 64 dependent squarings, against 20 short series steps.
- **Requiring correct rounding for exp at every size.** Rejected: very large results would need about 95 bits inside the calculation. The relative bound above is far below any modelling error.
- **A deterministic floating-point profile (ADR-005).** Rejected: not approved, and not needed for Pass 1.

## Consequences

- Any change to these algorithms must keep the published vectors and the stated bounds. A bit change in any vector changes replay hashes, so it is a deliberate, reviewed change.
- J2's time-to-event draw can use `FixedMath.Log` (t = -ln U / λ).

## Related

ADR-004, ADR-005, ADR-011, ADR-042, ADR-044.
