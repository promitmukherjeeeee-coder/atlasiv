# ATLASIV: instructions for Claude Code

ATLASIV is a single-player grand-strategy simulation of real Earth from 10,000 BCE until the first faster-than-light test. The owner, Promit, is not a programmer: he reads your summaries and test results, not the code. Write every summary for him in plain language.

## Sources of truth

- `docs/01` to `docs/07` are exports of the master design documents. They are read-only here. If code and docs disagree, the docs win. Report the disagreement; never change a doc to match the code.
- `docs/04_pass1_spec.md` is the confirmed spec for the current pass. `docs/06_computational_doctrine.md` is binding for all engineering.
- `docs/PASS1_PLAN.md` is the job list. Work one job at a time, in order.
- `registry/` holds the draft machine-readable registries (interface, mechanics, ownership, dependencies). Do not edit them in Pass 1.

## Current pass: Pass 1, Time and Resolution

In scope: clock and calendar, steps and phases, step transitions, the event queue, resolution order, standing-order and Advance Until hooks, fixed-point numerics, counter-based random streams, adaptive-integration hooks, the input log, the replay hash, tests and benchmarks.

Out of scope: the world state store and identifiers (Pass 2), ledgers and solvers (Pass 3), any real game system, and anything in Godot. Pass 1 code is a plain .NET library with no Godot dependency.

## Hard rules

1. Determinism. The same seed, rules and inputs give bit-identical state on every machine. Parallelism, logging and UI never change results.
2. No `float` or `double` in authoritative state or its update rules. Use 64-bit integers and Q32.32 fixed point, with `Int128` intermediates and checked overflow. Overflow is an error, never a silent wrap.
3. No `System.Random`. All randomness comes from the counter-based generator keyed by (seed, turn, phase, system, entity, draw index).
4. No library math functions (`Math.Exp`, `Math.Log` and so on) in authoritative code. Use the integer or table-based versions with test vectors.
5. Iterate entities in ascending stable-ID order. Never depend on dictionary, hash-set or thread order.
6. Dates display as BCE and CE, with no year zero. Internal years are astronomical (1 BCE = 0).
7. No modding, plugin or extension API. Only the source code is published.
8. Never invent a value the spec leaves open (tolerances, thresholds, reference undertaking). Use a named placeholder constant, mark it `// CALIBRATE`, and list it in your summary.
9. Simulate the maximum meaningful complexity with the minimum necessary computation: run systems only when due, and never add per-second ticking.

## Working method for each job

1. Create a branch named `pass1/jN-short-name`.
2. Read the spec sections listed for the job in `docs/PASS1_PLAN.md`.
3. Write the tests first, then the code. Keep each change small.
4. Run `dotnet build` and `dotnet test` locally until they are green.
5. Commit, push and open a pull request. Wait for CI on all runners.
6. Add any engineering decision you made (with alternatives and reason) to `docs/decisions-log.md`.
7. Finish with a summary for the owner: what was built, what the tests prove, the CI result, decisions taken, placeholders left, and what the next job is. Do not merge; the owner merges.

If something in the spec is ambiguous or contradictory, stop and ask the owner rather than guessing.

## Layout

```
src/Atlasiv.Core/          simulation library (no Godot)
tests/Atlasiv.Core.Tests/  xUnit and FsCheck tests
bench/Atlasiv.Benchmarks/  BenchmarkDotNet
docs/                      design docs (read-only), plan, decisions log, reports
registry/                  draft registries (not used until Pass 2)
```
