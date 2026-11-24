# Architectural Decision Records

One file per decision. ADR-001 to ADR-038 were split, wording unchanged, from the Architectural Decision Registry (formerly `docs/07_architectural_decision_registry.md`). Its original preamble and group notes are kept below.

## Original preamble

Nov 24, 2026 · @Promit

Engineering document F: every significant engineering decision, its alternatives, evidence and status. Decisions marked Delegated were taken by Claude under the owner's delegated authority and can be overridden by the owner at any time. Owner review on 24 Nov 2026 approved ADR-001, ADR-003, ADR-005, ADR-007, ADR-008, ADR-009 and ADR-010, and the refinements behind ADR-011 to ADR-015 as direction (each still decided in its named pass). ADR-004 and ADR-006 are confirmed with the Pass 1 spec.

## Group notes

**Additions, 24 Nov 2026 (owner-confirmed interface v3.3):**

**Additions, 24 Nov 2026 (knowledge and historiography amendment, section 59).** Status "owner-confirmed" means the owner's amendment sets the requirement; anything marked "proposed" needs engineering evaluation before it becomes a final implementation decision. Dependencies name mechanics (Mechanics section Q) and passes.

## Index

| ID | Decision | Group |
| --- | --- | --- |
| [ADR-001](0001-simulation-core-in-csharp-net-headless-library.md) | Simulation core in C# (.NET), headless library | Original table |
| [ADR-002](0002-client-engine-godot-4-with-csharp.md) | Client engine Godot 4 with C# | Original table |
| [ADR-003](0003-platforms-native-desktop-windows-macos-linux-plus.md) | Platforms: native desktop (Windows, macOS, Linux) plus headless command line; no browser or WebAssembly target | Original table |
| [ADR-004](0004-authoritative-state-uses-integers-and-per-quantity.md) | Authoritative state uses integers and per-quantity scaled fixed point, declared with unit and scale in the data schema; 128-bit intermediates | Original table |
| [ADR-005](0005-floating-point-allowed-for-presentation-forecasts-analysis.md) | Floating point allowed for presentation, forecasts, analysis and labelled approximations; in authoritative models only under a verified deterministic profile approved here after benchmarks | Original table |
| [ADR-006](0006-counter-based-random-generator-keyed-by-world.md) | Counter-based random generator keyed by world seed, turn, phase, system, entity and draw index; System.Random excluded | Original table |
| [ADR-007](0007-workspace-ids-use-the-ws-prefix-ws.md) | Workspace IDs use the WS- prefix (WS-A1 to WS-H3) | Original table |
| [ADR-008](0008-pass-order-full-declaration-level-dependency-pass.md) | Pass order: full declaration-level dependency pass at Pass 2b; synthetic early and late-game benchmark worlds added to Gate 1; validation inside every pass | Original table |
| [ADR-009](0009-performance-confirmed-values-are-design-targets-directive.md) | Performance: confirmed values are design targets, directive values are ceilings; mainstream reference hardware 6 to 8-core CPU, 16 GB RAM, SSD | Original table |
| [ADR-010](0010-one-unified-mechanic-specification-template-21-fields.md) | One unified mechanic specification template (21 fields) | Original table |
| [ADR-011](0011-analytical-propagators-cached-per-legal-time-step.md) | Analytical propagators cached per legal time step and regime | Original table |
| [ADR-012](0012-rare-hazards-scheduled-by-sampled-time-to.md) | Rare hazards scheduled by sampled time-to-event, resampled on invalidation | Original table |
| [ADR-013](0013-environmental-cells-on-an-equal-area-discrete.md) | Environmental cells on an equal-area discrete global grid | Original table |
| [ADR-014](0014-unit-typed-quantity-wrappers-in-csharp.md) | Unit-typed quantity wrappers in C# | Original table |
| [ADR-015](0015-actor-knowledge-stored-as-sparse-deltas-from.md) | Actor knowledge stored as sparse deltas from truth with observation date and decay | Original table |
| [ADR-016](0016-interface-id-convention-and-interface-architecture-registry.md) | Interface ID convention and Interface Architecture Registry | Additions, 24 Nov 2026 (owner-confirmed interface v3.3) |
| [ADR-017](0017-interface-levels-use-lv1-to-lv4.md) | Interface levels use LV1 to LV4 | Additions, 24 Nov 2026 (owner-confirmed interface v3.3) |
| [ADR-018](0018-the-interface-registry-references-data-ownership.md) | The interface registry references data ownership | Additions, 24 Nov 2026 (owner-confirmed interface v3.3) |
| [ADR-019](0019-universal-emergent-causality.md) | Universal Emergent Causality | Additions, 24 Nov 2026 (knowledge and historiography amendment, section 59) |
| [ADR-020](0020-knowledge-capability-separation.md) | Knowledge–Capability Separation | Additions, 24 Nov 2026 (knowledge and historiography amendment, section 59) |
| [ADR-021](0021-separation-of-objective-truth-observation-actor-knowledge.md) | Separation of objective truth, observation, actor knowledge, research, interpretation and publication | Additions, 24 Nov 2026 (knowledge and historiography amendment, section 59) |
| [ADR-022](0022-generalised-knowledge-artifact-identity-content-separated-from.md) | Generalised knowledge artifact identity, content separated from copies | Additions, 24 Nov 2026 (knowledge and historiography amendment, section 59) |
| [ADR-023](0023-source-independence.md) | Source independence | Additions, 24 Nov 2026 (knowledge and historiography amendment, section 59) |
| [ADR-024](0024-capability-deployment-regression-and-restoration.md) | Capability, deployment, regression and restoration | Additions, 24 Nov 2026 (knowledge and historiography amendment, section 59) |
| [ADR-025](0025-era-independent-transmission-and-open-publication.md) | Era-independent transmission and open publication | Additions, 24 Nov 2026 (knowledge and historiography amendment, section 59) |
| [ADR-026](0026-institutional-collections-and-recursive-synthesis.md) | Institutional collections and recursive synthesis | Additions, 24 Nov 2026 (knowledge and historiography amendment, section 59) |
| [ADR-027](0027-theories-peer-review-consensus-and-grand-intellectual.md) | Theories, peer review, consensus and Grand Intellectual Works | Additions, 24 Nov 2026 (knowledge and historiography amendment, section 59) |
| [ADR-028](0028-historically-situated-access-persistent-editions-on-demand.md) | Historically situated access, persistent editions, on-demand compilation | Additions, 24 Nov 2026 (knowledge and historiography amendment, section 59) |
| [ADR-029](0029-rendering-separated-from-simulation-offline-baseline-optional.md) | Rendering separated from simulation; offline baseline; optional language-model renderers | Additions, 24 Nov 2026 (knowledge and historiography amendment, section 59) |
| [ADR-030](0030-access-modes-and-archive-controls.md) | Access modes and archive controls | Additions, 24 Nov 2026 (knowledge and historiography amendment, section 59) |
| [ADR-031](0031-export-interoperability.md) | Export interoperability | Additions, 24 Nov 2026 (knowledge and historiography amendment, section 59) |
| [ADR-032](0032-extension-compatibility.md) | Extension compatibility | Additions, 24 Nov 2026 (knowledge and historiography amendment, section 59) |
| [ADR-033](0033-computational-scaling-and-aggregation-for-knowledge-and.md) | Computational scaling and aggregation for knowledge and history | Additions, 24 Nov 2026 (knowledge and historiography amendment, section 59) |
| [ADR-034](0034-cross-domain-causal-traceability.md) | Cross-domain causal traceability | Additions, 24 Nov 2026 (knowledge and historiography amendment, section 59) |
| [ADR-035](0035-truth-evidence-belief-consequence-separation.md) | Truth–Evidence–Belief–Consequence Separation | Additions, 24 Nov 2026 (knowledge and historiography amendment, section 59) |
| [ADR-036](0036-ftl-horizon-value-x28.md) | FTL horizon value (X28) | Additions, 24 Nov 2026 (knowledge and historiography amendment, section 59) |
| [ADR-037](0037-registries-as-versioned-yaml-files-in-the.md) | Registries as versioned YAML files in the repository | Additions, 24 Nov 2026 (knowledge and historiography amendment, section 59) |
| [ADR-038](0038-twenty-eight-owner-systems-with-single-writer.md) | Twenty-eight owner systems with single-writer ownership | Additions, 24 Nov 2026 (knowledge and historiography amendment, section 59) |
| [ADR-039](0039-pass1-time-rule-clarifications.md) | Pass 1 time-rule clarifications (owner rulings) | Proposed, 24 Nov 2026 (Pass 1 grilling session) |
| [ADR-040](0040-target-dotnet-10-lts.md) | Target .NET 10 (LTS) | Proposed, 24 Nov 2026 (Pass 1, J0) |
| [ADR-041](0041-build-test-and-ci-settings.md) | Build, test and CI settings | Proposed, 24 Nov 2026 (Pass 1, J0) |
