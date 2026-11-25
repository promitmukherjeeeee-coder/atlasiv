# Engineering decisions log

Engineering decisions taken while building ATLASIV, each with its alternatives and reason. An entry stays **Proposed** until the owner reviews it.

**Numbering.** Entries here are numbered `ENG-001`, `ENG-002` and so on. `ADR-` numbers belong only to the owner's Architectural Decision Registry, `docs/07_architectural_decision_registry.md`, and are never used here. If an entry deserves promotion to the registry, the owner gives it an `ADR-` number there and the entry here points to it.

**Precedence.** The design documents win over this log. If a later doc revision covers an entry, the doc applies and the entry is marked superseded.

---

## ENG-001: Where decision records live, and how they are numbered

- **Status:** Proposed, 25 Nov 2026. Decided by Claude at the owner's request ("decide on the number clash").
- **Context:** The repository held `docs/adr/0001` to `0038` (the owner's registry split into one file per decision) and `0039` to `0041` (records written during J0). The 25 Nov 2026 doc update made `docs/07` the registry, gave the numbers ADR-039 to ADR-044 to new owner decisions (campaign manifest, observer-independent cadence, substep budget, money tests, nominal and real separation, decision hierarchy), and moved engineering decisions to this log. The repository's ADR-039 to ADR-041 therefore named different decisions from the registry's.

### Decision

1. `docs/07` is the only source of `ADR-` numbers. The `docs/adr/` folder is removed: its files 0001 to 0038 were copies of the registry that the 25 Nov update had already made stale (for example ADR-036's 25 Nov amendment), and keeping them would mean two versions of the same decision.
2. Engineering decisions use `ENG-` numbers in this one file. No ID family in the design docs or registries uses `ENG-`.
3. The repository's former records move here:
   - former repo ADR-040 (.NET 10) becomes ENG-002;
   - former repo ADR-041 (build, test and CI settings) becomes ENG-003;
   - former repo ADR-039 (owner's time-rule rulings of 24 Nov 2026) is superseded by spec sections 3, 6 and 15, which now contain those rulings. The one ruling the spec does not yet contain is kept below under "Owner rulings not yet in the spec".
4. Code comments cite `ENG-` numbers for engineering decisions and `ADR-` numbers only for registry decisions.

### Alternatives considered

- **Renumber the repository's records above the registry (ADR-045 onward).** Rejected: the registry will keep growing, so the clash would come back, and CLAUDE.md now asks for a single log.
- **Keep `docs/adr/` for the registry copies only.** Rejected: copies of a read-only document drift out of date, and `docs/07` is the export the owner maintains.
- **Leave the old files and add a note.** Rejected: anyone reading "ADR-041" would still find two different decisions.

---

## ENG-002: Target .NET 10 (LTS)

- **Status:** Proposed (Pass 1, J0, 24 Nov 2026). Formerly repo ADR-040.
- **Context:** Spec section 12 and PASS1_PLAN J0 ask for "the long-term-support version that the current stable Godot 4.x .NET build supports", with the source checked. ADR-001 puts the simulation core in a headless .NET library; ADR-002 puts the client in Godot 4 with C#.

### Decision

Every project targets `net10.0`. The SDK is pinned in `global.json` to the 10.0 band (`10.0.100`, rolling forward to the latest installed 10.0 feature band, no previews).

### Evidence (checked 24 Nov 2026)

- **.NET 8 is out of support.** .NET 8 (LTS) and .NET 9 (STS) both reached end of support on 10 Nov 2026. .NET 10 is the current LTS release, supported until Nov 2028.
  Sources: Microsoft, ".NET and .NET Core Support Policy", <https://dotnet.microsoft.com/platform/support/policy/dotnet-core>; .NET Blog, ".NET 8 and .NET 9 end of support", <https://devblogs.microsoft.com/dotnet/dotnet-8-9-end-of-support/>.
- **Godot 4.8 requires .NET 10.** Godot 4.8 raises the minimum .NET version for C# projects from 8 to 10 (GH-123738, "Upgrade packages and minimum TFM required to net10.0").
  Sources: "Dev snapshot: Godot 4.8 dev 7", <https://godotengine.org/article/dev-snapshot-godot-4-8-dev-7/>; "Dev snapshot: Godot 4.8 beta 4", <https://godotengine.org/article/dev-snapshot-godot-4-8-beta-4/>.
- **Godot 4.7 also works with .NET 10.** On the day of this record, the newest stable Godot is 4.7.2 (18 Aug 2026), and 4.8 is at beta 4 (18 Nov 2026), with stable expected in Q4 2026. Godot's C# packages set only a *minimum* target framework, and projects may target a newer one, so a .NET 10 library works with 4.7 as well as 4.8.
  Sources: Godot download archive, <https://godotengine.org/download/archive/>; Godot release policy, <https://docs.godotengine.org/en/stable/about/release_policy.html>; Godot docs, "C# basics" ("The version targeted by Godot's C# packages is only the minimum version that your project can target, but you are always free to target a newer .NET version"), <https://docs.godotengine.org/en/latest/tutorials/scripting/c_sharp/c_sharp_basics.html>.

### Alternatives considered

- **.NET 8.** Rejected: out of support since 10 Nov 2026, and below Godot 4.8's minimum.
- **.NET 9.** Rejected: a short-term release, also out of support since 10 Nov 2026.
- **.NET 11 (preview).** Rejected: not released, not LTS, and not supported by any stable Godot.

### Consequences

- Pass 1 does not touch Godot, so the 4.7 to 4.8 question does not block any Pass 1 job. When the client starts, it uses Godot 4.8 or later (4.7 also works with this choice).
- The next LTS move (to .NET 12) is a later decision, expected before Nov 2028.

Related: ADR-001, ADR-002, ENG-003.

---

## ENG-003: Build, test and CI settings

- **Status:** Proposed (Pass 1, J0, 24 Nov 2026). Formerly repo ADR-041. Decision 6 is an owner decision.
- **Context:** PASS1_PLAN J0 asks for deterministic build settings (warnings as errors, invariant culture in tests) and CI on Windows, Linux, macOS x64 and macOS ARM. Hard rule 1 in `CLAUDE.md` requires bit-identical results on every machine. Several tooling choices had to be made to get there.

### Decisions

1. **Shared settings in one file.** `Directory.Build.props` sets the target framework, nullable reference types, `Deterministic`, `ContinuousIntegrationBuild` (on CI), `TreatWarningsAsErrors`, and the .NET analyzers at the `latest-recommended` level with code style enforced in the build. A project file can still override these, so any override must sit visibly in that project file and be reviewed. The library also writes its XML documentation file, so a public type without documentation raises a warning, and therefore an error.
2. **Invariant culture everywhere.** `InvariantGlobalization` is on for every project, not only the tests. The setting takes effect in the program that runs, so it binds the tests and benchmarks now. In the library it records intent only: the host program (later, the Godot client and the headless command line) must turn it on too, and authoritative code must still pass `CultureInfo.InvariantCulture` wherever it formats or parses text. A test checks that the current culture is invariant.
3. **Exact package versions in one place.** `Directory.Packages.props` (central package management) pins every NuGet package to one exact version, so every machine restores the same packages.
4. **xunit v3 on the Microsoft Testing Platform.** Tests use xunit v3 with FsCheck 3 (`FsCheck.Xunit.v3`). `dotnet test` runs on the Microsoft Testing Platform, set in `global.json`, because the .NET 10 SDK no longer runs xunit v3 through the older VSTest runner.
5. **Two narrow analyzer exceptions, in `.editorconfig`.** Test method names may use underscores (CA1707), because they read as sentences. Benchmark methods may be instance methods (CA1822), because BenchmarkDotNet requires it. Neither exception applies to the simulation library.
6. **Checked arithmetic everywhere (owner decision, 24 Nov 2026).** `CheckForOverflowUnderflow` is on for every project, so any integer operation that overflows throws an error instead of silently wrapping (hard rule 2). Code that must wrap on purpose, such as a hash or random-number mixer, marks that code `unchecked` explicitly, where a reviewer can see it. Tests confirm that addition, multiplication and narrowing conversions throw on overflow.
7. **Line endings.** `.gitattributes` stores and checks out every text file with LF endings on every machine, so a file that a test reads is byte-identical on Windows, Linux and macOS.
8. **CI matrix.** One GitHub Actions workflow (`.github/workflows/ci.yml`) restores, builds in Release and tests on `windows-latest`, `ubuntu-latest`, `macos-15-intel` (macOS x64) and `macos-latest` (macOS ARM64). `fail-fast` is off, so one failing runner does not hide the others. Each runner checks that its processor is the expected one (X64 or ARM64) and fails if not, so a relabelled GitHub image cannot silently drop a platform. Test results are uploaded from every runner.

### Alternatives considered

- **xunit v2 with VSTest.** Rejected: xunit v2 is in maintenance only, and v3 is the supported line for .NET 10.
- **NUnit or MSTest.** Rejected: the plan names xUnit with FsCheck.
- **Floating package versions (for example `4.*`).** Rejected: two machines could restore different versions.
- **Invariant culture only in tests.** Rejected: turning it on everywhere costs nothing and states the intent in every project.
- **A pinned SDK patch and pinned runner images.** Rejected for now: `global.json` rolls forward within .NET 10, and the images use `-latest` labels, so the compiler and OS can change between runs. This does not threaten hard rule 1, which is about identical simulation results, not identical binaries. J9's cross-runner replay hashes are the guard. Revisit if a hash difference is ever traced to the toolchain.
- **Checked arithmetic only inside the numerics types (J1).** Rejected by the owner: any plain integer operation anywhere in the code could still wrap silently. A small speed cost on arithmetic is accepted.
- **CLS-compliance attributes.** Not used: the library has no public extension API (hard rule 7), so there is no consumer in another .NET language to promise compatibility to.
- **`macos-13` for macOS x64.** Rejected: GitHub has retired that image; `macos-15-intel` is the current Intel image.

### Consequences

- Any new warning breaks the build on every runner. Fixing it is part of the change that caused it.
- Package upgrades are deliberate, one-line changes in `Directory.Packages.props`.
- J9 adds the cross-runner hash comparison to this workflow.

Related: ADR-004, ADR-006, ENG-002.

---

## ENG-004: Q32.32 type, rounding and overflow

- **Status:** Proposed (Pass 1, J1, 24 Nov 2026). Formerly J1 draft record "ADR-042", renumbered before merge (ENG-001).
- **Context:** Spec section 8 and ADR-004 fix the number formats: 64-bit integer base units for counts, money and mass; Q32.32 for rates, probabilities and other dimensionless values; `Int128` intermediates; round-half-to-even unless a rule says stochastic; overflow is a simulation error that stops the commit. J1 has to turn these into code, which meant several engineering choices.

### Decisions

1. **One fixed-point type, `Q32`.** A `readonly struct` that holds a signed 64-bit raw value read as raw / 2^32. The range is -2^31 to 2^31 - 2^-32, in steps of 2^-32 (about 2.3 × 10^-10). It has `+`, `-`, `*`, `/` and negation, comparisons, `FromInt`, `FromRatio` and `FromRaw`. Its text form is the exact decimal value (every Q32 value has at most 32 decimal places), so test failures and reports show true values, not approximations.
2. **Base units stay plain `long` for now.** Counts, money and mass are `long` in base units. Unit-typed wrappers (`People`, `Money` and so on) are ADR-014, which is to be decided in Pass 2 with the state store. J1 adds only the operations that cross between them and `Q32`: count × rate (`Rounding.MultiplyHalfToEven`, `Rounding.MultiplyStochastic`) and a × b ÷ c (`Rounding.MultiplyDivideHalfToEven`) for scale conversions.
3. **Every product and quotient goes through `Int128`, then rounds half to even once.** Multiplication rounds the exact 128-bit product, and division rounds the exact quotient, so the result is the true value rounded half to even. Addition and subtraction are exact.
4. **Overflow throws `SimulationOverflowException`, which derives from `OverflowException`.** Every numerics operation whose result can leave the 64-bit range checks its exact 128-bit result and throws with a message naming the operation and the exact value. (Operations whose range is provably bounded, such as sqrt, log and rounding a Q32 to a whole number, convert directly.) Because it derives from `OverflowException`, a commit boundary (Pass 2 onward) that stops on `OverflowException` also catches the plain checked-arithmetic overflows that ENG-003 turns on everywhere. Division by zero stays the standard `DivideByZeroException`. A logarithm of zero or a negative number, and a square root of a negative number, throw `ArgumentOutOfRangeException`. These are invalid inputs, and the doctrine says invalid states are prevented, never clipped.
5. **The stochastic-rounding hook is a pure function of a 64-bit uniform draw.** `Rounding.Stochastic(value, uniform)` and `Rounding.MultiplyStochastic(count, rate, uniform)` round up exactly when the top 32 bits of the draw are below the fraction (in units of 2^-32). So the chance of rounding up is exactly the fraction, and the expected result equals the exact value. J2 completes the hook by supplying the draw from the keyed random stream; J1 contains no randomness.
6. **A source scan enforces hard rules 2 and 4.** A test reads every C# file in `src/Atlasiv.Core`, ignores comments and string literals, and fails if it finds `float`, `double`, `decimal`, `Half`, `Math`, `MathF`, `BigInteger.Log` or a floating-point literal. `decimal` is banned too: it is not one of the two approved formats.

### Alternatives considered

- **A single fixed-point format for everything.** Rejected by ADR-004: Q32.32 overflows above about 2.1 billion.
- **Unit-typed wrappers now.** Rejected for J1: ADR-014 leaves this to Pass 2, where the state schema declares each quantity's unit and scale.
- **Truncation or round-half-away-from-zero.** Rejected: spec section 8 confirms half to even, which has no drift on average.
- **Letting the plain `OverflowException` from checked arithmetic stand.** Rejected: the error would not say which operation overflowed or what the exact value was. Deriving from `OverflowException` keeps a single type to catch.
- **A stochastic hook that takes a random-stream object.** Rejected: J1 would then depend on J2's types. Passing the draw keeps numerics pure and easy to test.
- **An IL or analyzer check instead of a source scan.** Rejected for now: the source scan is simpler and also catches local variables and literals. J2's `System.Random` check can extend it.

### Consequences

- Q32 results are bit-identical on every machine: there is no floating point, and every rounding is defined.
- The commit boundary (Pass 2 onward) must treat `OverflowException`, and therefore `SimulationOverflowException`, as a failed commit.
- J2 supplies uniform draws to the stochastic-rounding functions.

Related: ADR-004, ADR-005, ADR-014, ENG-003, ENG-005, ENG-006, ENG-007.

---

## ENG-005: Integer exp, log and sqrt with stated accuracy

- **Status:** Proposed (Pass 1, J1, 24 Nov 2026). Formerly J1 draft record "ADR-043", renumbered before merge (ENG-001).
- **Context:** Spec section 8 requires transcendental functions to use "integer algorithms or precomputed fixed-point tables with test vectors", because library math functions can differ between platforms. The spec does not say which algorithm, or how accurate the functions must be.

### Decisions

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

### Measured cost (Release, x64 development machine, BenchmarkDotNet short run)

| Operation | Mean |
| --- | --- |
| Q32 multiply | 11 ns |
| Q32 divide | 7 ns |
| exp | 111 ns |
| log | 69 ns |
| sqrt | 73 ns |
| hazard probability | 134 ns |

The first exp design divided by a 128-bit number at each series step and took 528 ns. Dividing r by the small step number n first, in 64 bits, cut this to 111 ns with the same accuracy.

### Alternatives considered

- **Lookup tables (for example exp of each 1/64 of ln 2).** Rejected for now: they add precomputed constants that would themselves need verifying, for a gain that matters only if exp becomes hot. ADR-011's propagator cache already avoids repeated exp calls for the legal time steps.
- **The binary-logarithm squaring method for log.** Rejected: it needs about 64 dependent squarings, against 20 short series steps.
- **Requiring correct rounding for exp at every size.** Rejected: very large results would need about 95 bits inside the calculation. The relative bound above is far below any modelling error.
- **A deterministic floating-point profile (ADR-005).** Rejected: not approved, and not needed for Pass 1.

### Consequences

- Any change to these algorithms must keep the published vectors and the stated bounds. A bit change in any vector changes replay hashes, so it is a deliberate, reviewed change.
- J2's time-to-event draw can use `FixedMath.Log` (t = -ln U / λ).

Related: ADR-004, ADR-005, ADR-011, ENG-004, ENG-006.

---

## ENG-006: Hazard primitive and exact integrated hazard

- **Status:** Proposed (Pass 1, J1, 24 Nov 2026). Formerly J1 draft record "ADR-044", renumbered before merge (ENG-001).
- **Context:** Spec section 10 asks for the hazard primitive, probability within an interval = 1 - exp(-λΔt), and "integrated hazards for varying λ", integrating per-year rates over exact elapsed time. Spec section 13 asks that the same elapsed time split into different legal subdivisions gives identical integer results, or documented, bounded differences. Spec section 3 (Confirmed) fixes rate time as elapsed seconds ÷ 31,556,952.

### Decisions

1. **Rates are per rate year; time is exact seconds.** λ is a non-negative `Q32` (events per rate year) and Δt is a non-negative `long` count of seconds. One rate year is `RateYear.Seconds` = 31,556,952 s. J3's clock can use the same constant.
2. **Varying λ is integrated piece by piece, exactly.** `IntegratedHazard` keeps the sum of λ_raw × seconds for each piece in a 128-bit integer. It divides by 31,556,952 and rounds half to even only once, when the hazard H is read. Because integer addition is exact and order-free, any split of the same interval (for example twelve calendar months against one year), in any order, gives bit-identical H, survival and probability. This meets spec section 13 with no bounded differences to document. A rate that changes within a piece is approximated by its value for that piece. Systems choose the pieces, for example one per substep (spec section 10).
3. **Outputs.** `Value` is H rounded to Q32. `Survival` is exp(-H). `Probability` is 1 - exp(-H). The convenience function `Hazard.Probability(λ, Δt)` handles one constant piece.
4. **Huge hazards are certain, not errors.** When H ≥ 30, exp(-H) is below 2^-43 and rounds to 0, so Survival = 0 and Probability = 1 exactly. This holds even when H is too large to fit in Q32. Reading `Value` in that case throws `SimulationOverflowException`, because the number itself cannot be represented.
5. **Negative rates or times are invalid** and throw `ArgumentOutOfRangeException`.

### Alternatives considered

- **Multiplying per-piece survival factors.** Rejected: each product rounds, so the result would depend on how the interval is split.
- **Converting each piece to rate years before adding.** Rejected: each conversion rounds, with the same problem.
- **Integrating a continuously varying λ (for example trapezoids).** Not needed yet. The spec says rates are integrated "over exact elapsed time", and piecewise-constant pieces cover substeps and event-driven rate changes. Later passes can add a closed form for a specific rate curve if a mechanic needs one.
- **Throwing an error when H is too large.** Rejected: a certain event is a valid outcome, not a numerical failure.

### Consequences

- Time-step consistency for hazards is exact, so J6's and J9's consistency tests should see identical results for any legal subdivision.
- Precision: the probability is within 1 raw unit (about 2.3 × 10^-10) of the true 1 - exp(-H). For very rare events per phase, Q32 cannot resolve the probability finely. Those events should use the time-to-event draws of ADR-012 (J2).

Related: ADR-011, ADR-012, ENG-004, ENG-005.

---

## ENG-007: 128-bit accumulation and money overflow tests

- **Status:** Proposed (Pass 1, J1, 25 Nov 2026).
- **Context:** Spec section 8 and clarification 14 (added 25 Nov 2026) require sums, aggregates and exposures to accumulate in 128-bit integers, and overflow tests for money through denomination change, high inflation, redenomination and large aggregate exposures. Registry ADR-042 defers the per-currency scale question to Pass 2, to be decided "using those tests".

### Decisions

1. **One total type, `Accumulator`.** A `readonly struct` holding an `Int128` total, in the same style as `IntegratedHazard`. `Add(amount)` adds a 64-bit amount (negative for debits and liabilities); `AddProduct(quantity, price)` adds an exact product such as an exposure (at most 2^126, so always exact); `Add(other)` combines partial totals; `ToInt64()` reads the total back as 64 bits. Integer addition is exact, so order and grouping never change the result, which keeps parallel partial totals deterministic.
2. **Leaving 128 bits is a simulation overflow.** The check is done explicitly and the error names the operation and both terms. In practice it needs more than 2^64 amounts, or two exposures near the 64-bit limit in both quantity and price. Reading a total that does not fit 64 bits is also a simulation overflow.
3. **Money conversions reuse `Rounding.MultiplyDivideHalfToEven`.** A denomination change or redenomination is amount × b ÷ c, rounded half to even. J1 adds no money-specific type: unit-typed wrappers are ADR-014, decided in Pass 2.
4. **The stress tests use historical cases as test data** (not game values): UK decimalisation in 1971 (240 old pence = 100 new pence), Hungary's 1946 100 million b.-pengő note, Zimbabwe's 100 trillion dollar note and its 2008 redenomination, and Turkey's 2005 redenomination.

### Findings for the Pass 2 decision (registry ADR-042)

- **Single amounts.** 64-bit minor units hold up to about 9.22 × 10^18. Hungary's 1946 note was 10^22 fillér, so one banknote does not fit, and converting it is a simulation overflow. Zimbabwe's 100 trillion dollar note (10^16 cents) fits, but 923 of them do not. Totals of either fit easily in an `Accumulator`.
- **Inflation.** Compounding a price is exact at every step until the true value passes the 64-bit range, and then it is a simulation overflow, never a wrap (tested on random prices and factors). Doubling from one minor unit fits 62 times.
- **Conversion ratios.** A ratio is two 64-bit integers. Hungary's 1946 switch to the forint (1 forint = 4 × 10^29 pengő) cannot be written as one such ratio; it would need two steps or a wider ratio. Not tested, because the input amount cannot be stored in the first place.
- **Rate factors (untested observation).** A per-period inflation factor in Q32 must be below about 2.1 billion. At Hungary's July 1946 peak (prices doubling about every 15 hours) the monthly factor was about 4 × 10^14 (about 48 doublings), so it fits only per phase on short steps, or as a per-year rate (about 400 per year) passed through `FixedMath.Exp` per phase.
- **Redenomination is not total-preserving.** Converting balances one by one rounds each, so their new sum can differ from the converted old total by up to half a new unit per balance (tested). A double-entry ledger must post that residual somewhere (Pass 3).

### Alternatives considered

- **128-bit balances everywhere.** Rejected: the spec fixes 64-bit minor units for amounts, and 128-bit balances double memory for every stored amount. Only totals need the range.
- **`BigInteger` totals.** Rejected: they allocate, have no upper limit to report as an error, and are slower.
- **Letting `Int128`'s own checked addition throw.** Rejected: its plain `OverflowException` would not say which total overflowed or by how much.
- **Adding a per-currency scale now.** Rejected: the spec leaves that decision to Pass 2, using these tests.

### Consequences

- Pass 2 and Pass 3 sum ledgers, statistics and exposures with `Accumulator`. `IntegratedHazard` already keeps its own 128-bit sum for the same reason.
- The findings above are the evidence for the per-currency scale decision.

Related: ADR-004, ADR-014, ADR-042 (registry), ENG-004.

---

## Owner rulings not yet in the spec

Rulings the owner gave that the spec does not yet state. They bind the code until a spec revision covers them; the spec then wins.

- **Turn numbering (24 Nov 2026, formerly repo ADR-039 ruling 3).** Turns count from 1 at the epoch and never reset, including across step switches. Phases are numbered 1 to N within their turn. Pass 1 does not enforce the 10,000-turn cap, which is a design budget, not a clock rule.
