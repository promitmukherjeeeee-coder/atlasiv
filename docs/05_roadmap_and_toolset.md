# Roadmap and Toolset v1

Nov 23, 2026 · @Promit

The revised pass order with a prototype gate, and the complete toolset for a non-coder building with AI. Decision authority (from 24 Nov 2026): the owner gives one final set of requirements and confirms every item of the Pass 1 spec. Every other open decision, including the toolset, is delegated to Claude, which decides, records the choice and its reason in these documents, and flags anything irreversible. Project record: seven working tabs (Final Master Vision, Mechanics Brainstorm, Deep Dives, Pass 1 Spec, Roadmap and Toolset, Computational Doctrine, Architectural Decision Registry) plus two archive tabs. The owner's final requirements (the computational directive) were received and integrated on 24 Nov 2026.

## 1. Principles

1. **Specs are tested by code.** Every pass that defines a system ends with a running, tested version of it, however small.
2. **A prototype gate comes early.** Before the remaining specifications, a small headless simulation must prove performance, determinism, plausibility and fun.
3. **Validation runs inside every pass,** not at the end.
4. **Identifiers and provenance are designed in at the start,** because retrofitting event identities and snapshots is expensive.
5. **The simulation is separate from the engine.** A headless core can be tested without opening the game.

6) **Maximum meaningful complexity, minimum necessary computation.** Every system spends detail only where it changes outcomes, and every specification states how. The Computational Doctrine tab is binding for all engineering, and the Architectural Decision Registry records every engineering decision.

Engineering documents A to F (Computational Doctrine section 12) are produced alongside the passes. Reference hardware, performance targets and ceilings are in Computational Doctrine section 8. Platform: native desktop plus a headless command line, no browser target (ADR-003).

## 2. Decision needed: first-release scope (Gate 0)

A full run from 10,000 BCE to FTL is about 6,390 turns and 34,800 phases. At 30 seconds per phase that is roughly 290 hours.

| Option | Scope | Phases | Play time at 30 s per phase |
| --- | --- | --- | --- |
| A | Full run in the first release | 34,800 | about 290 hours |
| B (recommended) | 10,000 BCE to 1500 CE first. Later eras (industrial, modern, FTL) arrive as expansions on the same architecture | about 6,300 (18%) | about 50 hours |
| C | Full run, with start-date bookmarks (for example 3000 BCE, 500 CE, 1500 CE, 1900 CE) | 34,800, entered mid-way | varies |

The architecture, determinism rules and data model are built for the full vision in every option. The option only changes what must be finished and tuned before release. Bookmarks (option C) need a synthesized world state for each start date, which is a large content task.

## 3. Revised pass order

| Pass | Content | Documents | Code | Exit test |
| --- | --- | --- | --- | --- |
| 1 Time and resolution | Clock, phases, event ordering, adaptive integration, determinism, transitions | Spec | Clock, phase scheduler, seeded random streams, replay hash | Identical hashes on two machines |
| 2 World state | Identifiers, state components, spatial structures, relationships, variable ownership, event identities and snapshot format | Spec | State store, 750 zones, snapshot save and load | Save, load and replay give identical hashes |
| 2b Dependency layer pass | Declaration-level dependency graph for every mechanic: authoritative owners, inputs, outputs, feedback loops and strongly connected components (doctrine stage 2) | Dependency graph (engineering document D) | None | Reviewed by you |
| 3 Conservation and dependencies | Ledgers, direct dependencies, strongly connected subsystems, solver contracts | Spec | Ledger checks, dependency graph validator | People, stock and money balance every turn |
| **Gate 1: skeleton** |  |  | Headless skeleton with one dummy system, plus synthetic early-game and late-game benchmark worlds sized to the confirmed scale (doctrine stage 6) | Determinism, conservation and speed tests pass, and benchmark worlds meet the doctrine budgets |
| 4 First coupled simulation | Population, water, food, resources, production, energy, transport and markets together. Prototype subset first: population, food, water, migration, technology diffusion, assimilation | Spec | Prototype P1 (headless, 10,000 BCE to about 3000 BCE) | Gate 2 criteria below |
| Prototype P2 | Playable 30-minute slice on a placeholder map | Minimal UI notes | Simple client in the engine | Fun test below |
| **Gate 2: go or no-go** |  |  |  | Criteria in section 4 |
| 5 Institutions and actors | Political authority, characters, factions, AI decision-making, diplomacy, military planning | Spec | Characters, factions, basic AI | AI entities survive, expand and act plausibly over 2,000 simulated years |
| 6 Historical provenance | Event dossiers, causal links, historical snapshots, replay | Spec | Event log, causal tracing, replay tool | Any outcome can be traced to its causes |
| 7a Technology, culture, religion | Invention, diffusion, adoption, culture pull, religion | Spec | Systems and tests | Calibration cases pass |
| 7b Economy and finance | Markets, trade, money, banking, global finance | Spec | Systems and tests | Conservation and price-convergence tests |
| 7c Intelligence and warfare | Espionage, command, supply, combat, occupation | Spec | Systems and tests | Battle and supply scenarios |
| 7d Environment and industry | Climate, hydrology, ecology, energy grid, advanced industry | Spec | Systems and tests | Convergence tests |
| 7e Endgame | FTL pressure curve, FTL project, scoring | Spec | Systems and tests | Full-run scoring test |
| 8 Full interaction matrix | Direction, coupling strength, update frequency, conservation requirements for every pair | Matrix | Matrix-driven dependency check | Code and matrix agree |
| 9 Validation and prioritization | Calibrated cases, convergence tests, benchmarks, risk list for feedback loops | Test plan | Full test suite in automated runs | All suites green |
| **Gate 3: pre-1500 slice** |  |  | A complete 10,000 BCE to 1500 CE headless run | Meets performance budget and calibration |
| 10 Interface integration | Map every subsystem to the four-level interface architecture, keeping the visual direction | Mapping | Real UI built on the engine | Playable alpha |
| **Gate 4: playable alpha** |  |  | Playable pre-1500 game | Playtest results |
| Later | Industrial and modern eras, FTL era content, multiplayer (deferred) |  |  |  |

**Knowledge and historiography amendment in the pass order (24 Nov 2026).** No new pass. The work lands in existing passes:

- **Pass 1 (Time): no amendment required.** The confirmed clock, event ordering, stable IDs and replay already give what the amendment needs (exact event times, the eight distinct time stamps as clock values, deterministic replay). Artifact identities, access relations and multi-time stamps are Pass 2 entities. Rendering sits outside the hashed state, and access modes are a Pass 2 and Pass 10 policy.
- **Pass 2 (World State):** permanent identities for artifacts, claims, editions and copies; access and rights relations; capability states; ownership per Doctrine 18.2 and state\_ownership.yaml (28 owner systems, ADR-038); the registry files from draft 0.1.0 (interface, mechanic, ownership, dependency; ADR-037) committed, reviewed and placed under schema validation.
- **Pass 2b (Dependencies):** declared causal inputs for every knowledge and history mechanic (AX-1); the feedback loops in Mechanics section Q checked for saturation. Starts from system\_dependency\_map.yaml: every cross-system link gets a timing (same stage, next phase or turn end), and the 17-system coupled core is broken into solvable loops.
- **Gate 1:** the knowledge-environment benchmark worlds (Doctrine 19.2) join the synthetic early and late-game worlds.
- **Pass 7a:** a knowledge prototype built first (artifacts, access, absorption, capability, synthesis on a small world), with its own gate on tests KH-016 to KH-071 and KH-100, before the full system.
- **Pass 8:** historiography (X16 to X24).
- **Pass 10:** deterministic offline renderer, historical reader and intellectual library, export and access modes (X25 to X27, A14, U11).

Content production (technologies, cultures, religions, recipes, names, art) runs in parallel from Gate 2 onward.

## 4. Gate 2 criteria (go or no-go)

1. **Performance.** The prototype turn-end tick at 750 zones takes 100 ms or less, which extrapolates to about 2 seconds at 15,000 provinces. Phases stay within budget.
2. **Determinism.** Replay hashes match across two machines.
3. **Conservation.** People, food and money balance every turn.
4. **Plausibility.** Across many seeds, farming and settlement emerge in the cradle regions within plausible windows and in plausible order, with natural variance. Real population estimates (such as HYDE) are the benchmark.
5. **Fun.** Five outside testers each play 30 minutes, and at least three want to continue without being asked.
6. **AI baseline.** Non-player peoples survive, grow and settle without cheats.

If Gate 2 fails, adjust the design or scope and run the gate again. Two failures means a scope reduction.

## 5. Toolset overview

### 5.1 Core tools

| Role | Tool | Why |
| --- | --- | --- |
| AI builder | Claude Code (desktop app or VS Code extension), using the strongest model in your model picker | Writes, runs and tests code in your repository |
| Design and specification | Claude chat and Docs (this document) | Specs, reviews, critiques |
| Knowledge-work tasks | Claude Cowork, Claude in Excel | Document upkeep, data cleaning, balancing tables |
| Editor | VS Code | A window onto the repository, with Git built in |
| Engine (client) | Godot 4 with C# | Free, light, text-based scenes that AI edits well. Unity is the alternative if you later hire a Unity developer |
| Simulation core | C# library, headless | One language for core and client. Fixed-point integer math, seeded streams |
| Version control | Git and GitHub | History, branches per pass, issues, automated checks |

### 5.2 Quality and testing

| Need | Tool |
| --- | --- |
| Unit and integration tests | xUnit (C#) |
| Property-based tests | FsCheck |
| Determinism | Golden replay hashes checked on every change |
| Performance | BenchmarkDotNet and profilers (Godot, dotnet-trace) |
| Automatic checks | GitHub Actions run all tests on every change |
| Calibration plots | Python notebooks (Jupyter, matplotlib) reading simulation output |

### 5.3 Data and content

| Need | Tool |
| --- | --- |
| Content tables | JSON, YAML or CSV in the repository, validated by JSON Schema |
| Balancing | Excel or Google Sheets, exported to the tables by script |
| Real-Earth data pipeline | Python (numpy, rasterio and GDAL, geopandas) |
| Candidate datasets (check licenses) | GEBCO and ETOPO elevation, ICE-6G and GLAC-1D ice and sea level, TraCE-21ka paleoclimate, HYDE population and land use, Seshat and D-PLACE for societies and cultures |

### 5.4 Design, art and audio

| Need | Tool |
| --- | --- |
| UI system | Figma (component library for the four-level architecture). Figma Make for clickable flows |
| Final art and UI | Hired illustrators and a UI designer. AI output is for concept only |
| Map rendering | Godot shaders generating the survey-sheet look from simulation data (technical artist) |
| Audio | Godot audio, FMOD if needed, composer under contract |

### 5.5 Release and operations

| Need | Tool |
| --- | --- |
| Playtesting | itch.io or Steam Playtest builds, Discord, screen recordings |
| Telemetry | Opt-in JSON logs from the game |
| Distribution | Steam (Steamworks) |
| Backend (later) | Supabase for accounts, cloud saves and multiplayer, only after the prototype. Steam Cloud covers saves earlier |
| Project tracking | GitHub Projects |

## 

1. Keep the vision, constraints and conventions in the repository (a short project instruction file that summarizes the confirmed decisions).
2. Write the test before the feature. Passing tests are the proof, since you do not read the code.
3. One subsystem per branch, small steps, merge only when all checks are green.
4. Ask Claude Code for plain-language summaries of every change and its test results.
5. Run the determinism hash and conservation checks daily.
6. Have a human engineer review the architecture at each gate. (Deb da and Dipan.)&#32;
7. Keep the Master Vision, Q&A and Mechanics documents as the source of truth, and note every change in them.

## 7. Open decisions

1. First-release scope: option A, B or C in section 2.
2. Engine: decided, Godot 4 with C# (24 Nov 2026).
3. Whether to commission a technical artist early for the map shaders, or prove the style on a placeholder first.
