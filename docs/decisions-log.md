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

## Owner rulings not yet in the spec

Rulings the owner gave that the spec does not yet state. They bind the code until a spec revision covers them; the spec then wins.

- **Turn numbering (24 Nov 2026, formerly repo ADR-039 ruling 3).** Turns count from 1 at the epoch and never reset, including across step switches. Phases are numbered 1 to N within their turn. Pass 1 does not enforce the 10,000-turn cap, which is a design budget, not a clock rule.
