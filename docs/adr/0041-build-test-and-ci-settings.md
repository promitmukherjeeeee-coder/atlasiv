# ADR-041: Build, test and CI settings

- **Status:** Proposed (Pass 1, J0). Awaiting owner review.
- **Context:** PASS1_PLAN J0 asks for deterministic build settings (warnings as errors, invariant culture in tests) and CI on Windows, Linux, macOS x64 and macOS ARM. Hard rule 1 in `CLAUDE.md` requires bit-identical results on every machine. Several tooling choices had to be made to get there.

## Decisions

1. **Shared settings in one file.** `Directory.Build.props` sets the target framework, nullable reference types, `Deterministic`, `ContinuousIntegrationBuild` (on CI), `TreatWarningsAsErrors`, and the .NET analyzers at the `latest-recommended` level with code style enforced in the build. A project cannot quietly opt out.
2. **Invariant culture everywhere.** `InvariantGlobalization` is on for every project, not only the tests, so no number, date or text is formatted or parsed by the machine's locale. A test checks that the current culture is invariant.
3. **Exact package versions in one place.** `Directory.Packages.props` (central package management) pins every NuGet package to one exact version, so every machine restores the same packages.
4. **xunit v3 on the Microsoft Testing Platform.** Tests use xunit v3 with FsCheck 3 (`FsCheck.Xunit.v3`). `dotnet test` runs on the Microsoft Testing Platform, set in `global.json`, because the .NET 10 SDK no longer runs xunit v3 through the older VSTest runner.
5. **Two narrow analyzer exceptions, in `.editorconfig`.** Test method names may use underscores (CA1707), because they read as sentences. Benchmark methods may be instance methods (CA1822), because BenchmarkDotNet requires it. Neither exception applies to the simulation library.
6. **CI matrix.** One GitHub Actions workflow (`.github/workflows/ci.yml`) restores, builds in Release and tests on `windows-latest`, `ubuntu-latest`, `macos-15-intel` (macOS x64) and `macos-latest` (macOS ARM64). `fail-fast` is off, so one failing runner does not hide the others. Test results are uploaded from every runner.

## Alternatives considered

- **xunit v2 with VSTest.** Rejected: xunit v2 is in maintenance only, and v3 is the supported line for .NET 10.
- **NUnit or MSTest.** Rejected: the plan names xUnit with FsCheck.
- **Floating package versions (for example `4.*`).** Rejected: two machines could restore different versions.
- **Invariant culture only in tests.** Rejected: the library itself must never depend on locale, and turning it on everywhere costs nothing.
- **`macos-13` for macOS x64.** Rejected: GitHub has retired that image; `macos-15-intel` is the current Intel image.

## Consequences

- Any new warning breaks the build on every runner. Fixing it is part of the change that caused it.
- Package upgrades are deliberate, one-line changes in `Directory.Packages.props`.
- J9 adds the cross-runner hash comparison to this workflow.

## Related

ADR-040 (.NET 10), ADR-004 (integer arithmetic), ADR-006 (random generator).
