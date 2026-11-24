# ADR-042: Q32.32 type, rounding and overflow

- **Status:** Proposed (Pass 1, J1). Awaiting owner review.
- **Context:** Spec section 8 and ADR-004 fix the number formats: 64-bit integer base units for counts, money and mass; Q32.32 for rates, probabilities and other dimensionless values; `Int128` intermediates; round-half-to-even unless a rule says stochastic; overflow is a simulation error that stops the commit. J1 has to turn these into code, which meant several engineering choices.

## Decisions

1. **One fixed-point type, `Q32`.** A `readonly struct` that holds a signed 64-bit raw value read as raw / 2^32. The range is -2^31 to 2^31 - 2^-32, in steps of 2^-32 (about 2.3 × 10^-10). It has `+`, `-`, `*`, `/` and negation, comparisons, `FromInt`, `FromRatio` and `FromRaw`. Its text form is the exact decimal value (every Q32 value has at most 32 decimal places), so test failures and reports show true values, not approximations.
2. **Base units stay plain `long` for now.** Counts, money and mass are `long` in base units. Unit-typed wrappers (`People`, `Money` and so on) are ADR-014, which is to be decided in Pass 2 with the state store. J1 adds only the operations that cross between them and `Q32`: count × rate (`Rounding.MultiplyHalfToEven`, `Rounding.MultiplyStochastic`) and a × b ÷ c (`Rounding.MultiplyDivideHalfToEven`) for scale conversions.
3. **Every product and quotient goes through `Int128`, then rounds half to even once.** Multiplication rounds the exact 128-bit product, and division rounds the exact quotient, so the result is the true value rounded half to even. Addition and subtraction are exact.
4. **Overflow throws `SimulationOverflowException`, which derives from `OverflowException`.** Every numerics operation whose result can leave the 64-bit range checks its exact 128-bit result and throws with a message naming the operation and the exact value. (Operations whose range is provably bounded, such as sqrt, log and rounding a Q32 to a whole number, convert directly.) Because it derives from `OverflowException`, a commit boundary (Pass 2 onward) that stops on `OverflowException` also catches the plain checked-arithmetic overflows that ADR-041 turns on everywhere. Division by zero stays the standard `DivideByZeroException`. A logarithm of zero or a negative number, and a square root of a negative number, throw `ArgumentOutOfRangeException`. These are invalid inputs, and the doctrine says invalid states are prevented, never clipped.
5. **The stochastic-rounding hook is a pure function of a 64-bit uniform draw.** `Rounding.Stochastic(value, uniform)` and `Rounding.MultiplyStochastic(count, rate, uniform)` round up exactly when the top 32 bits of the draw are below the fraction (in units of 2^-32). So the chance of rounding up is exactly the fraction, and the expected result equals the exact value. J2 completes the hook by supplying the draw from the keyed random stream; J1 contains no randomness.
6. **A source scan enforces hard rules 2 and 4.** A test reads every C# file in `src/Atlasiv.Core`, ignores comments and string literals, and fails if it finds `float`, `double`, `decimal`, `Half`, `Math`, `MathF`, `BigInteger.Log` or a floating-point literal. `decimal` is banned too: it is not one of the two approved formats.

## Alternatives considered

- **A single fixed-point format for everything.** Rejected by ADR-004: Q32.32 overflows above about 2.1 billion.
- **Unit-typed wrappers now.** Rejected for J1: ADR-014 leaves this to Pass 2, where the state schema declares each quantity's unit and scale.
- **Truncation or round-half-away-from-zero.** Rejected: spec section 8 confirms half to even, which has no drift on average.
- **Letting the plain `OverflowException` from checked arithmetic stand.** Rejected: the error would not say which operation overflowed or what the exact value was. Deriving from `OverflowException` keeps a single type to catch.
- **A stochastic hook that takes a random-stream object.** Rejected: J1 would then depend on J2's types. Passing the draw keeps numerics pure and easy to test.
- **An IL or analyzer check instead of a source scan.** Rejected for now: the source scan is simpler and also catches local variables and literals. J2's `System.Random` check can extend it.

## Consequences

- Q32 results are bit-identical on every machine: there is no floating point, and every rounding is defined.
- The commit boundary (Pass 2 onward) must treat `OverflowException`, and therefore `SimulationOverflowException`, as a failed commit.
- J2 supplies uniform draws to the stochastic-rounding functions.

## Related

ADR-004 (number formats), ADR-005 (floating-point policy), ADR-014 (unit-typed wrappers), ADR-041 (checked arithmetic), ADR-043 (exp, log and sqrt), ADR-044 (hazard primitive).
