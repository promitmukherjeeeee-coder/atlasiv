# Grand 4X Strategy Game: Final Master Vision

Nov 22, 2026 · @Promit

A single-player-first, turn-based grand-strategy sandbox where you guide one people through its entire history, from 10,000 BCE until a special project first proves faster-than-light travel (around 2450 CE), on a world that simulates itself. Companion tabs: Mechanics Brainstorm (the full mechanics set) and Deep Dives (resources, characters, intrigue, warfare).

## 1. Vision

The game combines the scope of Civilization, Europa Universalis, Crusader Kings, Hearts of Iron, Old World and Stellaris (only Stellaris's emergent scripted and unscripted stories), plus the best community-mod features of each. Every game should feel like a complete lived history: billions of stories across up to 10,000 turns. The goals are maximum depth and real-life mechanical resemblance. A steep learning curve is accepted.

## 2. Pillars

1. **Emergence over scripting.** Real-world outcomes arise from interacting equations, never from hard-coded rules. Real historical cases are the validation suite for every system. One declared exception: the real benefit of FTL lies outside the simulated world, so it is a hard-coded incentive that every actor knows (section 4.4, X28). Everything that incentive influences still runs through simulated systems.
2. **A living world.** Terrain, water, climate, vegetation and animals run as simulations. Civilizations shape the world and it pushes back.
3. **Every era, every path.** Branching tech and civic trees allow normal, cyber, solar, nature or steampunk civilizations.
4. **People, not just numbers.** Populations, leaders, families, dynasties, factions and religions act on their own interests.
5. **Interlocking systems.** Economy, diplomacy, religion, war and environment feed each other.

**Foundational axioms (owner-confirmed, 24 Nov 2026).** Three governing principles apply to every system: maximum meaningful complexity with minimum necessary computation; mathematical causality before scripted outcomes; knowledge is not capability.

- **Universal Emergent Causality.** Outcomes in every domain (physical, environmental, demographic, economic, political, technological, institutional, cultural, religious, scientific, social) arise from interacting simulated conditions, entities, constraints, mathematical relationships, stochastic processes and agent decisions. Eras describe a civilisation's aggregate condition and never act as causes. Technologies change actual capabilities, processes, constraints, costs, infrastructure, information transmission, production functions and feasible actions, and their benefits emerge from those changes, not from additive bonuses. Physical laws, justified coefficients, feasibility conditions, thresholds, stochastic processes, agent decisions, authored presentation, performance abstractions and chosen scenario initial conditions remain allowed, but never to disguise predetermined outcomes. One declared exogenous term is allowed: the FTL horizon value (X28), because the benefits it stands for lie outside the simulation; it acts only through declared valuation paths. Every mechanic can influence every relevant mechanic through typed, sparse, traceable dependencies (not indiscriminate coupling), with uncontrolled positive feedback, duplicated benefits and divergence prevented. The Universal Causal Inspector (GS-04) explains why an outcome occurred, which variables and direct or indirect pathways contributed, which constraints blocked alternatives, which interventions could have changed it, and what uncertainty remains. Deterministic replay is preserved.
- **Knowledge–Capability Separation.** Existence, exposure, accessibility, acquisition, possession, preservation, comprehension, validation, absorption, synthesis, application, operational capability, deployment and diffusion are independently represented states. Knowledge never automatically confers technological, scientific, industrial, institutional, administrative, military or economic capability. A technology is never an irreversible, universally operational unlock: it can be known but not understood, understood but not reproducible, reproducible experimentally but not commercially, feasible but not deployed, deployed in one place but not another, and lost while its documentation survives.
- **Truth–Evidence–Belief–Consequence Separation (added 24 Nov 2026).** Objective truth, observable evidence, evidentiary strength, perceived credibility, subjective belief, institutional acceptance, public acceptance, strategic utility and material consequences are independently represented. Information need not be true to have real political, diplomatic, economic, financial, institutional, cultural or social consequences; false, incomplete or misleading information can carry great strategic value when actors find it credible or expect others to act on it, and accurate information can carry little when it lacks credibility, access, relevance or actionability. Leverage comes from the expected consequences of possessing, disclosing, circulating, accepting or using information, and is contextual, target-specific, issue-specific and open to counter-evidence, verification, disclosure, changing circumstances and counter-leverage. Actors decide from legitimately accessible information, beliefs, expectations, incentives and their assessment of how others will behave. Actual evidence is distinct from perceived evidence, independent corroboration from repetition, credibility from correctness. Later correction does not automatically reverse consequences already produced. All consequences flow through existing mathematical systems and declared dependencies, and objective truth stays independent of beliefs, narratives, consensus and interpretation.

## 3. Scope and priorities

- Single-player against AI first. Multiplayer is not being designed now.
- No timers: unlimited thinking time every turn. Platform: native desktop (Windows, macOS, Linux), with a headless command-line simulation for testing and calibration, and no browser target (ADR-003).
- Core systems come first. Graphics and UI are last and least important.
- Current phase: planning. We detail the mathematics, logic, hierarchy and inter-relationships of every mechanic, in this order: vision, full mechanics brainstorm, dependency layers, then next steps.
- Stack decisions are deferred (candidates: Figma Make, Claude Pro MCP integration, Supabase).
- No modding support, no plugin or extension API and no mod tools (owner-confirmed 24 Nov 2026). Only the source code is published. Every capability the design asks for, including campaign histories and their export, is built in-house as a capability expansion.
- Visual direction: an antique survey-sheet map (contours, hillshading, lettered grid, scale bar) inside a dark framed UI with a top resource bar, left navigation, right panel, minimap and bottom action bar. The reference mockups are concepts, and all their text is placeholder. The map starts purely natural and fills with clutter (roads, fields, towns, mines, forts, borders) as the game progresses. The top bar shows Turn N, Phase k of N, and the date range of the phase. End Turn becomes End Phase, with a turn-boundary marker showing when the slow systems will tick. An Advance Until control with an exception filter list, and a turn summary after each turn-end tick, round out the turn controls.

## 4. Core constraints

### 4.1 Turns and time

A turn is the world's tick for slow systems. Each turn is divided into action phases, which are the tick for fast systems and for the player's decisions. Turn length steps down as the leading edge gets faster, and the number of phases per turn changes with the step, so each decision covers a comparable stretch of felt history. Everything is deterministic, with unlimited thinking time and no timers. A game runs up to 10,000 turns, and a substantial continental undertaking takes about 2 turns of strategic operational pacing at every step. Physical travel keeps its real speed, and fast transport can complete several journeys within one phase.

**Turn steps and phases.** Confirmed: the eight steps (50 y, 20 y, 10 y, 5 y, 2 y, 1 y, 6 months, 1 month), per-year rates, and phases that scale with the step. Proposed and tunable: the phase counts.

| Turn step | Phases per turn | Phase length | Typical era | Approx. turns |
| --- | --- | --- | --- | --- |
| 50 y | 20 | 2.5 y | 10,000 to 3000 BCE | 140 |
| 20 y | 10 | 2 y | 3000 to 1000 BCE | 100 |
| 10 y | 10 | 1 y | 1000 BCE to 500 CE | 150 |
| 5 y | 5 | 1 y | 500 to 1500 CE | 200 |
| 2 y | 4 | 6 months | 1500 to 1800 | 150 |
| 1 y | 4 | 3 months | 1800 to 1950 | 150 |
| 6 months | 3 | 2 months | 1950 to 2000 | 100 |
| 1 month | 5 | 6 days | 2000 to 2450 | 5,400 |

Eras are indicative only. The step trigger decides the real step. A full run from 10,000 BCE to 2450 CE is about 6,390 turns and 34,800 phases, which leaves about 3,600 turns of headroom under the 10,000-turn cap. About 78% of all phases fall after 2000 CE, so the late game needs the deepest content and the most auto-skipping.

**Step trigger (proposed).**

- Leading edge: the fastest transport capability held by any entity with at least 0.5% of world population. The size floor stops a tiny outlier from dragging the world forward.
- Crossing time T\_cross is a reference distance (a continent crossing, calibrated in the data phase) divided by that entity's best effective speed for people, armies and goods over its network. Communication speed is handled separately by administrative reach (N12).
- Target: T\_cross is about 2 turns. For each step, compare its length with T\_cross / 2 on a log scale and pick the closest as the candidate step.
- Switch only when the candidate differs from the current step for 10 consecutive turns (faster) or 20 consecutive turns (slower), with at least 20 turns between any two switches, one step at a time.
- Every switch is an announced era-transition event (K9) with a preview of what changes. Steps can go slower again after a global collapse.

**Anatomy of a turn and a phase.** A turn runs phases 1 to N, then the turn-end tick. Each phase:

1. Planning. The player plans with no timer. Every AI entity plans under the same rules.
2. Resolution in a fixed order: standing orders and scheduled events, diplomacy, covert operations, movement and migration, combat, sieges and naval action, character events, unrest events.
3. The phase is written to the history log.

Rules:

- Simultaneous conflicting orders (two entities claiming the same zone) resolve by deterministic contest rules: strength, proximity, order priority.
- Random draws come from seeded streams keyed by turn, phase, system and entity, never by iteration order.
- Phase events that affect slow systems (a siege starving a province, battle casualties, a migration) are recorded as phase deltas. The turn-end tick consumes them, so no information is lost.

| Resolves every phase | Resolves once per turn |
| --- | --- |
| Movement and migration (P4, N6) | Climate, hydrology, soil, biomes (W1 to W9, W11) |
| Logistics and attrition (M2) | Animal and disease ecology (W12 to W16) |
| Combat, sieges, naval, air (M4 to M7) | Demography and carrying capacity (P1 to P3) |
| Diplomacy and treaties (D1 to D15) | Settlement growth, districts, adjacency (P5 to P8) |
| Covert operations (D11, K8) | Economy, trade, finance, energy (E and R series) |
| Character and dynasty events (S5, S6, N14) | Technology invention and diffusion (K series) |
| Unrest, revolts, coups (S7) | Culture pull and assimilation (N3 to N5) |
| Disaster events (W10) | Institutions, factions, religion (S1 to S4) |
| Standing orders and event queue (T2, T4) | Scoring sample, chronicle, log flush (X2, X6, T6) |

Turn-end tick order: climate and water, biomes, demography, production and consumption, markets and trade, finance, technology, culture, institutions, scoring sample, turn summary.

**Multiplayer, when it is designed:** simultaneous planning, deterministic resolution, and an AI regent covering absent players. Timers become optional then.

### 4.2 Technology diffusion and isolation

- Laggards catch up through an exponentially rising transfer rate as more of the world holds a technology.
- Diffusion runs through channels (trade, migration, media and internet, students and travel, espionage, conquest, diplomacy), each with its own permeability from 0 to 1.
- Independent invention depends on population size, internal connectivity, surplus, specialization, literacy, need and prerequisites. For a fully isolated people it can stay near zero, as with the Sentinelese. Nothing forces creep.
- Embargoes, sanctions, closed-system internet and religious or cultural rejection slow diffusion but never block it unless a people is truly cut off.
- The same equations must produce North Korea, China, India (a hybrid of control and liberty) and other real cases.
- First movers keep lasting rents through the technology-monopoly mechanics, shrinking as more of the world adopts the tech.

### 4.3 Population

- User-facing numbers use real-life scale, with no conversion to backend units.
- Primary cohort key: Province, Culture, Religion and Class. Each group holds an age distribution (five-year bands with an open-ended last band) plus conditional distributions for occupation, education and health, and sparse exceptional subcohorts where aggregate independence would distort results. This replaces the earlier schema that put age bands in the key.
- The target is about 12 active primary groups per province. It is a target, not a cap: merge similar small groups while keeping ancestry and hidden subdistributions, preserve significant minorities, and expand an aggregate group when an event makes its distinctions matter.
- Population is exact integers. Every update satisfies end = start + births - deaths + immigration - emigration. Migration, conversion, class mobility, aging and territorial transfer never change totals, and a failed reconciliation is a simulation error.
- Aging uses an age-transition operator based on elapsed time, so results do not depend on how many turns cover the same duration.
- Ordinary households are statistical. Explicit families exist for notables and dynasties.
- Rates use fixed-point math with seeded stochastic rounding, and provinces far from the player update at coarser intervals.

### 4.4 End condition and the FTL project

- The game ends when a special project successfully test-runs the first working FTL drive. 2450 CE is a rough projection, not a deadline. The game can end earlier or later, and play continues under the existing rules past 2450 CE. If the 10,000-turn limit is reached without FTL, the game ends with a full historical assessment and an explicit unfinished-campaign outcome. FTL is never granted merely because a year arrives. There is no playable FTL era. Space is in scope only through its effects on Earth (launch, satellites, debris, planetary defense).
- Any entity with enough funding, labor and prerequisite tech can sponsor the project: a civilization, a multi-civilization consortium (like the UK-US nuclear project), a corporation, or a private research body. Sponsors hold shares that set their scoring credit and their control of the result.
- Real-life frictions apply: espionage and leaks, sanctions on materials, secrecy. The project advances in stages, each consuming real resources, and can fail for explicit technical or institutional reasons. It needs advanced capability in fundamental science, energy, materials, computing, precision manufacturing, industrial integration, specialized labor, institutional coordination, finance, experimental infrastructure and safety testing. No money can substitute for a missing physical prerequisite. Contribution records separate finance, science, infrastructure, materials, labor, intellectual property and governance control, and sponsor shares reconcile to the agreed total. The game states explicit fictional assumptions for FTL and does not present speculative physics as established science.
- FTL will eventually be discovered. The pressure toward it has two parts. The first emerges from simulated conditions: growing populations and demand against Earth's finite land, resources and production, the rising payoff of space projects, scientific incentives and competition. The second is the FTL horizon value (X28, owner-confirmed 24 Nov 2026): the real benefits of FTL lie beyond Earth and after the game ends, where no simulated system can produce them, so they are represented by a hard-coded incentive. Every actor, player and AI alike, knows it, and it weighs in their valuation of FTL-related research, institutions, funding and sponsorship. It never grants progress by itself, and there is still no fixed deadline. Its size and any change over time are calibration parameters (section 9.19). Anything after FTL is out of scope. The discovery itself still needs the tech path.

### 4.5 Scoring

Every category is a deterministic metric computed from the final state and the history log. Types: **End state** is sampled at the end, **Peak** is the maximum over history, **Lifetime** is the accumulated total, **Average** is the mean over time. Rankings include every people, player or not, so the Sentinelese can top ecology. The headline is the rank in each category. An overall composite uses visible weights and never hides category winners. Category formulas are transparent and distinguish absolute from population-adjusted achievements. Territory measures stable geographic equivalents, so creating more provinces earns nothing, and long-duration metrics use elapsed years, not turn counts. Achievements belong to the lineage that produced them: on fission, successors inherit connections without multiplying credit, on merger the contributing lineages keep attributable achievements, and shared achievements use defined contribution shares. Cross-era comparison uses invariant concepts with historically appropriate proxies, and prehistoric societies are not marked deficient for lacking modern institutions. The player's Career Record (which entities they led, when, and what happened under them) is separate and is not a 25th category.

| Category | Measure | Type |
| --- | --- | --- |
| Ecological footprint | Lowest cumulative emissions, habitat loss, extinctions caused | Lifetime |
| Biodiversity stewardship | Species preserved and restored | End state |
| State control | Central control over media, movement, economy | Peak |
| Freedom and rights | Civil liberties and political participation | Average |
| Military power | Forces, tech level, logistics reach | Peak |
| Territory | Provinces held and settled | Peak |
| Population | People alive | Peak |
| Health and longevity | Life expectancy, famine and plague deaths avoided | Average |
| Education | Share of population literate and trained | End state |
| Wealth | Total economic output | Peak |
| Prosperity per person | Output per capita | End state |
| Equality | Spread of wealth and status | Average |
| Trade | Volume and trade nodes controlled | Lifetime |
| Currency hegemony | Reserve share and financial influence | Peak |
| Corporate reach | Corporate influence abroad | Peak |
| Technology leadership | Techs held ahead of the world | End state |
| First discoveries | Techs discovered first | Lifetime |
| FTL contribution | Share of the winning project | End state |
| Cultural influence | Media reach, language spread, culture adopted by others | Peak |
| Religious reach | Followers and internal cohesion | Peak |
| Stability and longevity | Years without collapse or civil war, dynasty length | Lifetime |
| Resilience | Disasters survived, recoveries from collapse | Lifetime |
| Heritage | Wonders, monuments, great works | Lifetime |
| Diplomacy | Alliances, treaties kept, influence | Average |

## 5. World structure

- A fixed-size, zoomable map of real Earth, divided into natural zones, which split into province zones as they develop. Each tile is a province, and provinces form a region.
- Start: migratory peoples roam the natural zones, and six cradle areas (Mesopotamia, Egypt, Indus Valley, China, Peru, Mexico) already have people beginning to settle. The player starts as one entity in one cradle zone, and may stay migratory for a long time.
- Four height levels: lowland, plateau, highland, mountain.
- Six biome categories, each with its own modifiers and statistics. A region has one dominant biome, while finer cells inside it may differ (a mountain region can hold forest, highland grassland and rivers): ice and snow; forest (tropical rainforest, temperate, taiga/boreal); grassland (savanna, temperate grassland/steppe/prairie); desert (hot, cold); tundra; aquatic (one biome, closely coupled to weather, climate, disasters, resources and navigation; ocean regions hold no settlements).
- One city per region, plus multiple towns and villages. City cost rises geometrically and universally. Town cost rises progressively, per region only. Each has unique districts, buildings and upgrades.
- Improvements (dams, farms and so on) are placed adjacent to a city, town or village. Cities give +3 adjacency, towns and villages +1.
- Some improvements, buildable only in cities, affect the whole region (for example irrigation systems).

* All other geographic details derive from real-world data: terrain and heights, coastlines, rivers, biomes, climate, resources, animal ranges and the starting distribution of peoples. The 10,000 BCE start uses reconstructions of that era (lower sea level, retreating ice sheets, different coastlines), which the simulation then evolves.

- Geographic scale: about 750 natural zones at the start (acceptable range 500 to 1,000) subdivide into about 15,000 land provinces (design envelope 20,000), roughly 20 per zone on average depending on geography and history. Oceans and major aquatic regions use a separate, coarser network. Regions are generated from province relationships.
- Spatial model: a hybrid. Height fields and rasters for physical geography, polygons for zones, provinces, regions and borders, graphs for waterways, roads, trade, power and communications, and point features for settlements, facilities and resources. Environmental cells of about 10 to 20 km are used where needed, with finer data for coastlines, rivers, urban geometry and major infrastructure. Province identifiers stay stable while terrain, rainfall, ecology and land use vary inside them.
- Place identity: every place has a permanent identifier independent of its name, borders and owner. Child provinces inherit ancestry, merged provinces keep their historical identifiers, and changes create dated versions and events.
- Political boundaries keep separate relationships for sovereign claimant, recognized sovereign, de jure administrator, de facto controller, military occupier and local authority. Conquest changes control before it changes sovereignty.
- Settlement hierarchy: Camp, Village, Town, City, Metropolitan center. Metropolitan is an advanced city class, not a second city. One city per administrative region stays. Large urban areas appear as large towns, metropolitan districts or conurbations, and a region can be formally subdivided so each part develops its own city. City founding cost escalation and per-region town costs are unchanged.
- Urban growth consumes real land through demand-driven expansion into eligible adjacent cells, and affects food, habitat, runoff, transport and pollution.
- Terrain evolution: geology changes slowly. Rivers, wetlands, deltas, coasts, soils, glaciers, vegetation, land cover and built terrain evolve through physical processes. The four elevation classes are gameplay abstractions derived from physical terrain.
- Starting conditions: reconstructed Earth at about 10,000 BCE (elevation, coastlines, glaciers, sea level, rivers, climates, biomes, animal ranges, resources, carrying capacity, human distribution). The six cradles reflect reconstructed conditions, so not all start with developed agriculture. Historical resemblance arises from constraints, and eras are descriptive classifications, not a forced ancient-medieval-modern sequence.

## 6. Peoples and entities

The direction is set; exact numbers belong to the mechanics phase.

- **What an entity is.** Any organized people: a migratory band, tribe, chiefdom, city-state, kingdom, empire, nation-state or consortium. It is defined by its population groups, territory, institutions, leader and dynasty, and culture traits. Its form is not a scripted tier. It follows measurable thresholds (settlement, surplus, administrative capacity, cohesion).
- **Where peoples come from.** At the start, migratory peoples are seeded in natural zones according to carrying capacity (biome, animals, water), and the six cradles hold settling groups. The number of peoples seeded in each natural zone and cradle is selectable by the player when creating the game, with defaults taken from real-world data. After that, peoples arise naturally: bands split when they outgrow their land, merge, settle when surplus and domesticable resources exist, collapse, or break away in revolt. Non-player peoples are AI entities running the same rules as the player.
- **How entities interact.** War, trade, dynastic marriage, tribute and vassalage, migration, diplomacy, assimilation and absorption.
- **Cultural pull.** Each entity pulls nearby population groups toward its culture, based on prestige, surplus, tech, religion, language prestige, trade ties, proximity and, later, media. These are the cultural points that drive assimilation. A group builds affinity toward the strongest pull, and past a threshold it adopts that culture voluntarily, as in Romanization. Conquest speeds this up but raises unrest.
- **Absorption.** Conquest or voluntary union moves population groups into the player's entity. Absorbed groups keep their culture until assimilated, and cultural distance sets unrest.
- **Trait inheritance.** A culture is a bundle of weighted traits (for example seafaring, metallurgical tradition, law customs). Absorbing a people adds its traits in proportion to its share and prestige, up to a cap. Over generations the blend forms a new culture, and languages drift. Knowledge held by absorbed people spreads through the conquest and migration channels.
- **Transformation.** A national project or special project can change an entity's form. A dominant tribe that completes a founding-of-a-capital project becomes a chiefdom. After absorbing its neighbors, completing a charter project yields a new named entity with a blended culture, governance form and civic options (a Sumerian city-state becoming an Akkadian-style empire, for example). Projects have requirements (population, surplus, cohesion, legitimacy), a cost and a duration. The same mechanism carries the FTL project.
- **Continuity and perspective.** History and scoring follow each entity's lineage across transformations. The player always plays as the character leading the selected country and can switch countries at any time, as with Europa Universalis tags. The ruler's traits, health, family and ambitions shape what the player can do, play continues as the heir when the ruler dies, and a country the player leaves returns to AI control.

* **Characters across eras (proposed).** Detail scales with the era. Bands and tribes have an anonymous leader with traits, and a successor is rolled on death. Chiefdoms and early states have a named ruler, heir and notables, with dynastic stances (succession, marriage, patronage). House members become active from 10-year turns, and full Crusader Kings-style detail starts at 5-year turns. Only notables are simulated per phase, and aging, fertility and mortality integrate over the phase length. If a ruler dies mid-turn, play continues as the heir from the next phase.

- **Information and uncertainty.** The simulation keeps an objective world state, each entity's knowledge, and the player's visible knowledge as separate records. The player sees exact values for the controlled entity's own accounting statistics, and intelligence-dependent estimates with confidence ranges and observation dates for foreign entities. Unexplored territory is unknown, and contact alone does not reveal another civilization's internals.
- **AI.** All AI entities obey the same physical, economic, political and informational rules as the player, with no resource multipliers, free technologies or hidden bonuses. Difficulty changes only planning horizon, quality of evaluation, alternatives examined, institutional coordination, forecasting sophistication and tolerance for complexity. Each AI decides from its own knowledge state, and a government is not one perfectly unified will.
- **Collapse and elimination.** Collapse is classified as temporary crisis, political collapse, fragmentation, civilizational transformation or extinction. When the controlled entity ceases to exist, its record and lineage stay preserved, the disappearance is a dated event, legitimate successors are offered, and the player chooses a successor or switches. The simulation continues. If no eligible entity remains, a terminal-state assessment applies. Recovery depends on surviving population, knowledge, infrastructure, resources, institutions, culture, trade and organization, and lost capabilities must be rebuilt.
- **Civilization identity.** A civilization is a lineage network. Territory, government, language or religion can change without ending it, and splits and mergers create branching ancestry.
- **Player goals.** An open-ended sandbox. Momentum comes from ruler ambitions, dynastic interests, projects, opportunities, discoveries and crises, with optional soft objectives and no mandatory missions.

## 7. Feature scope

Detailed in the mechanics brainstorm.

**World simulation**

- Animal populations (growth, decline, extinction), animal kingdoms, biocycles, natural exploitation options.
- Natural disasters, sea levels, global warming, climatic cycles.
- Expanding and shrinking vegetation; biomes that change into other biomes.
- Dynamic rivers and water flow, with natural flooding that is not scripted.
- Culture-based looks and skins.

**Technology and resources**

- Branching tech tree and civic tree.
- Natural resources feed tiered man-made resources (four grades per chain) built on each other (for example silicon, then transistors, then SoCs, then supercomputers, then quantum computers). Higher steps are often infrastructure assets that deliver capabilities, not consumable resources. Aluminium is unlocked by discovering bauxite and researching smelting.
- Mining clusters and mineral families: one site can hold several resources (coal and diamond; bauxite and magnesium ore; uranium, polonium and thorium, convertible with tech or via a multi-generational breeder program).

**Economy**

- Media influence and output, entertainers, language systems.
- Internet with sector upgrades (corporate, economic, infrastructure) creating open or closed systems.
- Trade nodes (merchants) and trade routes (traders).
- National currencies, global reserve currency, fractional reserve banking, corporations, stock trading, public listings, corporate influence.
- Monopolies (cost, technology, material, trade), embargoes, sanctions, currency dynamics and conversion, currency hegemony, world banks.

**Society and diplomacy**

- Political factions and approval.
- HRE mechanics from EUIV, colonial nations, the combined diplomacy of Civ IV and EUIV.
- Characters and lineages: hundreds of families, dynasties, heirs, marriages and interpersonal stories (friendships, rivalries, feuds, intrigues), plus paragons and heroes. Detail is tiered: notable characters get full detail, house members lighter detail, and background families are aggregated, with promotion and demotion as events involve them.
- Religion: administration, cohesion, factions, Defender of the Faith, religion civic tree. Culture civic tree.
- Corporate, religious, civilian and military diplomacy.

**Defining systems**

- **Living Constitutional Order and Civilisational Legal Memory.** Governments are never chosen from a list. A civilisation's political identity accumulates from centuries of decisions, precedents, institutions and settlements, and can be codified into real in-game documents whose every provision traces to history and drives an operative rule. The order produces the document, and the document then shapes the order. Detail in section 9.15.
- **Autonomous institutions and transnational power.** Guilds, chartered companies, corporations, banks, universities, academies and international organisations act as persistent, independent participants with their own governance, assets, interests and, sometimes, territory and armed forces. Detail in section 9.16.
- **Nested command warfare.** Strategic, operational and tactical scales, with command delegation, a demographic military, physical logistics, operational geography and politically resolved war termination. Detail in section 9.17.

* **Living Knowledge Order and Living Historiography.** Knowledge exists as identifiable observations, records, theories, methods, works, datasets and expertise that are created, acquired, preserved, translated, contested, synthesised, validated, absorbed, applied, lost and rediscovered. Historians produce competing, readable accounts of one objective history, and every campaign leaves a readable, inspectable historical and intellectual legacy. Detail in section 9.18.

**Emergent narrative**

- Story events arising from the simulation, plus special projects and other mechanics that help, frame or support the stories. Together with the characters and lineages, they produce the full narrative arc, from the micro-stories of individuals to the sweep of whole civilizations.

## 8. Further confirmed rules

- Rates stored per year, with turn length changing in defined steps (50 years, 20, 10, 5, 2, 1 year, 6 months, 1 month). The phase table is in section 4.1.
- Budget note: if the world reaches 2450 CE, monthly turns from 2000 CE alone use about 5,400 turns, so the late eras need the most depth. A full run is about 6,390 turns and 34,800 phases, with about 78% of phases after 2000 CE.
- Absorptive capacity: a people can learn of a technology quickly but needs prerequisites and infrastructure to use it. Small populations may lose technologies.
- Biome conversion driven by temperature, precipitation, seasonality, elevation, water, and human and animal impact, emerging from the climate simulation.
- Map art rendered from simulation data rather than painted, with every symbol tied to a data-model object.
- Data-driven content tables for development and balancing (not for modding).
- The final tech tiers before FTL are the endgame content.
- Engineering: deterministic simulation (also keeps future multiplayer possible), a per-turn compute budget with level-of-detail simulation, AI-legible mechanics, explainable numbers (every figure shows why it changed), and a validation suite of real cases (Sentinelese, North Korea, China after 1978, Meiji Japan, India, Tasmania).

* **Intra-turn accuracy (proposed).** Slow systems use closed-form integration over the full turn. Seasonal cycles use precomputed annual profiles per biome, so a 50-year turn still has correct harvest and flood rhythms. Random events are Poisson draws (rate times duration). Provinces flagged volatile (famine, flood, siege, epidemic) may use up to 4 adaptive substeps at the turn-end tick. Conservation checks run every turn: people, stocks and money must balance.
* **Fast-forward and level of detail (proposed).** There is one global clock, and laggards accrue per-year rates over shorter increments. The player can advance until an exception: war declared, enemy army enters territory, succession crisis, standing order done or failed, first contact, large disaster, plot discovered, treaty offer, project milestone, famine threshold, new technology. Phases with no orders and no exceptions auto-skip, which is essential given about 27,000 monthly phases and free country switching. The player and contacts act every phase, other entities every 2 to 4 phases, and remote entities once per turn at a fixed offset phase derived from the entity id, which spreads load deterministically. Any event involving a remote entity wakes it to full cadence. Standing orders progress per phase and carry across turns.
* **Compute budget (proposed).** About 2 s per late-game turn (5 phases plus the turn-end tick) on a mid-range laptop, 10 s worst case. The turn-end tick takes up to about 1 s and each phase up to about 0.2 s. Each system declares its natural period, so monthly turns do not rerun yearly systems, and phase work is proportional to active actors, not the whole world.
* **Turn validation tests (proposed).** Convergence (a 50 y run matches the same scenario at 1 y steps within tolerance), determinism (replay hashes identical across machines), strategic pacing (a substantial continental undertaking takes about 2 turns at every step, while physical travel stays plausible), reign coverage (an early-era ruler spans at least 8 phases), and auto-skip (a quiet monthly stretch resolves without player input).

## 9. Confirmed architecture decisions

All items below come from the Q&A document and are confirmed. Performance and size figures are design targets until benchmarks demonstrate them.

### 9.1 Time and resolution

- One authoritative integer clock counts elapsed seconds from the 10,000 BCE start. It is event-driven, so nothing updates every second. Phase and turn boundaries use exact calendar dates.
- There are eight turn steps with seven forward transitions (reverse transitions are possible). A new step starts at the next turn boundary, with a preview of what changes. Standing orders keep their real elapsed-time requirements. Transport must be operationally usable at meaningful capacity to set the leading edge, and eras are descriptive, never triggers.
- Systems have their own natural periods. Fast: movement, logistics, military, diplomacy, covert operations, urgent political events, standing orders. Slow: demographic structure, institutions, technology, settlement growth, structural economy. Event-driven: disasters, discoveries, deaths, battles, failures, elections, project completion. Immediate stocks stay current through the turn, and the turn-end tick does structural integration and reconciliation.
- Events carry timestamps and enter a deterministic priority queue keyed by timestamp, resolution stage, event priority and stable event ID. The player acts at phase boundaries or at rare, formally defined interruption points. Informational events resolve automatically, and decision-critical events pause Advance Until.
- Phase resolution: snapshot state; collect and validate orders; commit standing orders; scheduled events and communications; diplomacy and covert actions; movement, transport and logistics; encounters, battles, sieges and occupation; character, political and unrest events; immediate stock transfers; record deltas, events and provenance; publish the player-visible state. Turn-end resolution: climate and water; ecology and biomes; demographic reconciliation; agriculture, extraction, production and consumption; trade, markets and logistics accounting; finance and ownership; knowledge and diffusion; culture, religion and institutions; derived statistics and scoring samples; records, summaries and snapshots.
- Simultaneous orders resolve against the same snapshot by domain-specific contest or allocation rules, with no advantage from internal list order.
- Circular dependencies: build the dependency graph, find strongly connected components, and give each an explicit solver (analytical, bounded deterministic iteration, or a documented lag) with convergence criteria, iteration caps and fallbacks. Outcomes never depend on execution order.
- Numerics: fixed-point for conserved quantities, non-negative stocks, maximum rates, adaptive local substeps, exact integer reconciliation and conservation assertions. Integration matches each process (flows, exponential and logistic growth, seasonal profiles, interval accounting, adaptive epidemics).
- Standing orders carry issuer, authority, target, limits, start condition, constraints, priority, suspension rules and an exception policy. When impossible, unaffordable or unauthorized they become paused, partial or failed. No irreversible action continues without authority, and committed costs are not refunded.
- Advance Until stops by default on war declaration or incursion, succession crisis, first contact, major disaster, famine threshold, hostile plot, treaty needing action, standing-order failure, project milestone, major discovery, and imminent rebellion or constitutional crisis. Filters can be customized but never disable state-consistency checks.
- Physical travel, strategic deployment, administrative execution and turn tempo are separate quantities. The two-turn target is strategic operational pacing, not a limit on physical movement.

### 9.2 Spatial rules

- Roads, rail, waterways, canals, pipelines, grids and telecommunications are graphs embedded in geography. Edges carry distance, capacity, speed, cost, condition, ownership, standard, vulnerability and seasonal availability. Routing minimizes generalized cost (time, money, risk, capacity, access, transfers), congestion raises cost, and military supply can use civilian networks subject to permissions.
- Every population count, material quantity and flow has exactly one authoritative owner and location at a given accounting level. Totals derive from disjoint membership, cross-boundary flows are transfers, and historical statistics use the boundaries valid at the requested date.
- Each entity has its own discovered map with confidence values. Exploration reveals specific knowledge, and maps can be traded, stolen, shared or become outdated.

### 9.3 Population dynamics

- Expected births and deaths are eligible or exposed population times a hazard times elapsed time, with bounded hazard conversion and seeded stochastic rounding. Fertility depends on health, nutrition, norms and incentives, and mortality on age, disease, famine, hazards, violence and healthcare.
- Migration follows expected welfare gain (income, food and water security, housing, safety, persecution, familiarity, networks, restrictions, transport cost, climate stress, family ties) with a bounded utility model. Refugees and nomads use different constraints. Each move is a matched departure and arrival, and deaths in transit are mortality events.
- Assimilation uses cultural pull. It transfers people between cohort identities, culture and faith can change independently, ancestry stays recorded, and conquest speeds pressure but can radicalize opposition.
- Disease uses compartmental models (extended susceptible, exposed, infectious, recovered), linked by trade, armies and migration, with distinct immunity histories.
- Urbanization is a population-location transfer. If urban capacity falls short, overcrowding and informal settlements appear instead of migration being blocked. Class mobility conserves population. Collective behavior comes from group-specific conditions and values and can produce protest, migration, mobilization, cooperation or rebellion.

### 9.4 Economy and production

- Households, firms, farms, merchants, banks, governments, cooperatives, religious institutions and investors share one actor framework with defined rights over assets, inventories, labor, money, debt and contracts.
- Physical stocks balance: closing stock = opening stock + extraction + production + imports + recovery - consumption - exports - transformation inputs - losses. Shortages cannot be solved with imaginary inventory.
- Every recipe lists inputs, outputs, byproducts, waste, energy, water, labor and skill, technology, equipment, quality limits, time and failure parameters. Recipes form a directed network. Output is the minimum of input, labor, energy, equipment, transport and permitted capacity, then recipe efficiency applies. Substitution is allowed where technically possible but can cost performance, and some bottlenecks cannot be substituted.
- Grades G1 to G4 apply within each chain above natural and refined materials. They are not universal power multipliers: a G3 power station and a G3 computing facility differ in performance.
- Manufactured goods become capital through construction, installation and commissioning. Assets track owner, location, capacity, labor, maintenance, efficiency, condition, compatibility and residual life. Wear, damage, maintenance backlog, obsolescence, stranding and destruction are separate.
- Ownership is separate from operational control, and ownership shares sum to the legally recognized total.
- Markets clear through deterministic constrained processes (auctions, contracts, administered prices, bounded damped adjustment). If a market cannot clear, shortages, rationing or unsold inventory remain. Labor markets aggregate by location, occupation and skill, and skills do not transfer instantly between unrelated professions.
- Trade is contracts, payments and physical shipments. Nodes do not create goods, tariffs move money, and unfulfilled contracts create obligations or defaults.
- Money regimes include barter, commodity money, coinage, credit, paper, fiat and digital money. Money is created and destroyed only through modeled issuance, lending, repayment or retirement. Banks keep balance sheets (assets = liabilities + equity), and failures spread through counterparties.
- Government accounts follow closing cash = opening cash + revenue + borrowing + monetary financing - expenditure - debt repayment. Public debt always has a creditor. Defaults stay recorded.
- Inflation is the change in a price index over an explicit consumption basket, never an applied modifier. Exchange rates arise from trade settlement, capital flows, policy, convertibility, reserves and expectations, with distinct regimes (peg, managed float, market, commodity-backed). Currency ledgers always balance.
- Firms decide from their own information. National statistics are derived without double-counting intermediate goods. Informal and subsistence production count in material balances, and reported statistics can differ from true totals where measurement capacity is weak.
- Crises propagate through input-output links, inventories, transport, banks, debt, exchange obligations, employment, demand and legitimacy using the same ledgers, with no scripted crisis modifier.

### 9.5 Energy, climate and ecology

- Energy is tracked by carrier, source, storage, transmission and use, with conversion efficiencies and network losses. Plants have capacity factors, maintenance, water needs and fuel limits.
- Grids are connected graphs of generators, substations, lines, interconnectors, storage and demand. Disconnected regions cannot share generation. Failure probability depends on age, maintenance, load, hazards, design, competence, sabotage and redundancy, and cascades follow capacity violations and dependencies.
- Water uses directed catchment graphs: upstream inflow, runoff, groundwater exchange, withdrawals, reservoirs, evaporation, flood losses and outflow. Upstream use propagates downstream, volume and quality are separate, and groundwater can be depleted beyond recharge.
- Yield is a constrained product of crop genetics, soil, weather, water, nutrients, labor, technology and management, with crop thresholds and diminishing returns.
- Climate is hierarchical: global and large-region trends, with spatial cells for local temperature, rainfall, seasonality and hazard exposure. Monthly summaries plus event extremes are the planning target, and calibrated reduced-order models are acceptable.
- Pollution is tracked by pollutant, source, location and receiving environment with transport and decay. Greenhouse gases also change the planetary climate.
- Ecological stocks (forests, wildlife, fisheries, soils, biodiversity) have distribution, carrying capacity, regeneration, mortality, habitat dependency, extraction pressure and climate sensitivity. Extinction, salinization, aquifer exhaustion, ecosystem collapse and desertification can be irreversible, with thresholds, hysteresis and recovery delays.
- Disasters separate hazard, exposure and vulnerability, and are generated from conditions by seeded processes. Preparation alters consequences, not the natural hazard.
- Environmental interventions are projects with measurable inputs and delayed effects. No action gives a universal ecology bonus.
- Long-term loops are explicit and every link has a traceable variable, direction, update schedule and calibration case: climate to yields to food prices to migration to stability; energy to industry to emissions to warming; land conversion to habitat fragmentation to disease to health; water control to output to downstream shortage to diplomacy; pollution to health to productivity to output.

### 9.6 Knowledge, culture and religion

- Four concepts stay separate: scientific knowledge, technical knowledge (recipes, designs), adoption and mastery, extended into the full knowledge lifecycle in section 9.18. Knowing a technology does not create capacity.
- Discovery comes from a deterministic hazard model fed by population, specialization, education, institutions, energy, materials, existing knowledge, need, contact and experimentation. Calendar years alone guarantee nothing for isolated peoples.
- Research separates basic, applied and engineering work, limited by qualified people and infrastructure, not money alone. The technology graph holds hard prerequisites, alternative pathways, weighted enablers and infrastructure prerequisites.
- **Institutional research and scientific specialisation.** Research is an activity performed by people, scholarly communities and institutions, not an abstract resource generated directly by a civilisation.
  - Before formal institutions emerge, observation, experimentation, accumulated experience, oral traditions and craft practice produce knowledge. As societies develop, organised research increasingly becomes the work of persistent institutions.
  - Early research institutions are generalist and support several accessible knowledge categories: royal scholarly offices, temple schools, guilds, academies, monasteries or culturally equivalent bodies. They emerge from surplus, literacy, education, specialisation, patronage, administrative capacity and existing knowledge, not predetermined dates.
  - Unlocking a research category or subtree establishes scientific accessibility but not the institutional capacity to pursue its advanced discoveries. Technologies and subtrees may carry institutional capability prerequisites in addition to scientific, technical, material, infrastructure and other prerequisites.
  - Specialised capability can come from institutional reform, departmental formation, subsidiaries, independent institutions, mergers, acquisitions, joint institutions or contractual collaboration. Requirements refer to functional competence, never compulsory institution names or historically predetermined forms.
  - Generalist institutions may evolve into multidisciplinary academies, universities or research organisations with specialised departments. Other civilisations may develop decentralised ecosystems of independent institutions, corporate laboratories, state agencies, professional associations and international consortia. Each arrangement has different costs, efficiencies, coordination needs, vulnerabilities and political consequences.
  - Research output is differentiated by domain, subtree, institutional competence and activity (basic, applied, engineering). Capacity is not interchangeable between unrelated disciplines. Funding can be redirected, but new competence needs qualified personnel, institutional development, infrastructure, experience and time.
  - Institutions research according to their resources, capabilities, objectives, governance, autonomy, incentives and access to knowledge. Public, private, religious, commercial, military and transnational institutions pursue different priorities, and autonomous institutions can make discoveries independently of government programmes.
  - Institutional competence, scientific knowledge, technical knowledge, adoption and mastery stay distinct. A civilisation may know something without the institutions to advance it, and an institution may hold scientific competence without the industry to apply it.
  - Institutions retain organisational memory, scientific traditions, specialist networks, expertise, intellectual property and provenance. Reform, destruction, fragmentation, migration, persecution, merger and collapse can alter or remove research capability without destroying all underlying knowledge.
  - The relationship is recursive: discoveries enable new disciplines and organisational forms, and institutional development enables deeper discoveries. This feeds scientific revolutions, divergence, stagnation, loss, recovery and transformation.
  - Interdisciplinary research, parallel invention, collaboration, intellectual property, secrecy, scientific freedom, competition, brain drain, education, infrastructure and industrial adoption run through the existing knowledge, institution, character, economic, political and diplomatic systems.
  - The global research indicator is an aggregate measure of output or capacity, not a freely transferable research currency. Its domain-specific and institution-specific contributions stay inspectable.
  - The same institutional requirements, discovery rules and resource constraints apply to the player and to autonomous actors. Invention and diffusion still resolve in the turn-end knowledge step, with relevant phase events recorded through phase deltas. Mechanics K22 to K39 and I11 to I18.
- Diffusion runs through trade, migration, travel, education, diplomacy, media, communications, conquest and espionage, limited by isolation and absorptive capacity. Knowledge can spread without the assets needed to use it.
- Adoption needs skills, capital, resources, institutions and expected benefit. Mastery accumulates through practice and can be lost with skilled workers and institutions even when theory survives. Loss of theory and loss of practical competence are tracked separately.
- Culture is a weighted, evolving trait bundle that can diverge, hybridize, assimilate, preserve minorities, reconstitute older identities and acquire new traits, with ancestry links. Exposure influences affinity but does not guarantee conversion.
- Religion separates personal belief from institutions: belief distributions, doctrines, organizations, clerical influence, political authority, finances, cohesion, conversion, schism and diplomacy.
- Independent discoveries keep separate provenance and first-discovery claims. Each actor's knowledge can include accurate discoveries, wrong theories, misread evidence and fabrications, and the authoritative state never depends on beliefs.

### 9.7 Government, characters and succession

- Every actionable order has a legal authority requirement and an enforcement requirement. A ruler can attempt an illegal action, but it becomes an explicit political act with resistance and consequences.
- Administrative capacity = institutional competence x staffing x communication reach x resources x legitimacy - corruption and disruption. It falls with distance, delay and overload, and governors can gain autonomy.
- Government forms are classifications derived from institutional features (executive, legislature, judiciary, territory, succession, elections, civil-military relations, religious authority, centralization).
- Legitimacy is a derived multidimensional value, differs by population group, and is shown as an aggregate with visible constituents.
- Factions have members, interests, resources, organization, leadership, access, ideology, grievances, approval and mobilization capacity, and need not match social classes.
- Proposals pass through the required approval process, and rulers, ministers, legislatures, courts, parties and military leaders can authorize, modify, obstruct or reject them.
- Character tiers are confirmed: notables get full simulation, house members reduced detail, background families aggregated, with promotion and demotion. Characters are independent actors driven by personality, wealth, prestige, careers, loyalty, kinship, grievances, opportunity and risk.
- On a ruler's death, succession rules determine candidates, lawful authority passes or a dispute starts, inheritance and administrative effects apply, regency resolves where needed, and the player's leadership updates at the next permissible decision point.
- Institutions keep identities separate from officeholders, so a change of ruler or dynasty does not erase bureaucracy, laws, debts or obligations.
- Revolution needs grievance, organization, leadership, resources, support, security-force loyalty, government response, triggers and outside help. Outcomes are reform, repression, regime change, civil war, secession or failure. Fragmentation allocates territory, people, assets, liabilities, claims and administrative inheritance explicitly, and does not imply cultural extinction.
- Player control: the player leads the selected country's leading character, and available decisions depend on that character's powers and the institutions. Countries can be switched at phase boundaries. The abandoned country returns to AI control and keeps its resources, achievements and obligations. The player does not inherit previous-country intelligence as an authorized asset.

### 9.8 Diplomacy, intelligence and war

- Foreign-policy actors include sovereign states, confederations, imperial hierarchies, subjects, international organizations, religious institutions, corporations with legal status, project consortia and insurgent authorities, each with different treaty and military powers.
- Treaties list parties, signatories, obligations, rights, conditions, ratification, duration, renewal, withdrawal, enforcement and remedies. They can be secret, public, multilateral or conditional. Trust comes from remembered interactions and can differ by institution and leader.
- Intelligence estimates have ranges, confidence, recency and reliability, and can contradict each other. Espionage operations have preparation, cost, prerequisites, duration, success and detection risk, and consequences. Attribution is separate from detection, and exposure affects domestic politics, trade, trust and war justification.
- Military hierarchy: personnel, formation, army or fleet, operational command, theatre, strategic command, with era-specific forms. Forces need real population, recruitment, training, equipment, finance, supply and authorization, and mobilization removes labor from the economy.
- Movement uses the physical network and logistics (terrain, infrastructure, weather, formation, equipment, readiness, supply, hostile control, command delay) independent of turn-length abstractions. Supply consumes food, fuel, ammunition, maintenance and medical resources, and unsupplied forces lose effectiveness.
- Combat is deterministic: capability x readiness x supply x terrain x doctrine x command x morale, with seeded stochastic factors only if replay stays deterministic. Losses hit real demography, equipment and logistics.
- Orders take time to travel, units can act on outdated orders, and modern communications cut delay but add electronic vulnerability. Morale and cohesion depend on casualties, supply, leadership, fatigue, experience, ideology and training.
- Occupation is separate from battlefield control and needs security forces, cooperation and reach. Wars end through negotiation, surrender, ceasefire, collapse, withdrawal, political change, exhaustion or stalemate, with no universal warscore threshold. Postwar accounting covers casualties, veterans, refugees, damage, debts, transfers, political change and grievances, all reconciled with the ledgers.
- Military AI turns objectives into feasible operations from its own intelligence, and cannot ignore transport or industrial limits.

### 9.9 Engine, AI and performance

**Governing engineering principle: simulate the maximum meaningful complexity with the minimum necessary computation.** Detail is spent only where it changes outcomes the player or the simulation can feel: aggregates for ordinary populations, firms and researchers, full detail for notables and active crises, closed-form integration instead of fixed subticks, event generation only when due, and evaluation of active eligible records rather than every possible combination. Every system specification states how it meets this principle. In full: simulate everything that matters, compute only what is necessary, and represent complexity through mathematics rather than brute force. The Computational Doctrine tab is the binding engineering reference, and the Architectural Decision Registry records every engineering decision.

- A versioned, deterministic authoritative state holds time and events, geography, environment, entities, populations, assets, ledgers, networks, knowledge, institutions, characters, orders and historical references. Each authoritative variable has exactly one owning system. Others read approved snapshots or issue validated transactions.
- Storage is a hybrid: entity-component storage for dynamic actors, relational structures for identities, dense arrays for spatial and demographic data, sparse graphs for networks and relationships, event-sourced history, and immutable content definitions. The language is C# (ADR-001), and the simulation state uses no external database.
- Every mechanic declares inputs, outputs, authority, units, update period, preconditions, conservation constraints, events emitted, other systems affected and calibration needs. The dependency graph is built from these declarations.
- Precision classes: exact (population totals, ledgers, ownership shares, transfers, identifiers, event identities), fixed-point (prices, rates, probabilities, ecological stocks), statistical approximation (household behavior, ordinary firms, ecosystem groups), and forecast (never authoritative).
- Adaptive resolution: full detail for the controlled entity, important contacts, active crises, theatres, major market disruptions and notable actors. The player and contacts act every phase, other entities every two to four phases, and remote entities about once per turn. Quiet regions advance through integrated rates, and detail is reconstructed from aggregates when an event needs it.
- Events are generated only when due, with unimportant repetitive events aggregated. Enough deltas are kept to guarantee replay.
- AI uses hierarchical horizons (tactical, operational, strategic) that grow with competence, multi-objective utility (survival, security, prosperity, control, legitimacy, culture, ideology, prestige, ambition, continuity), and institutional coordination that can fail. AI receives only authorized observations.
- Parallel work uses disjoint cells, entities and network segments with deterministic aggregation, and multithreading never changes results.
- Targets: ordinary late-game turn about 2 s, worst case about 10 s, turn-end tick about 1 s, ordinary phase about 0.2 s, up to 10,000 turns. Reference hardware, interface targets and worst-case ceilings are in Computational Doctrine section 8. Provisional memory baseline: 15,000 land provinces, about 12 primary cohorts each, thousands of political organizations and notable characters. Byte budgets are defined before implementation, with no arbitrary entity caps that change outcomes.
- Saves hold schema and ruleset versions, content version, world seed, current state, ordered inputs, random-stream state, pending events, checkpoints and integrity hashes. Migrations are deterministic, tested and recorded.
- Systems talk through documented data contracts, content is separate from algorithms, and public modding stays out of scope.
- Automated invariants cover population, material, energy, money and debt, ownership totals, non-negative stocks, identifiers, territorial membership, referential integrity, event ordering, hashes and unauthorized mutation, with reproducible diagnostics.
- Validation uses historical cases as behavior checks (Sentinelese isolation, Tasmania, Meiji, North Korea, China after 1978, India, plus demographic transitions, monetary crises, industrial revolutions, ecological degradation and state fragmentation). Eight benchmark worlds cover prehistoric, ancient, medieval, industrializing, modern, late-game, global-war and cascade-crisis conditions.

### 9.10 History, causality and forecasting

- Every causally meaningful change enters an event history with periodic snapshots and compressed deltas, enough to reconstruct exact prior states. Events have permanent IDs linked to time, actors, places, triggers, inputs, decisions, state changes, consequences and prior events.
- Causes are labeled direct, contributing or background. Complex outcomes use a causal subgraph with decomposition or sensitivity analysis, and interactions are disclosed instead of assigning false independent percentages.
- Counterfactuals are either exact alternate-history reruns from a snapshot or labeled approximations, and are scenarios, not facts. Replay reproduces the original state hashes.
- Three records stay separate: the actual state, what historical actors believed, and what the player could observe. Uncertain values show best estimate, interval, observation date, source, assumptions and reliability, and unknowns are never shown as exact.
- Forecasts give central, optimistic and adverse scenarios and are recalculated when assumptions change.
- The chronicle ranks events by scale, duration, novelty, consequences and relevance. Generated explanations are checked against recorded dependencies, numbers, ledgers and versions, and never invent causes.

### 9.11 Interface

- Four levels: Level 1 Living Survey Map, Level 2 domain workspaces (28 in eight groups: World, Identity and Development; Population and Society; Government and Institutions; Economy and Finance; Technology and Industry; Culture and Religion; Diplomacy, Intelligence and War; Planetary Systems, History and Legacy), Level 3 dossiers, Level 4 analytical investigations. Navigation is contextual, not sequential.
- Navigation preserves selected entity, geographic focus, layers, historical date, open dossier, workspace, filters and comparison selection. One shared entity-selection service lets every workspace render its own view of the same entity.
- Dossiers share universal sections (identity, classification, location, ownership, lineage, condition, relationships, statistics, events, actions) plus domain modules.
- Map layers have semantic categories with visible priority when encodings conflict. Semantic zoom (global, regional, local, detailed) adds detail without inventing entities.
- Every statistic links to its authoritative variable, owner system, method, inputs, date, uncertainty and history, and opens an explanation. Statistics distinguish stock, annual rate, phase change, turn change, period value, forecast and estimate versus exact. The header shows date, turn, phase and date span.
- Actions have four states: inspect, plan, issue, resolve. Unavailable actions explain what blocks them and what would make them feasible. Alerts are critical, important or informational, and filters never hide authoritative failures.
- Permanent elements at Level 1: ruler and civilization header, global resources and status, turn and phase controls, primary navigation, selected-entity inspector, main visualization, minimap, bottom action bar.
- Institutional research uses the existing four levels. WS-E1 Technology Tree shows scientific and institutional prerequisites. WS-E2 Knowledge Diffusion and Innovation shows research programmes, competence, allocation, discovery progress, diffusion and research networks. WS-C1 Government and Administration shows institutional charters, formation, reform and governance. WS-E3 Materials and Industrial Engineering and WS-E4 Energy, Infrastructure and Utility Networks show the industrial, material, energy and infrastructure dependencies of research. Research organisations, departments where relevant, research infrastructure and programmes have dossiers. Bottlenecks, competition and historical research development use the existing analytical investigations. The global research indicator opens a breakdown of institutional and disciplinary contributions. No new interface level or primary navigation category is added.
- Accessibility: scalable text, high contrast, color-independent symbols, keyboard navigation, search, readable tables, tooltips, alternatives to complex charts and label density controls. Minimum supported screen is 1920 x 1080 for the full dense interface, with collapsing panels below it.
- Errors are explained to players, with developer diagnostics kept separate, and an invariant failure stops unsafe commits. The concept screenshots illustrate the look and are not locked formulas.

**Workspace registry and ID convention.** The 28 Level 2 workspaces come from the Interface Architecture Chart (v3.3, redrawn as v3.4 from the registry on 24 Nov 2026), the visual companion to the authoritative Interface Architecture Registry. A showcase view is a major functional view inside an existing workspace or dossier, not an extra workspace, so the count stays 28. WS-G3 Military Grand Command has four showcase views: Theatre Operations and Campaigns; Military Logistics and Supply; Armed Forces and Command; War Council and Peace Negotiations. Event dossiers include the Battle and Campaign Dossier, and Causal Analysis includes military causal analysis. Workspace IDs always carry the prefix WS- (for example WS-E1), because the group letters A, D and E are also mechanic series (A engine, D diplomacy, E economy). A bare ID such as E1 always means a mechanic, and WS-E1 always means a workspace. Group sizes vary from 2 to 6, totalling 28.

| ID | Group | Workspace |
| --- | --- | --- |
| WS-A1 | A. World, Identity and Development | World and Territorial Structure |
| WS-A2 | A. World, Identity and Development | Projects and Transformations |
| WS-A3 | A. World, Identity and Development | Civilisation Identity and Lineage |
| WS-B1 | B. Population and Society | Population and Migration |
| WS-B2 | B. Population and Society | Social Development |
| WS-C1 | C. Government and Institutions | Government and Administration |
| WS-C2 | C. Government and Institutions | Civics and State |
| WS-C3 | C. Government and Institutions | Law, Rights and Politics |
| WS-C4 | C. Government and Institutions | Leaders, Families and Dynasties |
| WS-D1 | D. Economy and Finance | National Accounts and Macroeconomics |
| WS-D2 | D. Economy and Finance | Treasury and Public Finance |
| WS-D3 | D. Economy and Finance | Currency and Monetary Hegemony |
| WS-D4 | D. Economy and Finance | Banking, Credit and Capital Markets |
| WS-D5 | D. Economy and Finance | Corporations and Monopolies |
| WS-D6 | D. Economy and Finance | Trade, Supply Chains and Logistics |
| WS-E1 | E. Technology and Industry | Technology Tree |
| WS-E2 | E. Technology and Industry | Research, Knowledge Diffusion and Innovation |
| WS-E3 | E. Technology and Industry | Materials and Industrial Engineering |
| WS-E4 | E. Technology and Industry | Energy, Infrastructure and Utility Networks |
| WS-E5 | E. Technology and Industry | Information, Communications and Media Systems |
| WS-F1 | F. Culture and Religion | Culture, Philosophy and Traditions |
| WS-F2 | F. Culture and Religion | Religion and Religious Institutions |
| WS-G1 | G. Diplomacy, Intelligence and War | Diplomacy and International Order |
| WS-G2 | G. Diplomacy, Intelligence and War | Intelligence and Espionage |
| WS-G3 | G. Diplomacy, Intelligence and War | Military Grand Command |
| WS-H1 | H. Planetary Systems, History and Legacy | Planetary Environment and Ecology |
| WS-H2 | H. Planetary Systems, History and Legacy | Historical Atlas and World Chronicle |
| WS-H3 | H. Planetary Systems, History and Legacy | Scoring and Civilisation Legacy |

**Interface ID convention (permanent).** Interface IDs are separate from mechanic IDs and never reused.

| Element | ID format | Example |
| --- | --- | --- |
| Interface level | LV1 to LV4 | LV2 |
| Category group | CG-A to CG-H | CG-C |
| Primary workspace | WS-\[A-H\]\[1-6\] | WS-E2 |
| Showcase view | WS-\[A-H\]\[1-6\]/V\[n\] | WS-C3/V1 |
| Dossier type | DS-01 to DS-07 | DS-03 |
| Analysis type | AN-01 to AN-06 | AN-04 |
| Global system | GS-01 to GS-05 | GS-05 |
| Analytical pattern | AP-01 to AP-06 | AP-03 |
| Contextual cross-link | XL-0001 onward | XL-0147 |

Interface levels use LV, not the chart's L1 to L4, because L1 to L10 are already the Living Constitutional Order mechanics.

**Registered showcase views (10).** WS-A2/V1 FTL Grand Project and Consortium (endgame); WS-C1/V1 Institutions and Charters (guilds, universities, companies, international bodies); WS-C3/V1 Constitutional Archive and Governance Ledger (Living Constitutional Order); WS-E2/V1 Scientific and Institutional Research Ecosystem (programmes, competence, allocation, networks); WS-G3/V1 Theatre Operations and Campaign Planning; WS-G3/V2 Military Logistics and Supply Network; WS-G3/V3 Armed Forces, Formations and Commands; WS-G3/V4 War Council and Peace Negotiations; WS-E2/V2 Global Knowledge Atlas and Intellectual Genealogy; WS-H2/V1 Living Historiography and Historical Library (added 24 Nov 2026).

**Dossiers (LV3):** DS-01 Entity, DS-02 Place, DS-03 Organization, DS-04 System, DS-05 Asset, DS-06 Event (includes the Battle and Campaign Dossier), DS-07 Knowledge Artifact (added 24 Nov 2026). DS-07 is justified because a work's content, claims, editions, translations, copies, access relationships, validation and lineage fit none of the six existing types; physical copies remain DS-05 assets. Constitutions, charters and other governing documents are knowledge artifacts (DS-07), shown inside the state or institution dossier (DS-03) as a linked module.

**Analytical investigations (LV4):** AN-01 Causal (including military causal analysis), AN-02 Comparative, AN-03 Counterfactual, AN-04 Historical Replay, AN-05 Trend and Forecast, AN-06 Structural.

**Global interface systems:** GS-01 World Creation and Starting Conditions; GS-02 Turn Resolution, Standing Orders and Exception Management; GS-03 Knowledge, Information Access and Uncertainty; GS-04 Universal Causal Inspector and Historical Replay; GS-05 Search and Entity Selection.

**Analytical patterns:** AP-01 Entity, AP-02 Relationship, AP-03 Process, AP-04 Comparative, AP-05 Causal, AP-06 Historical analysis.

**Player Career Record** lives in WS-A3.

**Main menu.** The 13 menu items in the HUD are fixed shortcuts into the groups, so every workspace is reachable and nothing is duplicated:

| Menu item | Workspaces |
| --- | --- |
| World | WS-A1 to WS-A3 |
| Court and Dynasty | WS-C4 |
| People and Society | WS-B1, WS-B2 |
| Government | WS-C1 to WS-C3 |
| Economy | WS-D1 to WS-D6 |
| Technology | WS-E1 to WS-E5 |
| Culture | WS-F1 |
| Religion | WS-F2 |
| Diplomacy | WS-G1, WS-G2 |
| Military | WS-G3 |
| Environment | WS-H1 |
| History | WS-H2 |
| Scoring | WS-H3 |

**Interface Architecture Registry (authoritative).** A machine-readable file in the repository that defines every interface element and ID, navigation rules, linked mechanics, available actions, access conditions, cross-links and validation tests. For data ownership it references the World State and Data Schema rather than defining it, so ownership has one source of truth. The chart is its visual companion.

**Knowledge and historiography in the interface (24 Nov 2026).** WS-E2 is the principal research and knowledge workspace: programmes, competence, allocation, networks, collections, artifacts, acquisition, access, custody, translation, absorption, comparative research, synthesis, Grand Intellectual Works, genealogy, publication, peer review, replication, consensus, dissemination, application and capability bottlenecks. WS-H2 is the principal historical workspace, with WS-H2/V1 holding the Living Historical Library, comparative historiography, archives and sources, competing accounts, revisions and grand syntheses alongside the Atlas and Chronicle. WS-C3/V1 and WS-C1/V1 carry constitutional knowledge and knowledge institutions; WS-E5 carries publication, information infrastructure, censorship and digital distribution; WS-E1, WS-E3 and WS-E4 show known, understood, absorbed, experimental, operational, deployed, dormant and lost capability with restoration requirements. Dossiers gain reusable modules (history, collections, programmes, artifacts, evidence, access, lineage, validation, absorptive capacity, operational capability). AN-01, AN-02, AN-04, AN-05 and AN-06 cover source credibility and independence, intellectual dependency, synthesis, diffusion, impact, absorption, consensus and capability regression. GS-03, GS-04 and GS-05 integrate access, causality and search. Counts after this change: 28 workspaces, 10 showcase views, 7 dossier types, 6 analysis types, 5 global systems, 6 analytical patterns. The machine-readable registry now exists as draft 0.1.0 (interface\_registry.yaml, 24 Nov 2026) and enters the repository with Pass 2. Chart v3.4 is generated from it and includes the two new showcase views, DS-07, the extended WS-E2, WS-G2, WS-E5 and WS-H2 entries, the F2 label fix and the separated WS-E4 and WS-E5 contents.

### 9.12 Mandatory conservation and determinism

- People change only through births, deaths, migration, conversion, assimilation, transfer and mobility, with casualties posted to one population ledger.
- Materials move through extraction, processing, inventories, trade, consumption, recycling and waste. Transfers never raise global quantities.
- Energy, water and money balance under their modeled units. Every financial asset has a matching source or liability, and credit creates matching claims.
- No territory has contradictory exclusive control without an explicit contested-control structure. Events cannot precede their prerequisites, and expired entities keep valid historical references.
- Identical conditions, rules, seeds and decisions give identical authoritative states. Parallelism, UI navigation, zoom, logging and rendering never change results.

### 9.13 Reconciliations adopted

- Cohort groups hold age distributions inside primary groups. Long phases hold timestamped events with adaptive local integration. Immediate stocks stay authoritative while the turn-end tick integrates structure. Continental movement is strategic pacing, not literal travel time. One city per region stays, with towns, metropolitan areas and regional subdivision. A region has a dominant biome with finer local variation. History is events plus snapshots and deterministic deltas. The twelve-group target is preserved with conditional distributions and sparse exceptions. AI follows identical rules and differs in competence only. FTL is strongly encouraged but needs an achievable project. Screenshot numbers are illustrations. Province benchmark: 15,000 with a 20,000 envelope. Territory score uses stable geographic equivalents. Full replay uses versioned snapshots, events and deterministic simulation.

### 9.15 Living Constitutional Order and Civilisational Legal Memory

A government is the continuously evolving result of institutions, laws, distribution of power, political actors, social conditions and accumulated decisions. It is never selected from a list, and there is no Become Authoritarian button. The model is multidimensional, continuous and allowed to be contradictory: one country can hold an elected legislature, a hereditary head of state, a powerful military, an independent judiciary, public ownership, private monopolies and tight limits on dissent at once. Named forms (9.7) are only descriptive labels.

- **Governance Ledger.** Every civilisation keeps a persistent record across dynasties and thousands of turns: executive acts (decrees, appointments, emergency powers, pardons), legislation (enacted, repealed, amended, suspended, enforced), judicial precedents, political practices (elections, succession customs, consultation, patronage, censorship, coercion), social contracts (property, religious freedom, labour, citizenship), institutional behaviour (military intervention, bureaucratic and judicial independence, legislative resistance) and crises (civil wars, constitutional disputes, revolutions, contested successions, emergency rule).
- **Claim, law and practice stay separate.** The ledger records what a government claims to be, what its laws prescribe and how it actually behaves. A state can formally protect expression while repeatedly suppressing dissent, and both facts stay visible.
- **Five layers.** Written law, institutional interpretation, actual enforcement, public acceptance and historical precedent influence each other without being identical. A document can survive for centuries while its meaning and force change.
- **Erosion without rupture.** Formal constitutional continuity does not imply substantive continuity. Elections can lose competitiveness, courts independence and legislatures their real powers while the documents survive. Germany in 1933 (appointment within the constitution, the Reichstag Fire Decree, the Enabling Act) is a validation case: the system must reproduce such sequences from ordinary mechanics.
- **Institutional memory and inertia.** Weight of a precedent grows with repetition, duration, enforcement, institutional adoption, legitimacy, resistance overcome and historical significance. A decree repealed at once fades. The same decree renewed and upheld for two centuries becomes an entrenched convention.
- **Codification.** At certain moments a civilisation may codify its accumulated order through a constitutional convention, religious council, revolutionary assembly, imperial decree, parliamentary settlement, judicial consolidation or negotiated peace. Documentary forms, available only where the institutional history supports them: Constitution, Imperial Charter, Sacred Legal Canon, Revolutionary Manifesto, Foundational Covenant, Customary Law Compendium, Party Charter. Several can coexist with conflicting jurisdictions, and forms can blend (a constitution incorporating sacred law, a charter guaranteeing parliamentary rights, a manifesto hardening into a constitution).
- **Documents as artifacts.** Every major governing instrument is a readable in-game document built from simulated history. Each substantive article traces to the decisions, institutions, precedents and settlements that produced it, and corresponds to an operative simulation rule. Players can inspect the original text and amendments, the events behind each article, which factions backed or opposed it, how institutions interpret and enforce it, whether the government currently complies, and earlier versions, repealed provisions and disputed interpretations.
- **Codification is an event chain.** Disputes over prerogatives, representation, courts, taxation, military command, religious jurisdiction and provincial sovereignty. The player can negotiate articles individually, trade concessions between factions, accept a flawed compromise, impose a settlement or abandon codification. Every outcome becomes a future precedent.
- **Constitutions can fail.** Provisions can be exploited (a sovereign repeatedly dissolving an elected legislature), emergencies extended indefinitely, and practice transformed while the text survives.
- **Rupture and succession.** Revolution, conquest, dynastic collapse, reformation and fragmentation can invalidate or reinterpret parts of the inherited order without erasing them. Successor states can inherit the same founding document and interpret it differently.
- **Legal validity and revolution.** Government change, revolution and regime transformation follow Hans Kelsen's Pure Theory of Law: every legal norm derives validity from a higher norm up to a basic norm (Grundnorm), and a revolution is legally successful when the new order becomes effective by and large, which changes the basic norm. The simulation therefore tracks legal validity chains and effectiveness separately.
- **Integration.** The constitutional archive, ledger, conventions and interpretations are views inside the existing Government, Court and Dynasty, People and Society, Religion, Culture, Diplomacy, History and World Chronicle areas, with no separate government-type screen.

### 9.16 Autonomous institutions, corporations and transnational power

- **Who.** Merchant guilds, trading companies, chartered corporations, joint-stock and listed companies, private and state-owned enterprises, academies, research associations, universities, conglomerates, professional federations, trade leagues, banks, financial syndicates, multinationals and international organisations.
- **What each has.** Identity, founding history, legal status, governance structure, leadership, members or shareholders, assets, capital, revenue, liabilities, objectives, political influence, technological capability, and potentially territorial jurisdiction, diplomatic relations and armed forces.
- **Lifecycle.** Formation, expansion, consolidation, fragmentation, merger, acquisition, nationalisation, privatisation, listing, dissolution and transformation emerge from economic, political, technological, legal and historical development, never from scripts. A chartered trading company can grow into a transcontinental power with monopoly privileges, private armies and fleets, territorial administration, taxation, treaties and expeditions, holding effective sovereignty without becoming a state (the British and Portuguese East India companies are validation cases). A private firm can be nationalised, turned into a monopoly, privatised, broken up or become a multinational.
- **Science and technology bodies.** Universities, consortia, private labs, corporate R&D and international collaborations originate discoveries, own intellectual property, set standards and steer industry. In advanced eras private actors can match or exceed governments in research, computing, AI systems, finance and strategic technology.
- **Research institution structure.** Research institutions have evolving internal structures: research mandates, departments, laboratories, specialist programmes, subsidiaries and collaborative networks, which develop with scientific complexity, objectives, opportunities and resources. They may stay generalist, specialise, diversify, fragment, consolidate or join joint programmes. Reforms and founding charters set research authority, governance, funding obligations, intellectual property and autonomy, and enter the Governance Ledger where applicable. Internal departments are not independent legal actors. Subsidiaries and separately chartered organisations are, where their legal and governance arrangements establish independence.
- **Separate dimensions.** Ownership, legal authority, operational control, political influence, territorial administration and effective sovereignty are simulated independently, so institutions can operate across borders, cooperate with or oppose states, capture regulators, fund movements, dominate supply chains, wield coercion or become indispensable.
- **Internal governance evolves.** Hereditary ownership, merchant partnerships, shareholder assemblies, boards, cooperatives, state appointment, institutional councils and hybrids.
- **Real actors.** They are persistent, independently deciding participants with institutional memory and competing interests, never abstract modifiers.
- **Legal memory.** Charters, monopolies, institutional autonomy, nationalisation precedents and public-private conflicts feed the Governance Ledger and become part of a civilisation's legal identity.

### 9.17 Warfare model

**Three nested scales.**

- Strategic command: war aims, mobilisation, military budgets, alliance commitments, industrial priorities, theatre allocation, escalation policy, peace negotiation.
- Operational command: offensives, defensive lines, movements, supply corridors, reserves, manoeuvre, sieges, amphibious landings, campaign objectives.
- Tactical execution: deployment, frontage, engagement, weapon use, local manoeuvre, casualties, retreat, surrender, battlefield control. The player normally decides strategic and operational matters. Tactical engagements resolve through simulated commanders, doctrine, formations, equipment, terrain and orders, and are detailed and inspectable, not a real-time battle game.

**Command delegation.** Direct (the player sets routes, objectives, formations and reserves, subject to command delay and institutional limits), Guided (the player sets objectives and constraints, commanders submit plans for approval), Autonomous (commanders execute a broad mandate and report exceptions, risks and major decisions). Delegation is never an efficiency bonus. It changes who decides, what the player knows, how long authorisation takes and the risk of independent commander behaviour. How directly a ruler can command depends on era and institutions: an early ruler can lead an army personally, a modern head of government works through a command structure.

**Core rules.**

- **Military manpower is demographic.** Soldiers come from the population by age, health, occupation, class, culture, law, recruitment practice and institutional capacity. Volunteers, conscripts, levies, professionals, reservists, mercenaries, militias and irregulars are distinct. Recruitment costs the civilian economy, casualties reshape future demography, and veterans return with skills, disabilities, relationships, influence and grievances. An army is a temporary reorganisation of society.
- **Equipment comes from the production chains.** Formations hold equipment inventories with quantity, condition, maintenance, compatibility and replacement needs, supplied through natural, refined and G1 to G4 chains. Knowing a technology never grants mass-produced equipment. This creates arms export markets, dependencies, embargoes and captured-equipment problems.
- **Logistics is physical.** Supply moves on the same roads, railways, rivers, ports, warehouses and factories civilian society uses, with consequences for the economy, society and public opinion. Supply forecasts, reserve consumption, delivery reliability, stockpile locations, spare parts and repair turnaround are tracked. A cut-off formation fights on until its actual inventories run out, with no arbitrary collapse threshold.
- **Operational geography.** Terrain sets approach corridors, frontage capacity, defensive depth, chokepoints and crossing capacity, visibility, fortification coverage and retreat routes, derived from the map without hand-built battle maps.
- **Campaign planning.** Objectives, boundaries, timing, formations, reserves, logistics and fallback conditions, evaluated for feasibility on available information. Plans fail for visible, explainable causes, never hidden modifiers.
- **Command friction.** Command span, staff capacity, cohesion, initiative and communications reliability. Commanders can misunderstand, delay, retreat early or refuse orders they judge unlawful or suicidal, driven by the character, institution, communication and loyalty systems, not dice.
- **Combat is a process.** Contact and detection, deployment and engagement conditions, effective participation, attrition and equipment damage, morale and cohesion change, then withdrawal, pursuit, surrender or continued fighting. Battles last hours to longer depending on era and scale. Losses distinguish killed, wounded, captured, missing, deserted and temporarily unavailable where data supports it.
- **War termination is political.** Each side has objectives, acceptable settlements, military expectations, economic endurance, alliance obligations and domestic constraints. Winning battles, controlling territory and governing people are three different achievements. Occupation depends on security, administrative capacity, local institutions, legitimacy, economic continuity and attitudes. Resistance has real networks, resources, recruitment, organisation and territorial access, never a percentage that spawns units.
- **War and politics.** Victory can strengthen a ruler while empowering an ambitious general. Long campaigns can enrich suppliers, impoverish farmers, radicalise workers, destabilise a monarchy or trigger an election crisis. Military factions, legislatures, houses, corporations and religious institutions take distinct positions.
- **Domains emerge from systems.** Naval war from shipping, ports, shipbuilding, trade, navigation and maritime geography. Air war from aviation, fuel, maintenance, airfields, sensors and communications. Orbital capability stays Earth-facing (satellites and their services). Cyber and information war target real networks, institutions, communications and public information.
- **Wars persist.** Casualties, demographic change, refugees, veterans, debts, destroyed infrastructure, settlements, reforms and grievances remain as a causal chapter of history.

**Non-negotiable principles.** No artificial military resources: manpower, equipment, food, fuel, money, infrastructure and information come from their authoritative systems, and readiness is derived. No omniscient command: player, commanders and AI act on legitimately obtainable, possibly deceptive information. No universal victory formula: occupation, losses, political collapse, exhaustion, concessions and war termination stay distinct processes.

**Military time.** Hierarchical and event-driven: the strategic phase is the player's decision boundary, while operational events (movement, supply, orders), engagement events (contact, combat, withdrawal) and consequences (casualties, inventories, control) resolve at their own timestamps inside it, so a battle never lasts a whole five-year turn. Ongoing campaigns advance under standing orders until a strategically significant event needs authorisation or a decision (conditional military interruptions). The exact interruption rules are still to be specified and tested.

**Interface.** Level 1 survey map shows armies, fleets, theatres, borders, supply corridors, fortifications, operations and military layers. Level 2 lives in WS-G3 Military Grand Command through four showcase views: Theatre Operations and Campaigns (operational planning), Military Logistics and Supply, Armed Forces and Command (administration, mobilisation, doctrine, delegation mode), and War Council and Peace Negotiations. Intelligence stays in WS-G2, military industry in WS-E3 and war finance in WS-D2, reached by contextual links. Level 3 dossiers: formations, commanders, campaigns, battles, military institutions, forts, equipment classes, military projects, treaties. Level 4: campaign causal analysis, force comparison, logistics bottlenecks, counterfactual operations, forecasts, battle reconstruction, replay.

### 9.18 Living Knowledge Order and Living Historiography

Owner-confirmed (24 Nov 2026). The simulation develops civilisation's understanding of itself and the world, not only civilisation. It extends the Scientific and Institutional Research Ecosystem (K22 to K39, I11 to I18), the Living Constitutional Order, information access (GS-03) and history (X10 to X15) without duplicating them. Mechanics K40 to K62, X16 to X27, A14 to A15 and U11. Mathematics, numbers and algorithms remain open for engineering evaluation and calibration.

**Knowledge lifecycle.** Fourteen distinct states and processes: existence, exposure, accessibility, acquisition, preservation, comprehension, validation, absorption, synthesis, application, operational capability, deployment, adoption and diffusion. They are not a rigid linear sequence: knowledge can be applied before it is understood, validated after use, revised by new evidence, and spread across many institutions and civilisations. Not every artifact passes through every stage.

**Eleven layers with separate ownership.** Objective world state and history; observations and evidence; recorded knowledge; actor knowledge and interpretation; research; published artifacts; circulation and access; absorption and capability formation; synthesis; application and deployment; intellectual evolution. Literature, beliefs and theories never modify objective facts, and presentation never owns state.

**Research capacity versus knowledge products.** Research capacity (people, competence, funding, infrastructure, time) is distinct from identifiable products: observations, evidence, measurements, methods, theories, discoveries, designs, datasets, negative findings, failed hypotheses, replications, reconstructions, syntheses, publications and process knowledge. Research points remain a measure of effort, never a substitute for products. Failed research still produces knowledge.

**Knowledge artifacts.** One generalised, sparse representation for every class (oral traditions to datasets and software), with permanent identity, provenance, claims, methods, uncertainty, validation, language, lineage, rights, secrecy and reception where meaningful. Significant works are individual; ordinary production is aggregated. Contents can be held as typed claims with relationships (supports, contradicts, replicates, revises, supersedes, translates, summarises, extends, derives from, corroborates, depends on, applies, refutes), and factual disagreements are kept distinct from disagreements over values. Unresolved conflicts persist.

**Content, copies and rights.** Intellectual content is separate from its copies, editions, translations, revisions and locations. Copying does not deprive the holder, while moving a unique copy can. Content is non-rivalrous; media, researchers, infrastructure and funding stay under normal accounting. One work in a million copies is still one work. Custody, ownership, rights, awareness, practical access, legal access, comprehension, evaluation, reproduction, application, redistribution and derivation are distinct, at the level of individuals, institutions, organisations, governments, population groups, civilisations and international bodies. Formal intellectual property only exists where legal systems create it.

**Acquisition and transmission.** Acquisition runs through existing systems: purchase, procurement, gifts, patronage, publication, treaties, exchanges, collaboration, licensing, translation, copying, printing, diplomacy, mergers, succession, scholar migration, rediscovery, archaeology, espionage, unauthorised copying, wartime seizure, colonial extraction, restitution and digital exchange. The method sets cost, legality, diplomatic effect, secrecy, provenance, access, delay and risk. Transmission depends on each civilisation's real media (oral, inscription, manuscript, messengers, archives, printing, journals, post, telegraph, broadcast, databases, the internet and beyond), each with latency, reach, bandwidth, cost, infrastructure, energy, reliability, censorship, language and security, never on nominal eras. Old media can coexist with new ones.

**Open networks.** A work published openly on a network is potentially accessible to every actor with the connectivity, permissions and capability, without bilateral treaties, limited by censorship, paywalls, language, literacy, search, computing, energy and attention. Access never implies awareness, comprehension, validation, absorption, adoption, consensus or acceptance. The internet changes the economics, latency and reach of transmission, not competence. Access is resolved through shared publication scopes and lazy queries, never a record per actor per work.

**Absorption and forms of knowledge.** Absorptive capacity is discipline-specific, built from education, language, prior expertise, staff, mentoring, infrastructure, funding, time and freedom of inquiry; it takes time, persists after artifacts are lost, and decays with turnover, collapse and disuse. Codified, tacit, institutional, embodied, distributed, operational and dormant knowledge are related but distinct. Documentation alone never confers manufacturing; machinery never confers the ability to reproduce it. Operating, reproducing and improving are separate.

**Infrastructure and capability.** Information, archival, educational, scientific, institutional, computational, financial, industrial, energy, logistical, legal and social capacities are drawn from existing systems, and no single technology level stands in for them. A technology can be scientifically known, understood, experimentally demonstrated, prototype-operational, reproducible at limited scale, commercially feasible, deployed industrially, widely adopted, temporarily unavailable, obsolete, or historically known but no longer reproducible, differently by region, institution and industry. Deployment consumes real capital, labour, energy and materials, with no unlock bonus bypassing the production chains. Capability regresses when its dependencies decay, while documentation can survive; preserved knowledge lowers restoration cost but does not restore capability by itself.

**Institutions, synthesis and validation.** Libraries, archives, universities, academies, societies, institutes, religious bodies, museums, observatories, ministries, corporate laboratories and digital repositories hold real collections (owned, borrowed, licensed, shared, restricted, untranslated, damaged, internal). Research capability depends partly on what an institution can access, understand and absorb. Institutions act autonomously within mandates and budgets, under delegation and exception management. Knowledge synthesis is a universal, recursive research capability producing new attributable artifacts; feasibility and quality depend on coverage, independence, reliability and competence (hard prerequisites separate from continuous quality), never on a count of books. Source independence prevents copies, translations, summaries and circular citation from counting as new evidence. Researchers never see hidden equations: theories form from accessible evidence in bounded hypothesis spaces and can be correct, approximate, incomplete or wrong. Peer review and validation are historically situated and fallible; replication is distinct from acceptance; consensus emerges from evidence and differs from political, public, religious and commercial acceptance.

**Grand Intellectual Works.** Exceptionally comprehensive or consequential works (a comprehensive history of the known world, a geological survey of the planet, a comparative treatise on constitutional orders, and so on) emerge from real capability, are never unique scripted wonders, can compete, be superseded or rise in importance later, and are never objective truth. Benefits come from the knowledge actually embodied and used through authorised systems, never from a title or a generic permanent modifier: geology does not create deposits, a constitutional treatise does not guarantee stability, medicine does not create medicines. First movers gain prestige, talent and influence, but benefits diffuse, saturate and obsolesce; intellectual leadership can emerge without territorial or military hegemony; knowledge agreements are distinct from training or industrial transfer; an extinct civilisation's work can shape the world for millennia.

**Living Historiography.** Five layers: objective world history, observed history, recorded history, interpreted history and published historiography. Any meaningful entity, place, system or subject can have readable history, including extinct civilisations, dissolved institutions and former provinces, with contextual significance. History follows permanent geographic and entity identities across division, merger, transfer and renaming. Memory develops from oral tradition and myth through inscriptions, archives and printing to scholarship, archaeology and digital archives, only as capabilities emerge. Authors (court chroniclers, clergy, universities, corporations, foreign observers, independent scholars) write from situated perspectives with grounded bias (emphasis, omission, framing, competing causes), and different civilisations can give materially different accounts of the same event. Works carry publication dates and preserved editions, never contain knowledge later than their date, and distinguish when an event happened, was observed, recorded, accessible, acquired, claimed, published and re-edited. Evidence can be lost and recovered, archaeology derives only from real surviving evidence, natural history is independent of political ownership, and historical memory affects legitimacy, identity, claims and grievances only through authorised systems. Historical literature keeps four things distinct: what actually occurred, what contemporaries believed had occurred, what later scholars concluded, and the consequences that followed from those beliefs, including consequences of beliefs later shown to be false.

**Reading, rendering and export.** The reader produces long-form, professionally structured history (volumes, chapters, periodisation from real structural change, maps, genealogies, footnotes, references, reading levels) and disciplinary formats for science (abstract, methods, findings, limitations, peer-review and replication records), rendered on demand and cached. A deterministic, offline baseline renderer is mandatory. Optional local or external language models are replaceable in-house rendering components that never own state, resolve events, invent facts, discoveries, causes, bonuses or actor knowledge; their claims are validated against evidence, with fallback to the baseline. The game stays fully playable without internet, subscriptions or model downloads. Works export in open formats without DRM, respecting information access.

**Access modes.** Normal player access; authorised entity perspective; debug tag-switch mode (the selected entity's own knowledge, never omniscience); omniscient developer mode, kept separate so hidden information never leaks into actor knowledge.

### 9.19 Needs calibration or testing before numbers are final

- Historical calibration of fertility, mortality, migration, carrying capacity, yields, assimilation, legitimacy weights and diffusion parameters.
- The 10,000 BCE spatial datasets (a separate data-source audit).
- Solver parameters: tolerances, substep triggers, market damping, disease resolution, grid fidelity.
- Economic detail: commodity coverage, accounting complexity, firm density, financial granularity.
- AI planning depth, institutional decision complexity, utility weights and forecasting costs.
- Cross-era scoring and comparison definitions.
- Performance targets and memory budgets on benchmark hardware.
- Explicit fictional assumptions for the FTL endgame, including the size and time profile of the FTL horizon value (X28).

## 10. Open questions

**Added 24 Nov 2026, now resolved:**

- Resolved 24 Nov 2026: no modding, plugins or extension API; only the source code is published; every requested capability is built in-house (ADR-032).
- Resolved 24 Nov 2026: dates display as BCE and CE.

1. Which paleo-climate, terrain and demographic datasets provide the 10,000 BCE starting map and default peoples? (Data phase.)
2. Multiplayer (shared cradles, optional timers, AI regents): deferred until single-player is designed.
3. Is abstract off-world supply in scope? Space is currently Earth-facing only.
4. Should additional real agricultural cradles emerge organically?
5. Exact rules for conditional military interruptions in long early-era phases (to be specified and tested).
6. How precedent weight and constitutional entrenchment are computed, and the dimensions of the multidimensional political model.
7. Wording check: Kelsen's Pure Theory of Law is legal positivism rather than natural-law theory. The docs apply Kelsen's basic-norm and effectiveness approach. Confirm that is the intent.

## 11. Next steps

1. Dependency-layer pass: assign every mechanic to a layer and build the interaction matrix.
2. Per-system specifications with formulas and numbers, starting with the most coupled systems (climate, power grid, population groups, trade nodes, legitimacy).
3. Identify the real-world datasets for the 10,000 BCE Earth and default peoples.
