# Computational Architecture and Mathematical Doctrine

Nov 24, 2026 · @Promit

The binding technical doctrine for Atlasiv: every mathematical formulation, architecture choice, implementation, test and optimisation follows it. It integrates the owner's final directive (24 Nov 2026) with a critical evaluation, and it stays subordinate to the confirmed gameplay specifications (Final Master Vision, Mechanics, Deep Dives, locked interface), which it never alters.

**Governing principle:** simulate everything that matters, compute only what is necessary, and represent complexity through mathematics rather than brute force.

## 1. Role and objective

- Claude acts as principal computational architect, applied mathematician, numerical simulation engineer, systems architect, algorithm designer and performance engineer.
- The goal is a compact, coherent mathematical universe from which immense civilisational complexity emerges, running on modern consumer hardware.
- Optimisation is never permission to reduce scope, remove mechanics, compromise meaningful fidelity or replace emergence with predetermined outcomes. Reducing confirmed scope needs explicit owner approval.

## 2. Philosophy

Computational parsimony, mathematical elegance, emergent complexity and simulation fidelity. Mathematical sophistication must serve fidelity, performance, explainability and maintainability, and the simplest mathematically sufficient representation wins.

1. Maximum meaningful complexity with minimum necessary computation.
2. Emergent behaviour, not predetermined outcomes.
3. Mathematical relationships, not arbitrary modifiers, wherever appropriate.
4. Unified state, not duplicated subsystem states.
5. Analytical solutions, not unnecessary iteration.
6. Sparse, not indiscriminately dense, computation.
7. Incremental updates, not repeated global recalculation.
8. Multiscale simulation, not uniform resolution.
9. Conservation and reconciliation, not disconnected accounting.
10. Adaptive precision, not indiscriminate detail.
11. Bounded approximation, not uncontrolled simplification.
12. Reproducible stochasticity, not uncontrolled randomness.
13. Explainable causality, not opaque outcomes.
14. Empirical verification, not assumed correctness.
15. Measured performance, not speculative optimisation.

## 3. Order of engineering priorities

1. Correct mathematical formulation.
2. Appropriate abstraction and aggregation.
3. Algorithmic complexity reduction.
4. Authoritative data architecture.
5. Dependency architecture.
6. Computational scheduling.
7. Memory efficiency and locality.
8. Parallel execution.
9. Vectorisation and hardware acceleration.
10. Low-level optimisation.
11. Additional hardware.

Inefficient mathematics is never compensated by hardware, servers, needless parallelism or premature low-level tuning.

## 4. Unified mathematical representation

Atlasiv is one coupled, multiscale dynamical system with one authoritative world state. All domains (population, economy, institutions, government, military, diplomacy, knowledge, technology, culture, religion, environment, infrastructure) interact only through authoritative shared variables and declared dependencies. It must never become a set of loosely connected minigames. Discrete-event simulation, dynamical systems, network science, graph theory, computational economics, stock-flow-consistent accounting, operations research, numerical analysis, demography, stochastic processes, agent-based modelling, control theory, statistical mechanics, spatial mathematics, constrained optimisation, information theory and scientific computing are tools inside that one representation, never separate engines.

## 5. Unified specification template

No substantial mechanic is implemented before its specification exists. The doctrine's 26 formulation fields are merged with the Mechanics template into one template:

1. Mechanic ID, name, confirmed requirement and source references.
2. Purpose and real-world process represented.
3. Dependency layer and authoritative owner.
4. State variables with types, units and dimensions.
5. Inputs and outputs.
6. Governing equations, labelled as established relationship, calibrated empirical model, gameplay abstraction or unvalidated hypothesis.
7. Assumptions, initial conditions and boundary conditions.
8. Conservation requirements, constraints and feasibility conditions.
9. Spatial and temporal resolution, and resolution frequency.
10. Dependencies and feedback connections.
11. Stochastic components.
12. Candidate formulations and algorithms, compared explicitly, with the choice justified.
13. Time and space complexity, and expected workload.
14. Numerical stability, precision class and approximation error with its invalidity conditions.
15. Actor interaction and authority, information model, AI decision model.
16. Calibration and historical validation requirements.
17. Edge cases, pathological conditions and failure modes.
18. Verification strategy (invariants, convergence, regression) and benchmarking strategy.
19. Interface locations (WS- workspace IDs, dossier and investigation types).
20. How the mechanic meets the governing principle.
21. Status: confirmed, proposed, calibration required, implementation verified.

Equations are never invented silently during implementation. Every accepted formulation enters the Mathematical Model and Algorithm Registry (section 12).

## 6. Core techniques

- **Analytical time advancement.** Advance over the elapsed interval with closed forms wherever their assumptions hold (for example P(t+Δt) = P(t)·exp(rΔt) under constant growth), for growth, depreciation, deterioration, retention, competence, regeneration, interest, recovery, diffusion and accumulated probability. Use piecewise solutions, integrated rates, adaptive methods or event-driven transitions when conditions change, and never outside their assumptions.
- **Multiscale time.** One canonical clock, independent of displayed turn length. Each phenomenon uses its own scale: analytical advancement, scheduled updates, discrete events, adaptive and multirate steps, subcycling only where required, at defined synchronisation boundaries. A five-year turn never runs five years of daily steps. Equivalent elapsed time under different legal subdivisions must give consistent results, and unavoidable differences are quantified and documented.
- **Multiscale space.** Cells, settlements, districts, provinces, economic regions, watersheds, networks, political territories, cultural regions, continents and the globe. Resolution follows the phenomenon's mathematical needs, never the camera. Observing a region never changes its rules, and coarse calculation never erases strategically important local dependencies.
- **Sparse networks.** Adjacency lists, compressed sparse rows, indexed and hierarchical graphs, spatial neighbourhoods, active-edge evaluation and demand-driven discovery. 20,000 provinces with about 8 relevant edges each is about 160,000 edges, not 400 million pairs. Long-distance interactions stay discoverable through routing, markets and search. No quadratic or worse cost on frequent large-scale paths without explicit justification.
- **Single source of truth.** Every persistent variable has one owner. State classes: authoritative, derived, cached, historical, presentation, forecast, counterfactual and actor-specific knowledge. Stable IDs, controlled transactions, versioned schemas. The UI never mutates authoritative state.
- **Incremental computation.** Dependency graphs, dirty flags, version counters, incremental aggregation, invalidation and cached derived values. A destroyed bridge invalidates routes, logistics capacity and supply allocation, while unrelated systems keep their normal schedule. Incremental results are verified against full recalculation in automated tests.
- **Coupled systems.** Strongly connected components get bounded solvers with damping, tolerances, residual checks, iteration caps and explicit non-convergence handling, for example x(n+1) = (1−α)x(n) + αF(x(n)) where appropriate. No uncontrolled recursion, no forced equilibrium where disequilibrium is historically meaningful, and no artificial delays used only to dodge coupling.
- **Stock-flow consistency.** Stock(t+Δt) = Stock(t) + integrated inflows − integrated outflows, with explicit transformations, losses, regeneration, production and consumption. Stocks are distinct from flows, capacity from output, ownership from control, nominal from real. Double-entry where appropriate, with invariants checked at transaction and resolution boundaries.
- **Cohorts.** p(t+Δt) = T(Δt)p(t) + m, with sparse, evolving cohorts, controlled splitting, merging and compression, preserved joint distributions where mechanics need them, and quantified aggregation error. Individuals only where identity matters. The same applies to firms, households, soldiers, researchers and institutional subunits.
- **Adaptive resolution.** Effort follows rate of change, sensitivity, uncertainty, instability, cascade potential, significance and required accuracy, with measurable error bounds. The player's civilisation never gets different rules, and distant civilisations are never artificially simplistic.
- **Stochastic processes.** Hazards, not repeated probability checks: probability within Δt = 1 − exp(−λΔt), and integrated hazards when λ varies. Competing and correlated risks where required. Separated, seeded, reproducible streams. Narrative never invents authoritative state changes.
- **Bounded actors.** Hierarchical planning, utility and constrained optimisation, candidate generation and feasibility filtering, bounded rationality, actor-specific information, mandates, horizons, incremental replanning and reusable evaluations. Replan only when relevant conditions change. Generative AI is never required for decisions or resolution, and may only enrich presentation.
- **Precomputation and caching.** Terrain adjacency, distances, watershed topology, technology prerequisites, capability requirements, stable routes, regional aggregates, market-access structures, historical indexes, coefficients and reusable transition matrices, with dependency-aware invalidation, explicit ownership, validity conditions and memory budgets. Never cache what costs more to invalidate than to recompute.
- **Numerics.** Precision per variable, with absolute and relative tolerances, stability conditions, solver convergence, and overflow and underflow handling. Invalid states (non-finite values, impossible inventories, negative populations) are prevented, never hidden by clipping. Every approximation documents why, its assumptions, expected error, invalidity conditions, performance benefit, causal impact and detection method. Precision is not fidelity.
- **Data-oriented design.** Structure of arrays, contiguous buffers, compact IDs, typed and sparse structures, pools, hot and cold separation, batched updates and SIMD-friendly layouts on hot paths. Object-oriented code where it aids clarity without costing measured performance.
- **Parallelism.** Dependency-aware dynamic task scheduling, never domains pinned to cores. Deterministic reductions and controlled commit boundaries. GPU compute only where measured, otherwise the GPU serves rendering.
- **Determinism.** Reproducible from equal state, decisions, inputs and seeds, with an explicit execution contract covering arithmetic, parallel order, worker count, platform, compiler and solver behaviour. Bitwise determinism where required, documented tolerances elsewhere.
- **Budgets and scalability.** Per-subsystem budgets for CPU, memory, bandwidth, allocations, entity and relationship counts, solver iterations, queue sizes, history growth, serialisation and rendering. No unbounded growth. When a budget is exceeded: find the bottleneck, its cause, alternatives, benchmark them, verify equivalence or quantify the difference, implement the best, document trade-offs. Never silently discard significant information.
- **Engine separation.** Simulation core, UI, rendering, persistence, networking, narrative, analysis and platform integration are separate. The core runs complete campaigns without rendering a frame. Ordinary single-player uses the player's CPU, with no recurring cloud, external API or AI inference.
- **Presentation efficiency.** Keep the visual direction and locked four-level interface. GPU rendering, atlases, procedural composition, spatial indexing, semantic zoom, level of detail, culling, instancing, cached layers, incremental updates, on-demand loading. Every workspace, dossier and investigation is a view of authoritative state. The UI stays responsive during resolution, and transfer between core and presentation is minimal.
- **History and counterfactuals.** Checkpoints, compressed deltas, event sourcing where useful, stable IDs, causal provenance, indexes, deterministic replay, versioned models and archival storage, never full snapshots every phase. Recorded causes are distinguished from correlation. Counterfactuals run on isolated states, never mutate the campaign, and can run asynchronously.

## 7. Domain starting points

Candidate families, not final equations. Each domain compares alternatives in its specification.

| Domain | Candidate mathematics |
| --- | --- |
| Population and society | Cohort transition matrices, joint distributions, constrained labour allocation, migration choice models |
| Production | Sparse input-output matrices, production functions, capacity utilisation, inventories, stock-flow accounting |
| Trade and transport | Capacitated network flows, routing, market access, transport costs, constrained allocation |
| Commodity markets | Supply-demand, bounded price discovery, inventory adjustment, substitution, disequilibrium |
| Banking and finance | Double entry, stock-flow-consistent models, balance sheets, credit constraints, sparse exposure networks |
| Monetary systems | Circulation, monetary accounting, exchange-rate formation, inflation dynamics |
| Industrial grades | Typed compatibility, production dependencies, quality constraints, substitution, material conservation |
| Energy | Network balance, generation constraints, storage, transmission capacity, constrained dispatch |
| Hydrology | Directed watershed graphs, water balance, flow accumulation, storage |
| Agriculture | Yield-response and limiting-factor functions over soil, climate, water, labour, inputs |
| Ecology | Stock-flow, nonlinear regeneration, carrying capacity, spatial diffusion |
| Disease | Compartmental models, demographic risk, network transmission, health-system capacity |
| Research | Sparse prerequisite graphs, competence functions, discovery hazards, diffusion, adoption, mastery |
| Institutions | State transitions, organisational competence, resource constraints, governance relations, utility |
| Culture and religion | Network diffusion, demographic transitions, identity persistence, institutional influence |
| Government | Multidimensional state, institutional constraints, actor utility, legitimacy dynamics |
| Diplomacy | Sparse relationship graphs, strategic evaluation, obligations, negotiation, private information |
| Military logistics | Capacitated, time-dependent network flows, stockpiles, transport constraints |
| Warfare | Aggregate operational combat, readiness, logistics, terrain, composition, command, bounded stochastics |
| History | Event graphs, provenance, state transitions, causal dependencies, replay |
| Forecasts and counterfactuals | Bounded scenario simulation, sensitivity, uncertainty propagation, isolated states |
| Scoring | Transparent, normalised, multidimensional indicators from authoritative state |

## 8. Performance objectives

Preliminary engineering objectives, not guarantees, revised only through documented benchmarks and architectural review, and never a reason to remove gameplay silently.

**Reference hardware (mainstream tier):** modern 6 to 8-core desktop CPU, 16 GB RAM, integrated or dedicated GPU, SSD. Entry-level and high-performance tiers get their own targets after the first benchmarks.

| Measure | Design target | Ceiling |
| --- | --- | --- |
| Ordinary UI response | under 100 ms |  |
| Standard dossier opening | under 500 ms |  |
| Map rendering | about 60 FPS, representative conditions |  |
| Ordinary action phase | about 0.2 s (confirmed) | 3 s |
| Ordinary late-game turn (5 phases plus turn-end tick) | about 2 s (confirmed) |  |
| Turn-end tick | about 1 s (confirmed) |  |
| Complex late-game phase or turn | about 10 s worst case (confirmed) | 10 s |
| Exceptional crisis phase |  | 30 s |
| Process memory | below about 2 GB where feasible |  |
| Long-run memory growth | bounded |  |
| Exact accounting conservation error | zero | zero |
| Deterministic replay | mandatory under the execution contract |  |

Targets are what the architecture is designed for. Ceilings are the worst acceptable measured values. Advanced analytical investigations run asynchronously where needed. Median, high-percentile and worst-case times are tracked.

## 9. Verification

Unit, integration, property-based, conservation, dimensional-consistency, deterministic-replay, numerical-convergence, time-step-consistency, aggregation-consistency, incremental-versus-full, parallel-consistency, persistence, long-duration, performance-regression and pathological-state tests. Optimised algorithms are compared with small reference implementations. Stress cases include simultaneous wars, financial contagion, infrastructure collapse, institutional destruction, mass migration, shortages, ecological disasters, technological disruption, fragmentation and interacting crises. Headless campaigns hunt runaway feedback, instability, unbounded memory, implausible emergence and late-game slowdown. Correctness, plausibility, balance and efficiency are separate validation dimensions.

## 10. Calibration

Models are calibrated against historical, scientific and economic evidence across periods, geographies, political systems, economies and institutions, never one civilisation's trajectory, and never by hardcoding outcomes. Every parameter records its definition, source, uncertainty range and class: historically supported, scientifically established, estimated, gameplay balancing or fictional technology. Sensitivity analysis finds dominant parameters, and identifiability, robustness and uncertainty propagation are checked where appropriate.

## 11. AI-assisted development discipline

Never build the whole game in one pass, never treat the mother documents as an instruction to generate a codebase, and never build subsystems that bypass the unified architecture. For each mechanic:

1. Identify confirmed requirements.
2. Inspect existing architecture.
3. Identify data ownership.
4. Formulate the mathematics.
5. Identify dependencies.
6. Compare candidate algorithms.
7. Document complexity.
8. Set numerical and conservation requirements.
9. Implement the smallest coherent production-quality module.
10. Add automated tests.
11. Benchmark representative workloads.
12. Verify integration.
13. Document results and limitations.
14. Update the engineering specifications.

No frameworks, dependencies, databases or services without justification. No duplicated systems, no silent spec changes, no placeholders or mocks presented as finished, no needless abstraction, no premature optimisation.

## 12. Engineering documents

Subordinate to the gameplay specifications, with traceability from mechanic to model to module to test to benchmark.

| ID | Document | Where it lives and when |
| --- | --- | --- |
| A | Computational Architecture and Mathematical Doctrine | This tab, now |
| B | Authoritative World State and Data Schema | Produced by Pass 2, extended every pass |
| C | Mathematical Model and Algorithm Registry | Seeded by the Pass 1 spec, one entry per accepted formulation, version-controlled in the repository |
| D | Dependency and Computational Scheduling Architecture | Pass 1 (clock and scheduling), Pass 2b and Pass 3 (dependencies and solvers), Pass 8 (full matrix) |
| E | Verification, Benchmarking and Performance Protocol | Starts with Pass 1 tests and benchmark harness, consolidated at Pass 9 and every gate |
| F | Architectural Decision Registry | Its own tab, now, seeded with the decisions already taken |

**G. Interface Architecture Registry (authoritative, machine-readable).** Defines every interface element and ID (convention in Final Master Vision 9.11), navigation rules, linked mechanics, actions, access conditions, cross-links and validation tests. It references, never defines, data ownership (document B). The ID skeleton enters the repository with Pass 2; it is populated in Pass 10, and every UI element must resolve against it.

## 13. Development sequence mapped to passes

| Doctrine stage | Roadmap pass | Note |
| --- | --- | --- |
| 1 Requirements consolidation | Done | Vision, Mechanics, Deep Dives, Q&A, amendments, interface chart |
| 2 Dependency layer pass | Pass 2b (all mechanics: owners, inputs, outputs, loops, components), refined at Pass 8 | Pulled forward from a one-page draft to a full declaration-level graph |
| 3 Mathematical formulation | The spec at the start of every pass | Each system is formulated before its code |
| 4 Authoritative data architecture | Pass 2 |  |
| 5 Scheduling architecture | Pass 1, solvers in Pass 3 |  |
| 6 Synthetic scalability benchmarks | Gate 1 (new requirement) | Early and late-game synthetic worlds before production domain code |
| 7 Headless kernel | Passes 1 to 3 |  |
| 8 Coupled-system implementation | Passes 4 to 7 |  |
| 9 Calibration and stress testing | Inside every pass, consolidated in Pass 9 and Gate 3 |  |
| 10 Presentation integration | Prototype P2 and Pass 10 | Interface and art prototyping run in parallel but never dictate the mathematics |
| 11 Continuous profiling | Every pass |  |

## 14. Acceptance criteria and prohibitions

**A mechanic is complete only with:** conformance to confirmed requirements, correct ownership, explicit formulation, appropriate complexity, numerical stability, defined precision and tolerances, conservation, correct dependencies, appropriate temporal and spatial resolution, reproducible stochastics, explainable outcomes, automated verification, measured performance, calibration where applicable, maintainability, extensibility and documentation. Exceptions are documented and submitted for approval.

**Prohibited:** simulating billions of individuals; unjustified dense all-to-all evaluation; whole-world recalculation after local change; conflicting authoritative states; uncontrolled recursion; per-turn multipliers across variable turn lengths; unbounded solvers; unexplained clipping; history generated outside the simulation; language models for actor decisions; unjustified indefinite full snapshots; heavy simulation on the UI thread; workspaces as independent simulations; causality traded for convenience; unmeasured performance claims; brute force in place of mathematics; complexity without purpose; scope reduction without approval.

## 15. Evaluation of the confirmed scope

**Already supporting this architecture:** single integer clock and event queue; per-year rates with exact elapsed time; phases with fast and slow systems; closed-form slow systems and adaptive substeps; seeded counter-based random streams; one owner per variable; conservation ledgers and invariants; precision classes; adaptive resolution by significance; sparse typed relationship web; events plus snapshots plus replay; cohort keys with age vectors and a group target; aggregated households, firms and researchers with tiered characters (K39); research evaluated over active programmes only.

**Needing further mathematical definition:** every equation (no mechanic has formulas yet); market clearing; migration choice; legitimacy aggregation; precedent weight in the Governance Ledger; cultural pull; discovery hazard; combat process; grid dispatch; climate reduced-order model; FTL pressure curve; scoring normalisation.

**Likely computational bottlenecks:** market clearing per node and good; transport routing under congestion; grid dispatch in late eras; AI planning for thousands of actors; migration destination choice; disease contact across networks; history and provenance storage growth; counterfactual state copies.

**Analytical opportunities:** cohort ageing and survival as precomputed propagators per legal phase length; exponential decay of depreciation, competence, memory and trust; logistic regeneration of ecological stocks; compound interest; integrated hazards for discovery, disasters, succession and unrest; seasonal profiles integrated in closed form.

**Sparse opportunities:** province adjacency, trade, transport, migration corridors, watersheds, grids, finance exposures, diplomacy, institution networks, character relations, diffusion channels, technology prerequisites, recipe graph.

**Incremental opportunities:** routes after infrastructure change; market access; regional aggregates; legitimacy by group; precedent weights; institution eligibility for subtrees; scoring indicators.

**Aggregation opportunities:** quiet regions on integrated rates; ordinary firms, households, researchers and soldiers as cohorts; departments and programmes as records; minor events summarised.

**Combinatorial explosion risks:**

| Risk | Naive size | Mitigation |
| --- | --- | --- |
| Actor knowledge states (each entity's view of others and of geography) | entities × provinces × categories, tens of millions | Store knowledge as sparse deltas from truth with observation date and decay, materialise on demand |
| Cohort Cartesian product | full product of province, culture, religion, class, age, occupation | Primary key plus age vector plus conditional distributions, about 12 groups per province |
| Market pairs | nodes × goods × nodes | Node-local clearing plus sparse inter-node arbitrage edges |
| Character relations | notables squared | Typed sparse links, aggregate links for non-notables |
| Diplomacy pairs | states squared | Contact-gated sparse graph |
| Institution × technology eligibility | institutions × techs | Evaluate active programmes and frontier techs only (K39) |
| Governance Ledger growth | entries × 10,000 turns | Precedent weights as running aggregates, raw entries archived |
| Event history | every change | Checkpoints plus input-log replay, archived summaries |
| Counterfactual copies | full world per scenario | Copy-on-write state over a checkpoint |

**Coupled systems (strongly connected components to solve explicitly):** prices, production, wages, employment and energy demand; food, health, labour and stability; credit, investment and output; legitimacy, unrest and policy; migration and housing; climate, yields and emissions.

**Ownership requirements:** labour by military, agriculture, industry and research reconciles against one population ledger; research commitments against funding, personnel, equipment and compute ledgers; military personnel against demography; equipment against inventories.

**Numerical and conservation requirements:** integer people, money and inventories; scaled fixed-point rates; zero conservation error; bounded solvers with logged non-convergence.

**Required benchmark worlds:** the eight confirmed scenarios (prehistoric, ancient, medieval, industrialising, modern, late-game, global war, cascade crisis) plus maximum intended complexity, sized from the confirmed scale: about 15,000 provinces (envelope 20,000), about 12 cohort groups each, thousands of political organisations and notable characters.

**Architectural risks:** fixed-point performance and range in continuous science models; AI planning cost; incremental invalidation bugs that leave stale state; history growth over 10,000 turns; Godot to core data transfer for large map layers; calibration effort for hundreds of parameters.

## 16. Critical evaluation and refinements to the directive

The directive is sound. These refinements strengthen it without reducing scope:

1. **Integers make parallel determinism cheap.** Integer and fixed-point addition is associative, so parallel sums give identical results in any order, while floating-point sums do not. Keeping authoritative state in integers gives deterministic parallelism without ordered reductions. This is the strongest reason to keep the Pass 1 rule for authoritative state.
2. **Precision per quantity, not one fixed-point format.** A single 32.32 format overflows above about 2.1 billion, which world population, GDP and energy totals exceed. Each quantity declares its unit and scale in the data schema: 64-bit integer base units for counts, money and mass, scaled fixed point for rates and probabilities, 128-bit intermediates for products.
3. **Floating point where it belongs.** Presentation, forecasts, analysis and labelled approximations may use floating point freely. Authoritative continuous models (climate, hydrology) may use it only under a verified deterministic profile (basic IEEE operations only, no platform library transcendentals, fixed operation order, no fused operations), approved through the decision registry after benchmarks. This reconciles the directive's "no universal representation" with confirmed bitwise replay.
4. **Cache propagators per legal time step.** There are only eight turn steps and a small set of phase lengths, so transition matrices and decay factors such as exp(AΔt) can be precomputed once per legal Δt and regime, turning analytical advancement into lookups.
5. **Sample time-to-event instead of checking each phase.** For rare hazards, draw the next event time once when the hazard changes and schedule it in the event queue. Resample only when the hazard is invalidated. Work falls from entities × phases to events plus hazard changes.
6. **Warm-start solvers.** Markets, flows and dispatch change slowly between phases, so iterative solvers start from the previous solution and converge in a few iterations.
7. **Error-bounded cohort merging.** Merge cohorts only when their transition rates differ by less than a tolerance (lumpability), so aggregation error is bounded and measurable.
8. **Equal-area global grid for environmental cells.** A latitude-longitude grid distorts area towards the poles. A discrete global grid of near-equal cells (icosahedral hexagons are one candidate) makes diffusion, climate and hydrology uniform. To be decided in Pass 2.
9. **Dimension-typed quantities.** Unit-typed value wrappers in C# (persons, kilograms, years, currency) turn dimensional mistakes into compile errors at near-zero runtime cost. To be decided in Pass 2.
10. **One template, not two.** The doctrine's 26 fields and the Mechanics template are merged (section 5).
11. **Knowledge as deltas from truth.** Actor knowledge is the largest combinatorial risk, and sparse deltas with decay keep it bounded (section 15).

## 17. Conflicts resolved

Decisions under the owner's delegated authority, recorded in the Architectural Decision Registry:

| Topic | Directive | Resolution | Reason |
| --- | --- | --- | --- |
| Core language | Evaluate Rust as preferred candidate | Keep C# (ADR-001) | Owner already chose C#. One language for core and Godot client avoids a foreign-function boundary on the hot data path, and AI-assisted development is simpler in one language. Rust's advantages (WebAssembly, no garbage collector) matter little for a desktop target, and allocation-free data-oriented C# controls the collector. Reopen only if browser delivery becomes a requirement. |
| Browser, WebAssembly, browser workers | Supported targets | Native desktop (Windows, macOS, Linux) plus headless command line, no browser target (ADR-003) | Godot 4 desktop is confirmed. Browser phrasing is read in its platform-neutral meaning: no heavy work on the UI thread, background workers for simulation, a memory target for the process. To verify at setup: whether the chosen Godot 4 release can export C# projects to the web at all. |
| Numeric representation | No universal mandate, floating point where appropriate | Integers and scaled fixed point for authoritative state, floating point elsewhere and under a verified profile (ADR-004, ADR-005) | Bitwise replay is a confirmed exit test, and refinement 1 above |
| Performance numbers | Phase under 3 s, complex under 10 s, exceptional under 30 s | Confirmed tighter targets remain design targets, directive values become ceilings (section 8) | Both are preliminary, and the confirmed ones were set earlier |
| Development order | Dependency pass, formulation and benchmarks before the kernel | Pass 1 kernel proceeds now; full declaration-level dependency pass at Pass 2b; synthetic benchmarks added to Gate 1; each domain formulated before its code | The clock, random streams and hashing do not depend on any mechanic's equations, so nothing is coded ahead of its formulation |
| Templates | 26-field formulation list | Merged with the Mechanics template (section 5) | One template avoids divergence |

## 18. Knowledge and historiography: axioms, data model and mathematics (added 24 Nov 2026)

Scope: Final Master Vision 9.18, Mechanics section Q (K40 to K62, X16 to X27, A14, A15, U11), ADR-019 to ADR-034. Nothing here is implemented or tested. No parameter values are set.

### 18.1 Formal axioms

- **AX-1 Universal Emergent Causality.** For every authoritative quantity q, every change Δq in a phase is the sum of contributions from declared causal inputs registered in document D. A contribution with no declared input is a defect. No rule may take an era label, a technology name or a title as its only input. One declared exogenous input is allowed (owner decision 24 Nov 2026): the FTL horizon value (X28), registered in document D as an exogenous constant or schedule that feeds only valuation terms (AI and institutional utility, sponsor-share value, the FTL contribution score) and never a state change directly.
- **AX-2 Knowledge–Capability Separation.** For actor a and capability c, the states access(a,c), understood(a,c) and capability(a,c,region,institution) are separate variables. A change in one changes another only through a declared transition (absorption K48, development K50). Access never implies understanding; understanding never implies capability.
- **AX-3 Truth isolation.** No actor-facing computation (research, synthesis, AI decisions, publication) reads objective state except through an observation channel that writes an actor-knowledge record (ADR-015). Testable by instrumenting reads.
- **AX-4 Non-duplication of evidence.** The effective evidence of a set of sources equals the evidence of their distinct origins after collapsing copies, translations, derivatives and citation cycles (K54).
- **AX-5 Non-duplication of benefit.** Each underlying improvement has exactly one authorised application path with a declared owner, scope, conditions and duration (K61).
- **AX-6 Temporal integrity.** An artifact with publication time t contains no claim whose earliest accessible evidence time is after t. Editions are immutable once published.
- **AX-7 Presentation neutrality.** Rendering, reader, export and access-mode code cannot write authoritative state; per-phase state hashes are identical with rendering on, off or replaced.
- **AX-8 Truth–Evidence–Belief–Consequence Separation.** For information item i and actor or group g, truth(i), evidence(i), strength(i), credibility(i,g), belief(i,g), acceptance\_institution(i,k), acceptance\_public(i,g) and utility(i,g,issue) are separate variables; none is computed from truth(i) except evidence generation at observation. Consequences produced while belief(i,g) was held are not undone when belief changes (K65).

### 18.2 Conceptual data model (inputs to Pass 2, not a schema)

| Entity or relation | Key contents | Owner | Cardinality concern |
| --- | --- | --- | --- |
| Objective event | Time, place, participants, effects, cause references | Event store (X10) | Grows with events; compaction rules needed |
| Knowledge artifact | Permanent ID, class, authorship, creation time, provenance, components | Knowledge system (K40) | Significant works individual, ordinary output aggregated |
| Claim | Typed proposition, epistemic category, evidence links | Knowledge system (K41) | Only where meaningful |
| Content version and edition | Content hash, edition date, language, revision of | Knowledge system (K42) | Deduplicated by hash |
| Copy | Carrier, location, condition, custodian | Resource accounting (K42) | Rivalrous; aggregated by holding where possible |
| Rights and access relation | Holder, scope, granularity, conditions, duration | Knowledge system with legal inputs (K43) | Sparse; scope-based for open publication |
| Actor understanding | Actor, discipline or domain, level, last refreshed | Knowledge state (A8 deltas) | Sparse, by discipline not by artifact |
| Competence stock | Institution, discipline, level, personnel links | Institutions (K26, K48) | One per institution and discipline |
| Capability state | Actor, capability, region, institution, industry, state, dependency status | Production and industry (K50) | Sparse; only where not default |
| Historical memory | Group, claim, belief strength, source mix | Culture and politics (X24) | Bounded by salient claims |
| Publication record | Artifact, edition, renderer ID and version, ruleset version | Presentation metadata (X25) | Outside hashed state |

### 18.3 Mathematics requirements matrix

| Need | Mechanics | Candidate approach | Determinism note | Decide in |
| --- | --- | --- | --- | --- |
| Absorption and decay | K48, K49 | First-order relaxation toward accessible frontier, exact per legal Δt | Fixed-point propagator cache (ADR-011) | Pass 7a |
| Capability transitions and regression | K50, K51 | Hazards with dependency-gated rates; time-to-event sampling | Counter-based RNG (ADR-006, ADR-012) | Pass 7a |
| Effective independent evidence | K53, K54 | Count of distinct origins on the provenance graph after cycle collapse | Integer graph algorithm, stable ordering | Pass 7a |
| Synthesis quality | K53, K57 | Bounded saturating function of coverage, independence, reliability, competence | Fixed point; no unbounded sums | Pass 7a |
| Open-publication reach | K46, K47 | Eligibility filter times uptake hazard, evaluated lazily on query | Query results cached; invalidated by events | Pass 7a |
| Consensus and acceptance | K56 | Bounded belief updates per community; separate public and political quantities | Integer or fixed-point weights | Pass 7a or 8 |
| Historical memory | X24 | Per-group belief over claims with decay and transmission | Same as consensus | Pass 8 |
| Prestige and diffusion | K58 | Saturating prestige with obsolescence decay | Fixed point | Pass 7a |
| Feedback stability | Section Q loops | Saturation and damping on every reinforcing loop; check with sensitivity analysis | Verified in tests 76 and 80 | Pass 2b and 7a |

### 18.4 Technology and era mechanics review

The existing catalog was checked for rules that would break AX-1 or AX-2. Found and fixed: K1 (Boolean unlock wording) and X5 (scripted FTL incentives). To be checked when each spec is written, because their current one-line entries do not say how benefits apply: any K-series entry that grants a bonus on discovery, any era-gated change in the E and N series, and the Internet and media entries. Each must route through K50 capability and K61 benefit rules.

### 18.5 Additions for AX-8 (24 Nov 2026)

- **Data model.** Information item properties (K63): evidence links and strength on the claim; credibility and belief as sparse per-actor or per-group records only where they differ from a group default; institutional and public acceptance owned by institutions and public opinion (S20). Leverage holdings (D22): holder, target, item, issue, expected-consequence estimate, expiry; sparse and event-invalidated.
- **Mathematics (proposed).** Credibility as a bounded update from perceived evidence, source reputation, visible independent corroboration (AX-4) and prior belief, with incentive-weighted bias; candidate forms include bounded Bayesian-style updating in fixed point. Leverage value as the target's estimated loss from disclosure times the holder's estimate of disclosure credibility, recomputed on relevant events; candidate solution via the existing decision evaluators, no new solver. Anticipation (D23) limited to bounded-depth estimates of others' beliefs to keep cost finite. Decide in Pass 7a, with D-series details in the pass that specifies diplomacy.

## 19. Knowledge and historiography: scaling, benchmarks and acceptance tests (added 24 Nov 2026)

### 19.1 Scaling analysis

The risk is combinatorial: actors × artifacts × access relations × disciplines over up to 10,000 turns. The design keeps every store proportional to something smaller:

- **Access** scales with publication scopes and explicit grants, not actors × artifacts (open content is one scope record).
- **Understanding** scales with actors × disciplines touched, not artifacts.
- **Capabilities** are stored only where they differ from the default for that actor or region.
- **Content** is deduplicated by hash; copies of ordinary works are aggregated counts per holding.
- **Provenance graphs** are sparse; independence is computed on demand for a synthesis, then cached until an input changes.
- **History** stores objective events once; accounts store references and claims, not copies of events; rendered prose is a cache outside the hashed state.
- **Unknown until measured:** growth of the event store over a full campaign, claim counts in the digital era, and synthesis frequency. These are Gate 1 questions.

### 19.2 Benchmark plan by knowledge environment

Synthetic worlds for Gate 1 and the Pass 7a gates, each measured for phase time, memory, save size and replay hashes against the ADR-009 targets and ceilings:

- **Oral and pre-literate:** many groups, no written artifacts, tacit knowledge only.
- **Manuscript era:** few copies, slow transmission, monastic and court collections, frequent loss.
- **Print era:** many editions and copies, growing institutions, translation networks.
- **Industrial science:** journals, review, replication, rapid capability transitions.
- **Digital open network:** very large open publication volume, global scopes, censorship zones.
- **Collapse and recovery:** mass loss of infrastructure and personnel, regression and restoration.
- **Long campaign:** a full-length run to measure history and artifact storage growth (tests 83 to 85).

### 19.3 Acceptance tests KH-001 to KH-100

Numbered as in the amendment, section 58. None has been run. Methods: deterministic tests, property-based tests, synthetic benchmark worlds, long-campaign stress tests, adversarial information-access tests, research-model validation, sensitivity analysis and human evaluation of publication quality.

| Range | Group | Tests (short form) | Main mechanics | Verified in |
| --- | --- | --- | --- | --- |
| 001 to 015 | Historical architecture | One event in many histories without duplication; different accounts from legitimate knowledge; consistency with events and beliefs; no hidden-information leakage; debug tag switch keeps perspective; omniscient mode separate; history survives division, merger and transfer; extinct entities keep records; source destruction and rediscovery; no anachronistic knowledge in prehistory; editions keep dates; later discoveries absent from earlier works; revisionism never edits objective history; natural history independent of ownership; significance changes without altering events | X16 to X24, A14 | Pass 2, 8, 10 |
| 016 to 033 | Knowledge architecture | One artifact shared without duplication; custody vs content; ownership vs access; access vs acquisition; acquisition vs comprehension; comprehension vs absorption; absorption vs capability; capability vs deployment; copying keeps the original holder's access; unique physical transfer moves custody; acquisition through existing systems; translation not discovery; derivatives not evidence; circular citation gives no credibility; repeated acquisition gives no unlimited bonus; different collections give different research ability; succession keeps knowledge; losing a copy keeps assimilated knowledge | K40 to K45, K52, K54 | Pass 2, 7a |
| 034 to 043 | Transmission and absorption | Open publication reachable by connected actors; no bilateral treaty needed; network restrictions limit access; same access, different competence; same acquisition, different absorption; translation and education matter; absorption needs time, competence, resources; tacit transfer through people; competence decays after losses; digital publication scales without actor × artifact records | K46 to K49, A15 | Pass 7a |
| 044 to 052 | Capability and deployment | Same literature, different capability; understanding without industry; demonstration vs deployment; imported machines do not confer manufacturing; regression after dependency loss; codified knowledge survives; preserved knowledge eases but does not restore; deployment consumes capital, labour, energy, materials; regional differences survive | K50, K51 | Pass 5, 7a |
| 053 to 071 | Research, synthesis, validation | Independent sources enable comparison; incomplete evidence gives uncertainty; missing prerequisites block research; synthesis creates an artifact with provenance; synthesis becomes a source; competing Grand Works; later works supersede; scholarly authority vs truth; no hidden information; wrong theories distinguishable; review not proof; replication vs acceptance; consensus from evidence; consensus vs political acceptance; benefits through authorised paths; geology creates no deposits; constitutional scholarship guarantees no stability; medical knowledge creates no medicines; volume does not decide legacy | K53 to K62 | Pass 7a, 8 |
| 072 to 080 | Universal emergence | Same knowledge, different outcomes; no Internet bonus; no era-forced industrialisation; effects through declared dependencies; stable feedback loops; no duplicated benefits; Causal Inspector names contributors; classifications describe, not prescribe; counterfactual changes change outcomes | AX-1, AX-5, K61 | Pass 2b onward |
| 081 to 100 | Computation and presentation | Deterministic replay; rendering leaves hashes unchanged; history, artifact, access graph and synthesis scalability; replaceable renderers; export correctness; source traceability; ruleset and build provenance on every work (see ADR-032); play continues without language models; biased accounts distinguishable from hallucination; long volumes coherent; beliefs affect play only through authorised mechanics; editions preserved after invalidation; save migration; phase performance within benchmarks; no interface mutation of state; no debug knowledge transfer; meaningful player interaction with acquisition, research, synthesis, absorption and deployment | AX-3, AX-7, X25 to X27, A14, A15, U11 | Gate 1, Pass 7a, 10, Gate 4 |

**KH-101 to KH-110 (added for AX-8).** 101: a false but credible claim produces real political, diplomatic or market consequences. 102: an accurate but non-credible or inaccessible claim produces little effect. 103: the same item has different utility for different targets and issues. 104: credibility differs from correctness in both directions. 105: repetition and derivative copies do not raise credibility as independent corroboration would. 106: counter-evidence, verification, pre-emptive disclosure and counter-leverage reduce or flip leverage. 107: correction does not reverse earlier consequences and itself produces new ones. 108: AI and institutional decisions use beliefs and expectations of others' behaviour, never objective truth (adversarial access test). 109: historical works keep occurrence, contemporary belief, later scholarly conclusion and consequences distinct. 110: objective truth is unchanged by belief, narrative, consensus or interpretation. None has been run.

**KH-111 to KH-112 (added for X28).** 111: the FTL horizon value changes only valuation terms (AI and institutional utility, sponsor-share value, FTL contribution score) and never research progress, capability or project stages directly; removing it changes decisions, not physics. 112: every actor, player and AI, sees the same declared value at the same time, and no other exogenous term exists in document D. None has been run.

## 20. Integration record for the knowledge and historiography amendment (24 Nov 2026)

### 20.1 Traceability

| Amendment area | Vision | Mechanics | Deep Dives | Doctrine | ADR | Roadmap |
| --- | --- | --- | --- | --- | --- | --- |
| Axioms (emergence, separation) | Foundational axioms, 4.4 | K1, X5 revised | 12.9 | 18.1 | 019, 020 | Pass 2b |
| Knowledge artifacts, rights, lifecycle | 9.18 | K40 to K44 | 12.5, 12.6 | 18.2 | 022, 023 | Pass 2, 7a |
| Transmission, open networks, absorption | 9.18 | K45 to K49 | 12.8 | 18.3 | 025 | Pass 7a |
| Capability, regression | 9.18 | K50, K51 | 12.9, 12.10 | 18.3 | 024 | Pass 5, 7a |
| Institutions, synthesis, review, Grand Works | 9.18 | K52 to K62 | 12.5, 12.7 | 18.3 | 026, 027 | Pass 7a |
| Historiography | 9.18 | X16 to X24 | 12.1 to 12.4 | 18.2 | 021, 028 | Pass 8 |
| Rendering, reader, export, access modes | 9.18, 9.11 | X25 to X27, A14, U11 | — | 18.1 AX-7 | 029 to 031 | Pass 10 |
| Scaling | 9.18 | A15 | — | 19.1, 19.2 | 033 | Gate 1 |
| Traceability and benefits | Foundational axioms | K61 | — | 18.1 AX-5 | 034 | Pass 2b |

### 20.2 Change log

24 Nov 2026: Vision foundational axioms, 4.4 FTL wording, new 9.18 (old 9.18 now 9.19), DS-07, two showcase views; Mechanics section Q (38 new IDs), K1 and X5 revised; Deep Dives section 12; Doctrine sections 18 to 20; ADR-019 to ADR-034; Roadmap knowledge and historiography paragraph. Later the same day: Truth–Evidence–Belief–Consequence Separation axiom in Vision, Living Historiography four-way distinction, K63 to K65, D22, D23, X16 revised, AX-8, 18.5, KH-101 to KH-110, ADR-035.

### 20.3 Consistency audit and conflicts

- **FTL incentives.** Previously confirmed as "scripted incentives"; then made purely emergent under AX-1; finally resolved by the owner on 24 Nov 2026 as emergent pressure plus one hard-coded FTL horizon value (X28, ADR-036) standing for benefits outside the simulation. Vision 4.4, pillar 1 and X5 updated.
- **Modding vs extensions.** The amendment's extension, open-source plugin and modded-history requirements conflicted with "No modding support whatsoever." Resolved by the owner on 24 Nov 2026: no mods, plugins or extension API; only source code is published; all requested capabilities are built in-house (ADR-032).
- **Date labels.** Resolved 24 Nov 2026: BCE and CE in every tab and in the game.
- **Existing systems extended, not duplicated.** K5, K21, K3, K4, K17, K26, K29, K30, K32, K34, I1, I15, I16, X1, X2, X9, X10, X15, A8 are extended by Q entries; their owners are unchanged.
- **ID collisions.** None. New IDs continue K39, X15, A13 and U10; tests use the KH- prefix; axioms use AX-.

### 20.4 Count reconciliation

New mechanics: K40 to K62 (23), X16 to X27 (12), A14 to A15 (2), U11 (1): 38 entries; then K63 to K65 and D22 to D23 (5) for AX-8; then X28 FTL horizon value (1): 44 in total. Interface: 28 workspaces (unchanged), 10 showcase views (was 8), 7 dossier types (was 6), 6 analysis types, 5 global systems, 6 analytical patterns. ADRs now 38. Acceptance tests added: 112. Axioms: 8 (AX-1 now names its one declared exogenous input).

### 20.5 Status classes

- **Owner-confirmed requirements:** all gameplay requirements in Vision 9.18 and the axioms.
- **Proposed for engineering evaluation:** every formulation in 18.3, storage choices in 18.2, scaling methods, renderer and export formats.
- **Not implemented, not tested:** everything in this amendment.

### 20.6 Unresolved questions and unavailable sources

- Addressed 24 Nov 2026 (draft 0.1.0 files, to be committed in Pass 2): interface\_registry.yaml (the machine-readable Interface Architecture Registry), mechanic\_registry.yaml (all 407 mechanics with owner, layer and declared links), state\_ownership.yaml (28 owner systems, the seed of document B) and system\_dependency\_map.yaml (system-level draft of document D). Plugin and open-source requirements: none, confirmed by the owner. Still open by design: variable-level inputs and outputs, which need each mechanic's 21-field specification (Pass 2b).
- Owner decisions, all resolved 24 Nov 2026: no modding, plugins or extension API (ADR-032); BCE/CE; the FTL horizon value (X28, ADR-036); interface chart redrawn as v3.4 from the registry.
- Engineering questions: event-store compaction over 10,000 turns; claim granularity in the digital era; how often synthesis runs; the offline renderer's quality bar for human evaluation.

### 20.7 Gameplay audit

The risk is a system that is correct and invisible. The player must feel it through decisions: what to acquire and how, whom to hire or poach, which institutions to fund, what to publish or keep secret, when to build the production chain behind a known technology, and how to use history for legitimacy. Delegation follows the Autonomous Institutions model, so the player sets priorities and institutions do the volume. Test KH-100 is the gate; the Pass 7a prototype must show these choices matter before the full system is built.
