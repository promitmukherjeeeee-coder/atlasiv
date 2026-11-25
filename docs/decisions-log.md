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
   - former repo ADR-039 (owner's time-rule rulings of 24 Nov 2026) is superseded by spec sections 3, 6 and 15, which now contain those rulings. Its one ruling the spec did not yet contain (turn numbering) was added to spec section 15 as clarification 16 on 25 Nov 2026, at the owner's request.
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
9. **A missing ARM machine is not a failure (owner decision, 25 Nov 2026).** GitHub sometimes has no macOS ARM machine free, and the job then never starts. A final job, **CI result**, gives the verdict for the run: each runner passes only if its Test step ran and succeeded; macOS ARM64 with no machine assigned is a warning, not a failure; a missing machine on any other runner, a test failure on any runner (ARM included), or fewer than four runners is a failure. The ARM job is marked `continue-on-error` so that its missing machine does not fail the run either. The script was checked against two past runs (ARM with no machine, all four passing) and a simulated ARM test failure.

### Alternatives considered

- **xunit v2 with VSTest.** Rejected: xunit v2 is in maintenance only, and v3 is the supported line for .NET 10.
- **NUnit or MSTest.** Rejected: the plan names xUnit with FsCheck.
- **Floating package versions (for example `4.*`).** Rejected: two machines could restore different versions.
- **Invariant culture only in tests.** Rejected: turning it on everywhere costs nothing and states the intent in every project.
- **A pinned SDK patch and pinned runner images.** Rejected for now: `global.json` rolls forward within .NET 10, and the images use `-latest` labels, so the compiler and OS can change between runs. This does not threaten hard rule 1, which is about identical simulation results, not identical binaries. J9's cross-runner replay hashes are the guard. Revisit if a hash difference is ever traced to the toolchain.
- **Checked arithmetic only inside the numerics types (J1).** Rejected by the owner: any plain integer operation anywhere in the code could still wrap silently. A small speed cost on arithmetic is accepted.
- **CLS-compliance attributes.** Not used: the library has no public extension API (hard rule 7), so there is no consumer in another .NET language to promise compatibility to.
- **`macos-13` for macOS x64.** Rejected: GitHub has retired that image; `macos-15-intel` is the current Intel image.
- **`continue-on-error` on the ARM job alone (for decision 9).** Rejected: a real ARM test failure would then also stop counting, which would hide a cross-platform determinism break (hard rule 1).
- **Dropping macOS ARM from CI.** Rejected: J0 and J9 require all four platforms.

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
4. **Overflow throws `SimulationOverflowException`, which derives from `OverflowException`.** Every numerics operation whose result can leave the 64-bit range checks its exact 128-bit result and throws with a message naming the operation and the exact value. (Operations whose range is provably bounded, such as sqrt, log and rounding a Q32 to a whole number, convert directly.) Because it derives from `OverflowException`, a commit boundary (Pass 2 onward) that stops on `OverflowException` also catches the plain checked-arithmetic overflows that ENG-003 turns on everywhere. Division by zero stays the standard `DivideByZeroException`. Impossible inputs (a logarithm of zero or a negative number, a square root of a negative number, a negative hazard rate or elapsed time) throw `SimulationDomainException`, which derives from `ArgumentOutOfRangeException`. The doctrine says invalid states are prevented, never clipped. **Owner decision, 25 Nov 2026: an impossible input stops the commit exactly as an overflow does.** So the commit boundary (Pass 2 onward) treats `OverflowException` and `SimulationDomainException` alike as a failed commit. Internal guards against programming mistakes (for example an out-of-range shift count inside `WideMath`) keep the plain `ArgumentOutOfRangeException`.
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
- The commit boundary (Pass 2 onward) must treat `OverflowException` (and therefore `SimulationOverflowException`) and `SimulationDomainException` as a failed commit.
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

## ENG-008: Random generator (Philox4x64-10) and the draw-key layout

- **Status:** Proposed (Pass 1, J2, 25 Nov 2026). Records the choice that spec section 9 and registry ADR-006 leave to Pass 1.
- **Context:** Spec section 9 (Confirmed) requires a counter-based generator in which each draw is a pure function of (world seed, turn, phase, system ID, entity ID, draw index), with Philox or a SplitMix64-style mixer as candidates, the choice fixed in code with published test vectors, and no `System.Random`. Clarification 3 adds that the turn end has its own fixed slot after the last phase, and that the substep index is part of the key, so no two substeps share a draw. (Issue #6 cites "ADR-039 §4"; that was the repository's former record of the owner's 24 Nov rulings, now superseded by clarification 3, per ENG-001.) Pass 1 uses placeholder 64-bit entity IDs; Pass 2 brings the permanent ones.

### Decisions

1. **Generator: Philox4x64-10**, exactly as in the reference implementation Random123 (Salmon, Moraes, Dror and Shaw, "Parallel random numbers: as easy as 1, 2, 3", SC 2011). It maps a 256-bit counter and a 128-bit key to 256 output bits, keeps no state, and needs only 64-bit multiplies, XORs and additions, so it is bit-identical on every processor.
2. **Key layout, with no hashing.** Every field gets its own space in Philox's input:

   | Philox input | Contents |
   | --- | --- |
   | key word 0 | world seed (64 bits) |
   | key word 1 | system ID (32 bits, low half); high 32 bits zero, reserved |
   | counter word 0 | draw index ÷ 4 (the block number) |
   | counter word 1 | entity ID (64 bits) |
   | counter word 2 | turn (counts from 1; 64 bits) |
   | counter word 3 | slot (high 32 bits) and substep (low 32 bits) |

   Draw index *i* is output word *i* mod 4 of block *i* ÷ 4, so one Philox call serves four draws.
3. **Slot.** Phase *n* of a turn is slot *n* (phases count from 1). The turn end is slot 4,294,967,295 (2^32 − 1), above every possible phase number: this is clarification 3's "own fixed slot after the last phase". Slot 0 is invalid.
4. **Substep.** Substeps count from 0. A stage that is not split uses substep 0, so it draws the same values as the first substep of a split run of the same stage. Phase draws use substep 0. J6's substep driver numbers its substeps 0 to N − 1.
5. **Guarantee.** The layout is one-to-one: two different addresses (or draw indices) always give a different Philox counter or key. For a fixed key, Philox is a bijection, so two draws within the same seed and system can never come from the same output word. Across different keys an equal 64-bit value can occur only by chance (about 1 in 2^64 per pair), as with any good generator.
6. **Streams.** `RandomDraws` reads one address's draws in order and keeps the current block. It is a class, not a struct, so it cannot be copied by accident: a copied struct would hand out the same draws twice. (The analyzers forbid type names ending in "Stream", hence the name.) A stream always starts at draw 0 of its address.
7. **Errors.** A bad turn, phase or substep given to the constructor is a programming mistake and throws the plain `ArgumentOutOfRangeException` (ENG-004 decision 4, internal guards). An address that still holds turn 0 or slot 0 (the default value, or a copy of it made with `with`) throws `InvalidOperationException` when it is used, through one check, `DrawAddress.EnsureValid`. A bad helper argument, such as a bound of 0 or a probability above 1, can come from game data, so it is an impossible input and throws `SimulationDomainException`, which stops the commit.
8. **The ban on `System.Random` is a build error.** Every project runs Microsoft's banned-API analyzer (`Microsoft.CodeAnalysis.BannedApiAnalyzers` 5.6.0) over `BannedSymbols.txt`, which lists `System.Random`, `System.Security.Cryptography.RandomNumberGenerator` and `Guid.NewGuid`. Any use is error RS0030 and fails the build on every CI runner. This was checked by adding `new System.Random(1)` to the library and, separately, to the test project: both builds failed with RS0030. A test keeps the list and its wiring in place.

### Published test vectors

- **Philox4x64-10 itself:** the three vectors in Random123's `tests/kat_vectors` (all zeros, all ones, digits of pi). The code matches all three.
- **Keyed draws:** twelve (address, draw index, value) vectors in `tests/Atlasiv.Core.Tests/Randomness/DrawVectorTests.cs`. For example, seed 0, turn 1, phase 1, substep 0, system 0, entity 0, draw 0 gives `0x8b42ce270989c8fd`. They were computed by a separate Python implementation of Philox and this layout, itself checked against the Random123 vectors first. The C# code matches all twelve.

### Measured cost (Release, x64 development machine, BenchmarkDotNet short run)

| Operation | Mean |
| --- | --- |
| One Philox4x64-10 block (four draws) | 9 ns |
| One draw by address, no block reuse | 11 ns |
| One draw from a stream | 4 ns |
| New stream for an entity plus one draw | 17 ns (104 bytes allocated) |

### Alternatives considered

- **A SplitMix64-style mixer.** Rejected. The key fields total over 300 bits, so a 64-bit mixer would first have to hash them into 64 bits. Two different addresses could then collide (by the birthday bound, likely somewhere among about 2^32 addresses, which a long campaign can reach), and the combined hash would be our own construction, with no outside test vectors. Philox takes the whole address without hashing, and has published vectors from its authors.
- **Philox4x32-10.** Rejected: its 128-bit counter and 64-bit key cannot hold the address without hashing, for the same reason.
- **Threefry or AES-based counter generators.** Not needed: Threefry is slower in plain code, and AES-based generators need hardware instructions whose availability differs between machines.
- **A stateful generator per system (the alternative named in ADR-006).** Rejected by ADR-006: one more draw for one entity would shift every later draw.
- **A 64-bit system ID.** Not needed for Pass 1: systems are few and compiled in. The high 32 bits of key word 1 stay reserved, so Pass 2 can widen it without moving any other field, and existing draws do not change while those bits stay zero.
- **Turn end as slot N + 1 (one after the turn's last phase).** Rejected: the number would depend on the step's phase count. A single constant is simpler and can never meet a phase number.
- **A struct stream.** Rejected for now (decision 6). It would save the 104-byte allocation per stream; revisit if Pass 4 benchmarks show the allocation matters.
- **A source-scan test instead of the analyzer.** Rejected: the analyzer works on resolved symbols, so it also catches `using static`, aliases and fully qualified names, and it fails the build itself rather than a later test.

### Consequences

- Any change to the generator or the layout changes every draw, and so every replay hash. The vectors above make such a change visible; it must be a deliberate, reviewed decision.
- Pass 2 replaces the placeholder entity IDs with permanent ones, keeping counter word 1 as the entity's 64-bit ID.
- J6's scheduler builds one `DrawAddress` per (system, entity, phase or turn end, substep) and passes the substep index from its substep driver.

Related: ADR-006, ADR-012, ENG-003, ENG-004, ENG-009.

---

## ENG-009: Draw helpers

- **Status:** Proposed (Pass 1, J2, 25 Nov 2026).
- **Context:** Spec section 9 asks for helpers on the generator: uniform integers, fixed-point uniform values, Bernoulli and Poisson draws and stochastic rounding. Section 10 and ADR-012 add the time-to-event draw. The spec does not say which algorithms to use.

### Decisions

Each helper is a method on `RandomDraws` and uses a stated number of draws, so a stream's later draws are predictable from what it was asked for.

1. **Uniform integer** (`NextBelow`, `NextInRange`): Lemire's multiply-and-reject method. Every value is exactly equally likely. It uses one draw, and a further draw only in the rare case (probability below bound ÷ 2^64) that the first would bias the result.
2. **Fixed-point uniform** (`NextUnit`): the top 32 bits of one draw, read as a Q32 value in [0, 1). Each of the 2^32 values is exactly equally likely.
3. **Bernoulli** (`NextBernoulli`): true when `NextUnit` is below the probability, so the chance is exactly the probability, to the 2^-32 grid. A probability outside 0 to 1 is an impossible input (`SimulationDomainException`).
4. **Stochastic rounding** (`RoundStochastic`, `MultiplyStochastic`): one draw passed to J1's hook (`Rounding.Stochastic`, `Rounding.MultiplyStochastic`, ENG-004 decision 5). This completes the hook.
5. **Poisson** (`NextPoisson`): the mean is split into pieces of at most 16. Each piece is drawn by inversion from one 64-bit draw, with the probabilities P(K = k) held at 64 fractional bits, starting from exp(−mean) and stepping by mean ÷ k. A sum of independent Poisson counts is Poisson with the summed mean, so the split does not change the distribution. exp(−mean) comes from a new internal function, `FixedMath.ExpNegativeQ64`, which reuses J1's exp series and returns 64 fractional bits, within 2^-62 (4 units of 2^-64; at most 2 observed), so tiny means keep their precision: a Q32 result would round exp(−2^-32) to within one unit of 1 and badly distort the rarest events. A mean of 0 uses no draw.
6. **Time to event** (`NextUnitExponential`, `NextTimeToEventSeconds`): a unit exponential is −ln U, with U = (top 32 bits + 1) ÷ 2^32 in (0, 1], using J1's `FixedMath.Log`. The waiting time for a constant per-year rate is that value × 31,556,952 ÷ rate, rounded half to even to whole seconds. A rate of 0 returns "never" (null) but still uses its draw, so later draws stay where they were. For a rate that varies, the unit exponential is the integrated hazard the event waits for (compare it with `IntegratedHazard`).

### Accuracy and limits

- Uniform integers, `NextUnit`, Bernoulli and stochastic rounding are exact.
- Poisson: probabilities carry an error of a few units of 2^-64. When P(K = k) underflows to 0 at the tail, the remaining tail (below about 2^-58) is given to that k.
- Time to event: U takes 2^32 values, so the largest unit exponential is about 22.18, and the cut-off tail has probability 2^-32. The log adds at most 1 unit of 2^-32.
- **Cost grows with the Poisson mean, with no cap:** one draw and a short loop per 16 of mean (about 140 ns for mean 2.5). A mean in the thousands is fine; a mean in the millions would take milliseconds, and the largest Q32 mean (about 2.1 billion) about 130 million draws in one call. No cap is set, because the spec gives none and an error would be a new rule. If a later system needs huge means, it should add a constant-time method (for example Hörmann's PTRS) with its own benchmark and vectors.

### Published test vectors

`ExpNegativeQ64Tests` lists twelve exp(−mean) vectors, computed in Python with 90-digit decimals, and checks 2,000 random inputs from 0 to 48 against a 160-bit reference, all within the stated bound (hard rule 4). `RandomDrawsTests.A_fixed_sequence_of_helper_draws_matches_the_published_values` runs a fixed sequence of every helper on one stream and checks each result. The expected values were computed independently in Python with exact fractions and 80-digit decimals. The C# code matches all of them exactly. Statistical tests (fixed seeds, bounds of about five standard deviations) check fairness, means, variances, a chi-squared fit for Poisson, and that time-to-event draws agree with J1's hazard primitive.

### Measured cost (Release, x64 development machine, BenchmarkDotNet short run)

| Operation | Mean |
| --- | --- |
| Bernoulli | 5 ns |
| Poisson, mean 2.5 | 142 ns |
| Time to event | 162 ns |

### Alternatives considered

- **Modulo for uniform integers.** Rejected: biased. With a bound of 3 × 2^62, half the results would fall in the lowest third (a test checks this).
- **Knuth's multiply-uniforms method for Poisson.** Rejected: it uses one draw per event counted, not one per 16 of mean, and needs the same exp(−mean).
- **Normal or PTRS approximations for large means.** Not needed yet, and they need more transcendental functions with their own vectors. Deferred until a system needs large means.
- **Piece size for Poisson.** 16 keeps exp(−16) (about 2^-23) at about 39 significant bits inside the 64-bit probabilities. A smaller piece uses more draws; a much larger one loses precision. This is an engineering setting, not a game value.
- **Returning a sentinel such as `long.MaxValue` for a zero rate.** Rejected: "never" is not a time, and a sentinel could be scheduled by mistake.

### Consequences

- The event queue (J5) and scheduler (J6) can schedule rare events from `NextTimeToEventSeconds`, resampling only when the rate changes (ADR-012).
- Any change to a helper's algorithm or draw count changes replay hashes; the vectors make that visible.

Related: ADR-006, ADR-012, ENG-004, ENG-005, ENG-006, ENG-008.

---

## ENG-010: Clock, calendar and boundary calculator

- **Status:** Proposed (Pass 1, J3, 25 Nov 2026).
- **Context:** Spec sections 2 and 3 (Confirmed) fix the clock (signed 64-bit seconds from 1 January 10,000 BCE 00:00, proleptic Gregorian), astronomical years inside and BCE/CE with no year zero on display, the Gregorian leap rule over the whole timeline, the eight steps and their phase rules, half-open turns and phases (clarification 2), and rate time as elapsed seconds ÷ 31,556,952. Issue #7 cites "ADR-039 §2" for the half-open rule; that was the repository's former record of the owner's 24 Nov rulings, now superseded by clarification 2 (ENG-001). The spec does not say how to compute dates, how to name or store the types, or how to format dates for display.

### Decisions

1. **Types** (namespace `Atlasiv.Core.Time`):
   - `Instant`: one clock reading, a signed 64-bit count of seconds from the epoch. Converts to and from `CalendarDate` plus time of day.
   - `CalendarDate`: astronomical year (64-bit), month and day. Impossible days (29 February 1900, 31 April) are refused.
   - `DisplayYear` and `CalendarEra`: the BCE/CE year players read. 1 BCE = year 0, 10,000 BCE = year −9,999.
   - `Calendar`: the leap rule, month lengths, and the internal day-counting core.
   - `TurnStep` and `StepTable`: the eight steps, their lengths in months and their phase counts.
   - `TurnDates`: the boundary calculator. Given a step and a valid turn start, it gives the turn's end, each phase as an interval, and the phase that contains any instant.
   - `TimeInterval`: a half-open [start, end) stretch of time, with its exact seconds and its rate time.
   - `RateYear.FromSeconds`: rate time as a Q32 number of rate years.
2. **Day counting:** Howard Hinnant's `days_from_civil` and `civil_from_days` algorithms ("chrono-Compatible Low-Level Date Algorithms"). They count whole 400-year cycles of 146,097 days, use integers only, take constant time, and are exact for every year that fits in 64 bits. The work is done in 128-bit integers, so only the final 64-bit result can overflow. A date too far out for the clock raises `SimulationOverflowException`, naming the operation and the exact value (ENG-004 decision 4); it never wraps. The helpers take and return `CalendarDate`, so they can never produce an impossible day. Instants before the epoch (negative clock readings) convert correctly too. Nothing in Pass 1 needs them, but forbidding them would be a new rule.
3. **Turn starts:** every turn starts at 00:00. Steps of a year or longer start on any 1 January, the 6-month step on 1 January or 1 July, and the 1-month step on the 1st of any month. A 50-year turn may start in any year, not only in multiples of 50. Section 3 says only "every turn starts on 1 January", and section 4 says a slower switch "can wait up to one year". That would be false if long steps had to align to multiples of their length. **This is an inference from the spec, not a stated rule, so it is flagged for the owner's yes or no.**
4. **Phase dates:** for every step except 1 month, phase *k* starts (*k* − 1) × (months per turn ÷ phase count) months after the turn start: 30, 24, 12, 12, 6, 3 and 2 months. On the 50-year step this alternates 1 January and 1 July, as the spec requires. On the 1-month step, phases start on days 1, 7, 13, 19 and 25, and phase 5 runs to the 1st of the next month (4 to 7 days). Asking for the 1-month step's phase length in months is an error, not 0, so no caller can divide by it by mistake.
5. **Only the step and start are stored** in `TurnDates`. The end and the phase dates are computed when asked, in a few integer operations. Nothing is cached or allocated, so the turn's data cannot disagree with its start.
6. **Default values are real values.** A struct's default cannot be prevented in C#, so the types store month, day and year number counting from 0. The default `CalendarDate` is 1 January 1 BCE, the default `DisplayYear` is 1 BCE, the default `Instant` is the epoch, the default `TimeInterval` is empty at the epoch, and the default `TurnDates` is the first 50-year turn at the epoch. None of them is an impossible "day 0" (compare ENG-008's `DrawAddress.EnsureValid`, which needed a runtime check instead).
7. **Name `TurnStep`, not `Step`:** the build's analyzers refuse a type named `Step`, which is a reserved word in Visual Basic (rule CA1716). The glossary term is still "step". Steps run from slowest (`FiftyYears` = 0) to fastest (`OneMonth` = 7), so "one step faster" is the next value up (for J4).
8. **Rate time:** elapsed seconds ÷ 31,556,952, rounded half to even to the Q32 grid (2^-32 of a rate year, about 0.007 seconds). Seconds add up exactly however a period is split into turns; each conversion to rate years rounds once, by at most half a grid step. J1's hazard primitive (`IntegratedHazard`) already works from exact seconds, so split-independence holds there exactly. 365 days gives 0.99934 rate years (raw 4,292,115,685) and 366 days gives 1.00207 (raw 4,303,874,906), checked against exact fractions computed separately in Python. A negative elapsed time is an impossible input (`SimulationDomainException`), as in J1's hazard primitive.
9. **Errors:** an impossible date, time of day, phase number or turn start given to a constructor is a programming mistake, so it throws `ArgumentOutOfRangeException` (ENG-004 decision 4). A clock reading, day count or year that leaves the 64-bit range throws `SimulationOverflowException`, which stops the commit.
10. **Display text** (display only, never used in computation): dates read "1 January 10,000 BCE" and instants "20 July 1969 CE, 20:17:40". Year numbers of five or more digits group thousands with commas ("10,000 BCE") and shorter ones do not ("1066 CE"), following the design documents' own style. All text uses the invariant culture.

### Tests

- Leap years at 1600, 1700, 1900 and 2000, plus BCE years (1 BCE and 401 BCE are leap; 101 BCE and 10,000 BCE are not). Every 400-year cycle has 146,097 days.
- Epoch vectors: 1 January 1 CE is 315,569,520,000 s, exactly 10,000 rate years (25 whole Gregorian cycles). 1 January 2000 CE is 378,651,801,600 s.
- The day-counting core matches .NET's `DateTime`, an independent proleptic Gregorian reference, on every day of six multi-year spans and on 2,000 random dates from 1 CE to 9999 CE. Tests may use `DateTime`; the simulation never does, because it cannot represent BCE years.
- Hand-written phase dates for every step, across month ends, year ends, leap and common Februaries, and 1 BCE to 1 CE.
- FsCheck properties, 2,000 cases each:
  - over about ±2.7 billion years: instant to date and time and back; date to instant and back; date to BCE/CE year and back; consecutive day counts are consecutive dates;
  - for turns of every step, half on the real timeline (10,001 BCE to 3000 CE) and half up to about ±2 billion years: every second of a turn lies in exactly one phase; phases tile their turn with no gaps or overlaps.
- The 64-bit extremes: every 64-bit day count has a date and converts back, and years beyond the clock's range raise `SimulationOverflowException`.

### Alternatives considered

- **`System.DateTime` or `DateOnly`.** Rejected: they cover only 1 CE to 9999 CE, and the timeline starts in 10,000 BCE.
- **Loops over years and months.** Rejected: the time grows with the distance from the epoch. Hinnant's algorithms take constant time and are widely checked.
- **A 32-bit year.** Rejected: the 64-bit clock reaches about ±292 billion years, which does not fit in 32 bits. A 64-bit year keeps every clock reading convertible to a date.
- **Storing phase start dates in an array per turn.** Rejected: it allocates, and could go stale. Computing them takes a few integer operations.
- **Suppressing CA1716 to keep the name `Step`.** Rejected: renaming is cheaper than an exception to the build rules.
- **Turn starts aligned to multiples of the step (for example 50-year turns only in years divisible by 50).** Rejected: not in the spec, and it contradicts section 4's "wait up to one year".

### Not built here

- **Seasons in the header** (spec section 2: meteorological, by the hemisphere of the selected location, display only). This needs a selected location and a header, which arrive with the client. It never touches computation.

### Consequences

- `TurnDates` describes a turn's dates only; it does not carry the turn's number. Turn numbers (from 1 at the epoch, never reset, spec clarification 16) belong to whoever advances turns: J4's step controller or J6's scheduler.
- J4 (step controller) decides switches with `TurnDates.IsValidStart` and `TurnStep` order. J5 (event queue) orders events by `Instant`. J6 (scheduler) runs phases from `TurnDates.Phase` and passes `TimeInterval.Seconds` to the hazard and rate primitives.
- No values are left open: J3 adds no `// CALIBRATE` placeholders.

Related: ADR-004, ENG-001, ENG-004, ENG-006, ENG-008.

---

## ENG-011: Step controller

- **Status:** Proposed (Pass 1, J4, 25 Nov 2026). Decision 5 is an owner ruling given during J4 (25 Nov 2026).
- **Context:** Spec section 4 (Confirmed) and clarifications 4 to 7, 10 and 16 fix the rules: the leading edge is read through an interface; the candidate is the step closest on a log scale to half the strategic time; switches need 10 consecutive turns (faster) or 20 (slower), at least 20 turns apart, one step at a time; faster switches align at once and slower ones wait for 1 January or 1 July; an era-transition event with a preview is issued when a switch is decided, and the switch is then committed. Issue #8 cites "ADR-039 §3, §5, §6, §7 and §11"; that was the repository's former record of the owner's 24 Nov rulings, now superseded by clarifications 4 to 7, 10 and 16 (ENG-001). The spec does not say when the leading edge is read, how the count behaves while a switch waits, or which turn the 20-turn gap is measured to.

### Decisions

1. **Types** (namespace `Atlasiv.Core.Time`):
   - `CandidateStep.For(strategicSeconds)`: the candidate step for a strategic time in whole seconds.
   - `StepTable.NominalSeconds(step)`: a step's nominal length in whole seconds.
   - `ILeadingEdge`: one method, the leading edge's strategic time for a reference undertaking as of a turn boundary. Tests supply a scripted one.
   - `ReferenceUndertaking`: the undertaking the leading edge is timed on (a distance in metres), with an uncalibrated placeholder.
   - `StepController`: owns the turn number and the current turn's dates. `AdvanceTurn()`, called once the current turn's turn-end stages have run, reads the leading edge, decides any switch and starts the next turn. It is not named `EndTurn`, because the glossary's turn end is the running of the 10 turn-end stages, which the scheduler does. The constructor takes the reference undertaking explicitly, so no caller can use the placeholder without naming it.
   - `EraTransition`: the era-transition event and its preview.
2. **Nominal lengths:** a step's months × 2,629,746 s, one twelfth of the 31,556,952-second rate year, which divides exactly (50 years = 1,577,847,600 s; 1 month = 2,629,746 s). These are only for comparing steps. Real turns still follow calendar dates.
3. **Candidate comparison:** with strategic time S, target T = S / 2 is closer to the slower of two neighbours when T² ≥ L_slower × L_faster. Both sides are multiplied by 4, so S² is compared with 4 × L_slower × L_faster in 128-bit integers. There is no logarithm and no rounding of S / 2. Scanning slowest first, the first neighbour pair whose midpoint S reaches picks the slower step; if none does, the candidate is 1 month. Ties go to the slower step (≥), and targets beyond the table clamp to 50 years or 1 month. A negative strategic time is an impossible input (`SimulationDomainException`); zero is allowed and gives 1 month.
4. **Exact ties cannot occur with whole seconds.** No two neighbours' nominal lengths multiply to a perfect square: their month counts multiply to 144,000, 28,800, 7,200, 1,440, 288, 72 and 6. The tie rule is still in the code. It is tested through an internal entry point that takes S² directly, and a separate test checks that the last whole second on each side straddles the tie strictly.
5. **The 20-turn gap is measured to the announcement (owner ruling, 25 Nov 2026).** A switch may be decided at a turn boundary only if the turn about to start is at least 20 turns after the turn the current step took effect (turn 1 at the epoch). A slower switch then waits for its alignment, so the real gap between two switches taking effect is 20 to 25 turns. A count that completes earlier fires at the first boundary the gap allows, if it still holds there.
6. **When the leading edge is read:** once at the epoch to pick turn 1's step, then once at every turn boundary, at the instant the next turn starts. Each reading is one turn's observation for the count. It is not read while an announced switch waits for alignment, because the switch is committed and the count restarts when the new step takes effect. Counting during the wait would give the same decisions: no switch can be decided in the 20 turns after one takes effect, and 20 observations of the new step are enough for both thresholds, so the result never depends on what happened before.
7. **The count** is a direction (faster, slower or neither) and a number of consecutive turns. It resets to zero when the candidate equals the current step, restarts at 1 when the direction changes, and is cleared when a switch is announced. A switch is always one step in the count's direction, whatever the candidate.
8. **Alignment:** the effective boundary is found by walking the old step's turns forward until a boundary is a valid start for the new step (`TurnDates.IsValidStart`). A faster step accepts every boundary of the next slower step, so a faster switch never waits. A slower switch waits at most 5 turns (1 month to 6 months, from an August boundary to January). Turn numbers keep counting through the wait and across the switch.
9. **The era-transition event** holds the old step, the boundary where it was announced, the effective turn number and the first new turn's dates. The new step, effective date, phase count and phase dates are read from those dates, so they cannot disagree. `AdvanceTurn()` returns it at the boundary where the switch is decided. `PendingSwitch` holds it until it takes effect. Pass 1 has no event queue yet, so J6 puts it on J5's queue as an informational event.
10. **Placeholders:** `ReferenceUndertaking.PlaceholderDistanceMetres = 0` and `ReferenceUndertaking.Uncalibrated`, both marked `// CALIBRATE`. The spec leaves the undertaking and its distance to the data phase (section 14, decision 2). 0 means "not set", so a real transport system given the placeholder would produce an obviously wrong answer. The controller passes the undertaking to the leading edge unchanged; the scripted leading edge in Pass 1 ignores it.
11. **J3 fix:** `TurnDates` now has its own `ToString()` ("OneMonth turn from 1 September 9999 BCE"). The record's generated version printed every property, including `Next` (another turn), so it recursed until the stack overflowed. Any failing assertion that printed a turn or an era-transition event crashed the whole test run instead of reporting the failure. A regression test covers it.

### Tests

- Candidate: each step's nominal length; a strategic time of exactly two turns of each step picks that step; for each neighbour pair, the last whole second that picks the faster step and the next one that picks the slower (boundary values and exact ties computed separately in Python: isqrt(4 × L_slower × L_faster) and 4 × L_slower × L_faster); exact ties go to the slower step; clamping at both ends; a negative strategic time is an error.
- Controller, through a scripted leading edge: turn 1 on the candidate at the epoch; readings at each boundary for the given undertaking; a faster switch after 10 turns but not 9, including one on a 1 July boundary that takes effect at once, and a slower one after 20 but not 19; a 1-month to 6-month switch announced on 1 September and effective on 1 January, with the hand-written preview and the four waiting turns; 6 months to 1 year waiting one turn; a committed switch going ahead after the candidate drifts back; the gap counted from the effective turn (the next switch is turn 45, not 41); resets on an equal candidate (both directions) and on a change of direction; direction-only counting with a switch of one step; a count completed inside the gap that fires at turn 21, and one that no longer holds and fires later; all seven switches from 50 years down to 1 month, and all seven back up after a collapse, one step every 20 turns, with turn numbers never reset.
- Mutation check: deliberately breaking the direction reset, the gap, the slower threshold or the one-step rule each makes tests fail. Removing the reset on an equal candidate changes nothing, because the direction change already resets the count.

### Alternatives considered

- **Logarithms or a log table.** Rejected: clarification 7 asks for exact integer comparison, and hard rule 4 forbids library math.
- **Calendar-exact turn lengths for the comparison.** Rejected: they vary with leap days and month lengths, so the candidate would depend on the date. Clarification 7 says nominal lengths.
- **Gap measured to the effective turn.** Offered to the owner, who chose the announcement (decision 5).
- **Counting the exact candidate instead of direction.** Rejected by clarification 5.
- **An immutable state value with a pure `Next` function instead of a class.** Not needed yet. J8 decides how the controller's state (turn number, current turn, last switch turn, count and pending switch) is serialized and restored.

### Consequences

- J6's scheduler calls `AdvanceTurn()` once per turn, after the turn-end stages, and queues any returned `EraTransition` as an informational event.
- J8 must serialize and restore the controller's state for replay. J9's 1,000-turn scenario drives it with a scripted leading edge.
- Placeholders left: the reference undertaking and its distance (decision 10).

Related: ENG-001, ENG-004, ENG-010.
