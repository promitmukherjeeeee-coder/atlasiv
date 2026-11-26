# Mechanics Brainstorm v1

Nov 22, 2026 · @Promit

The complete mechanics set for the grand 4X game, grouped by domain. Each entry has an ID, a one-line description and its key variables. This pass is for completeness. The next pass sorts every entry into dependency layers and builds the interaction matrix. Resources, characters, intrigue and warfare are deepened in the Deep Dives tab. The layer tags on each heading (L0 to L5) are superseded by the authoritative dependency layers in section P.

## A. Time and turns (L0)

- **T1 Variable turn length.** Stepped lengths from 50 years down to 1 month. All rates are stored per year, and the world's tempo follows the leading edge. Each step has a phase count (20, 10, 10, 5, 4, 4, 3 and 5 phases per turn).
- **T2 Standing orders.** Multi-turn orders (migrations, builds, trade routes) progress per phase, carry across turns, and raise alerts only on exceptions. Development orders add priorities, spending limits, expiry and triggers (A17).
- **T3 Seasonal aggregation.** Long turns still run harvest, flood and migration cycles inside the turn, so annual rhythms matter even at 50-year turns. Precomputed annual profiles per biome drive them.
- **T4 Event queue.** Scheduled and conditional events resolve in a fixed order each phase.
- **T5 Calendar.** BCE/CE dating (owner decision 24 Nov 2026), era names, cultural calendars.
- **T6 History log.** Every state change is logged. The log feeds scoring, the chronicle and replays.

* **T7 Country switching.** The player can switch to any country at a phase boundary (Europa Universalis tag style). The country left returns to AI control, and each entity keeps its own history and score lineage.

- **T8 Action phases.** Each turn is divided into phases, which are the tick for fast systems and for the player's decisions. The phase count scales with the turn step so each decision covers a comparable stretch of felt history.
- **T9 Phase resolution order and phase deltas.** Planning, then resolution in a fixed order (standing orders and events, diplomacy, covert operations, movement and migration, combat, sieges and naval action, character events, unrest). Phase events that touch slow systems are recorded as phase deltas and consumed by the turn-end tick.
- **T10 Advance until and exceptions.** Run phases and turns automatically until a trigger needs a decision (war declared, army enters territory, succession crisis, first contact, disaster, plot discovered, treaty offer, project milestone, famine, new technology). Quiet phases auto-skip.
- **T11 Tempo trigger.** The leading edge is the fastest transport capability held by any entity with at least 0.5% of world population. The step switches when its candidate differs for 10 consecutive turns (faster) or 20 (slower), with at least 20 turns between switches. Each switch is an announced era-transition event (K9), and steps can slow again after a global collapse.

## B. World simulation (L1)

**Land and water**

- **W1 Height field.** A fine height map, collapsed into four gameplay bands (lowland, plateau, highland, mountain).
- **W2 Tectonics.** Slow uplift, volcanoes, earthquake zones.
- **W3 Hydrology.** Flow accumulation makes rivers. Rivers carve valleys, flood, shift course, form deltas, lakes and marshes. Dams and canals change flow.
- **W4 Soil and erosion.** Fertility, depletion, salinization from irrigation, erosion after deforestation.
- **W5 Sea level and ice.** Eustatic rise and fall, glaciers and ice sheets, coastal flooding.

**Climate and life**

- **W6 Climate model.** Temperature, precipitation, seasonality, monsoons, ocean currents, multi-year oscillations, solar and volcanic cycles.
- **W7 Greenhouse feedback.** Emissions, carbon cycle, albedo, permafrost thaw, warming.
- **W8 Biome dynamics.** Biomes convert when climate thresholds hold for long enough, with lag. Examples: desertification, forest advance, tundra thaw.
- **W9 Vegetation and fire.** Succession, wildfire regimes, grazing pressure.
- **W10 Disasters.** Floods, droughts, quakes, eruptions, storms, wildfires, tsunamis, pandemics, rare impacts. Probability follows geography, and tech and infrastructure mitigate.
- **W11 Deposits.** Mineral families and mining clusters (one site, several resources) with discovery by prospecting and tech.

**Animals and exploitation**

- **W12 Animal populations.** Per-species density, carrying capacity, growth and decline, predator-prey links, migration routes.
- **W13 Extinction and extirpation.** Overhunting, habitat loss, invasive species. Extinct species stay gone.
- **W14 Animal kingdoms and biocycles.** Herbivore, predator, scavenger and marine guilds with seasonal and boom-bust cycles.
- **W15 Natural exploitation.** Hunting, fishing, foraging, logging, quarrying, each with a sustainable yield and a collapse threshold.
- **W16 Disease ecology.** Zoonoses, vectors, epidemics linked to density, trade and climate.
- **W17 Domestication.** Plants and animals domesticated by region (real cradle packages), which sets farming and herding options.

## C. Population and settlement (L2)

- **P1 Population groups.** Integer counts keyed by culture, religion, class and age band, at most about a dozen per province.
- **P2 Demography.** Births, deaths, aging, life expectancy, with famine, epidemic and war losses and a demographic transition with development.
- **P3 Carrying capacity.** Food, water, tech and land use set the ceiling.
- **P4 Migration.** Push and pull factors, seasonal rounds for bands, refugees, mass migrations.
- **P5 Settlement formation.** Camp, village, town, city, driven by surplus, storage, defense and water. One city per administrative region; a second city needs lawful reorganisation (P32, ADR-045), and subdivision alone never creates city capacity (exploit test, revised 25 and 26 Nov 2026).
- **P6 Settlement costs.** City and town costs rise steeply, derived mainly from land, infrastructure, labour, administration and resource constraints; any remaining geometric factor is a labelled, justified gameplay abstraction (revised 25 Nov 2026).
- **P7 Districts and buildings.** Unique districts, buildings and upgrades for each settlement type. Extended by P35 and R32 (26 Nov 2026): districts are repeatable, mixed-use geographic units, and building capacity is a derived summary, not a slot ledger.
- **P8 Adjacency.** Fixed, typed adjacency bonuses between contiguous neighbours (owner-confirmed 26 Nov 2026, superseding the 25 Nov wording). A district, improvement or site earns one bonus of each type for which it has a qualifying neighbour (settlement, district, building, landmark or natural feature), so one district can hold several types. Each type has a fixed value; it counts once (non-stacking), types add, and no bonus multiplies another (non-compounding). The original +3 city and +1 town are the settlement type; the type catalogue grows with city planning and building families (P35, R32). A bonus lasts while its neighbour exists and operates. Network and distance benefits are separate (R33). City-level services can reach the whole region where access allows.
- **P9 Infrastructure.** Roads, bridges, rail, canals, ports, grids, pipelines, networks. Speeds feed back into the tempo. Network capacity is reserved and allocated for development (R33, I20).
- **P10 Land use.** Farmland, pasture, forest, urban footprint and their competition.
- **P11 Health and education.** Sanitation, medicine, literacy, schooling.
- **P12 Class and occupation.** Specialization grows with surplus.
- **P13 Language and writing.** Language families, drift, prestige, scripts and literacy.

## D. Resources and production (L2)

- **R1 Raw resources.** Stone, wood, ores, fuels and minerals from deposit clusters.
- **R2 Food.** Crops, livestock and fisheries by real regional package.
- **R3 Processing chains.** Smelting, refining, chemistry. Discovering an ore makes its research path accessible; processing it still needs operational capability (K50) (bauxite, then aluminium smelting).
- **R4 Man-made resources.** Four grades, from basic to advanced (for example transistors, SoCs). Grades follow purity and process complexity.
- **R5 Energy.** Muscle, biomass, water, wind, coal, oil, gas, nuclear, solar, fusion, with grids and storage.
- **R6 Nuclear materials.** Uranium, thorium, plutonium and polonium from shared deposits, convertible by tech or breeder programs.
- **R7 Logistics and stockpiles.** Transport capacity, spoilage, storage.
- **R8 Construction.** Cost, labor and time, scaled to turn length. Replaced in detail by the shared lifecycle R31 (26 Nov 2026).
- **R9 Pollution and waste.** Local damage and global emissions.

## E. Knowledge (L3)

- **K1 Branching tech tree.** Prerequisite graph with path styles (normal, cyber, solar, nature, steampunk). Early choices shape later options. A technology node is never a Boolean unlock: it links knowledge artifacts (K40) to per-actor lifecycle and capability states (K44, K50) that can differ by region and institution and can regress.
- **K2 Independent invention.** Depends on population, connectivity, surplus, specialization, literacy, need and prerequisites.
- **K3 Diffusion.** Channels (trade, migration, media, students, espionage, conquest, diplomacy) with permeability. Catch-up rises as more of the world holds a tech.
- **K4 Absorptive capacity and loss.** Using a tech needs prerequisites and infrastructure. Small or collapsing populations lose techs.
- **K5 Research institutions.** Schools, libraries, universities, labs, corporate R&D, scholars. Expanded by institutional research in K22 to K39 and I11 to I18.
- **K6 Civic trees.** Governance civics, culture civics, religion civics.
- **K7 First-mover rents.** Patents, monopolies and royalties that fade as a tech spreads.
- **K8 Espionage and theft.** Intel, sabotage and technology transfer by theft.
- **K9 Era transitions.** Tempo steps triggered by transport and communication tech.
- **K10 Lost knowledge.** Burned libraries, forgotten crafts, rediscovery.

## F. Economy and finance (L2 to L5)

- **E1 Local economy.** Production, consumption, wages, from subsistence to markets.
- **E2 Trade nodes and routes.** Merchants collect and steer nodes. Traders run routes (caravans, ships, rail, shipping lanes).
- **E3 Markets and prices.** Supply and demand per node, shortages, inflation.
- **E4 Money.** Barter, commodity money, coinage, paper, fiat, digital. Each entity can have its own currency.
- **E5 Exchange and reserves.** Exchange rates, conversion, reserve currency, currency hegemony.
- **E6 Banking.** Credit, interest, fractional reserve, bank runs, central banks, world banks.
- **E7 Debt.** Sovereign debt, default, restructuring, crisis spread.
- **E8 Corporations.** Charter companies, joint stock, multinationals, public listings, stock trading, corporate influence.
- **E9 Monopolies.** Cost, technology, material and trade monopolies, and antitrust.
- **E10 Embargo and sanctions.** Blockades, tariffs, smuggling, sanction evasion.
- **E11 Taxation and budgets.** Tribute, corvee, tax systems, spending priorities.
- **E12 Media economy.** Media output and influence, entertainers and stars.
- **E13 Internet.** Sector upgrades (corporate, economic, infrastructure) that create open or closed systems.
- **E14 Labor and informal economy.** Unions, strikes, black markets.
- **E15 Inequality.** Wealth spread feeds factions and scoring.

## G. Society (L3)

- **S1 Factions and approval.** Interest groups (nobles, clergy, merchants, farmers, military, workers, industrialists, students) with approval and demands.
- **S2 Government and legitimacy.** Governance forms, succession law, legitimacy.
- **S3 Culture.** Weighted trait bundles, syncretism, culture groups with visual skins.
- **S4 Religion.** Beliefs, administration, cohesion, factions, schisms, reformations, defender of the faith.
- **S5 Leaders.** Paragons, heroes, generals, scholars, prophets with traits, aging and death.
- **S6 Families and dynasties.** Marriage, heirs, claims, regency, succession crises.
- **S7 Unrest.** Protests, revolts, coups, civil war, secession.
- **S8 Law and corruption.** Crime, enforcement, administrative decay.
- **S9 Soft power.** Propaganda, media reach, language spread.
- **S10 Ideologies.** Modern-era belief systems that shape civics and factions.

## H. Entities and projects (L2 to L3, detailed in section M)

- **N1 Entity model.** Population groups, territory, institutions, leader and dynasty, culture traits.
- **N2 Entity forms.** Emergent forms from band to supranational union.
- **N3 Cultural pull.** Pull on nearby groups from prestige, surplus, tech, religion, language, trade, proximity, media.
- **N4 Assimilation and absorption.** Voluntary shift by affinity, or conquest and union.
- **N5 Trait inheritance.** Absorbed traits blend into the entity's culture.
- **N6 Fission and fusion.** Bands split, merge, settle, collapse, revolt.
- **N7 National projects.** Single-entity projects that change form or institutions.
- **N8 Joint and special projects.** Multi-sponsor and rare projects (great works, FTL). Great works are built under R32 with no artificial global races; wonders are designated from them at each era's end (X29) (revised 26 Nov 2026).
- **N9 Lineage.** The player's history and scoring persist across form changes.
- **N10 Subjects.** Vassals, tributaries, colonies, protectorates.

## I. Diplomacy and politics (L4)

- **D1 Contact and knowledge.** First contact, map knowledge, rumor.
- **D2 Opinion and reputation.** Trust, grievances, track record.
- **D3 Treaties.** Alliances, pacts, marriage, trade deals, tribute, borders, guarantees.
- **D4 Imperial structures.** HRE-style elective emperor, imperial authority, free cities, princes.
- **D5 Colonial nations.** Overlords and subjects with liberty desire.
- **D6 International bodies.** Leagues, a UN-like body, trade blocs, currency unions.
- **D7 Religious diplomacy.** Defender of the faith, holy wars, conversion pressure.
- **D8 Corporate diplomacy.** Corporations as treaty partners and lobbyists.
- **D9 Civilian and military channels.** Back channels, attaches, joint exercises.
- **D10 Casus belli and peace.** War goals, peace terms, reparations.
- **D11 Covert operations.** Intelligence, subversion, sabotage.
- **D12 AI personalities.** Culture and leader shape diplomatic style.

## J. Warfare (L4)

- **M1 Forces by era.** Bands, levies, professional armies, fleets, mechanized, air, space, cyber.
- **M2 Logistics and attrition.** Supply lines, terrain and climate attrition.
- **M3 Terrain and fortification.** Height bands, rivers, forts, chokepoints.
- **M4 Combat resolution.** Deterministic, driven by tech, terrain, leaders, supply and doctrine.
- **M5 Naval and amphibious.** Fleets, blockades, landings.
- **M6 Air and space.** Air superiority, strategic reach, orbital assets.
- **M7 Sieges.**
- **M8 Occupation.** Resistance, garrisons, assimilation or unrest.
- **M9 War support.** Exhaustion and mobilization, conscription.
- **M10 Military industry.** Production, templates, stockpiles.
- **M11 Nuclear weapons.** Proliferation, deterrence, fallout and climate effects.
- **M12 Mercenaries and irregulars.** Private forces, militias, guerrillas.
- **M13 Doctrine and tradition.** Experience and culture shape how forces fight.

## K. Narrative, endgame and scoring (L5)

- **X1 Story engine.** Events generated from simulation state (famine, discovery, schism) with scripted overlays and anomalies.
- **X2 Chronicle.** An auto-written history with named ages.
- **X3 Special projects.** Great works and landmarks (R32), large national efforts, the FTL project.
- **X4 FTL project.** Stages, sponsor shares, espionage, sanctions, secrecy, a final test run that ends the game.
- **X5 FTL pressure.** Two sources, kept separate: emergent pressure from simulated scarcity, demand, project payoffs and competition; and the hard-coded FTL horizon value (X28), which stands for benefits outside the simulation (revised twice on 24 Nov 2026; owner-confirmed).
- **X6 Scoring.** 24 categories ranked across all peoples.
- **X7 Game creation.** Real-Earth start for 10,000 BCE, selectable peoples per zone and cradle.

## L. AI and engine (cross-cutting)

- **A1 Entity AI.** Needs and goal driven. Non-player peoples follow the same rules as the player.
- **A2 Level of detail.** Coarser updates for quiet entities, judged only from simulation state (activity, volatility, involvement in events, significance), never from the player's position, controlled country or view; observing or switching country cannot change outcomes (revised 25 Nov 2026). Active entities act every phase, quieter ones every 2 to 4 phases, and dormant ones once per turn at a fixed offset phase derived from the entity id. Any event involving a remote entity wakes it to full cadence.
- **A3 Determinism.** Seeded randomness and fixed-point math.
- **A4 Explainability.** Every number shows its breakdown.
- **A5 Validation suite.** Real cases (Sentinelese, North Korea, China after 1978, Meiji Japan, India, Tasmania) test the equations against plausible ranges and causal relationships, not by requiring history to reproduce its actual outcomes.
- **A6 Data tables.** Content in tables for balancing.

## M. Entity-transforming projects

An entity changes form when it completes a project. Forms are emergent states set by measurable thresholds, and projects are how the player (or an AI) crosses them. Each project has prerequisites (tech, civics), capacity (surplus, labor, administrators), legitimacy and cohesion, a cost and duration scaled to turn length, a risk of failure or backlash, and effects that enable institutional changes (offices, laws, administration, civics, institutions, culture mix). The resulting form and name are derived from the new institutional configuration (L1, S29), never granted as a predefined political form (revised 25 Nov 2026). Projects can be national (one entity), joint (several sponsors) or special (rare and huge).

| Transition | Real example | Project | Key requirements | Main effects |
| --- | --- | --- | --- | --- |
| Band to village | Natufian settlements | Permanent settlement | Storable surplus, reliable water, population of a few hundred or fewer | Districts, granaries, more disease and inequality |
| Village to chiefdom | Gobekli Tepe, Poverty Point | Monument and feast project | Surplus, ritual leader, labor mobilization | Rank, redistribution, specialists |
| Chiefdom to early state | Uruk | Temple, granary and records | Irrigation, administrators, tally or writing | Taxation, writing, standing guard |
| City-state to kingdom | Narmer, Sargon | Unification charter | Absorbed neighbors, army, legitimacy, shared language | Provincial administration, blended culture |
| Kingdom to empire | Achaemenid Persia | Imperial charter | Several cultures, road network, coinage | Governors, tribute, imperial authority |
| Dynastic to feudal or theocratic | Medieval Europe, Tibet | Oaths of fealty or church codification | Landed elites or institutional faith | Vassal ties, religious administration |
| Empire to nation-state | Meiji Japan, revolutionary France | Constitution and standard language | Mass literacy, print, railways, conscription | National identity, conscript army |
| Monarchy to republic | Rome, the United States | Constitutional convention | Strong factions, legitimacy crisis | New succession and civics |
| Agrarian to industrial | Britain, Japan | Industrialization program | Energy, capital, urban labor | Factories, urbanization, new factions |
| States to union | EU, USSR | Treaty of union | Several entities with shared interests | Pooled sovereignty, shared currency or army |
| Entity to consortium or corporation | East India Companies, UK-US nuclear project | Charter or consortium agreement | Capital, shared goal, partners | Shares, joint control |
| Collapse or split | Bronze Age collapse, Soviet breakup | (Reverse) Crisis cascade | Overstretch, famine, revolt, loss of legitimacy | Fission into smaller entities |

Other transformations follow the same pattern: adopting a state religion, a reformation, a revolution, founding a colony, founding a diaspora. Reversals are possible, since forms can regress.

## N. Additions from the real-world audit

These extend the domains above. A star (★) marks the twelve you approved. The audit covered energy, food, water, materials, manufacturing, transport, communications, urban systems, health, education, finance, law, governance, security, culture, environment, space and information.

**Energy and electricity**

- **R10 Electricity generation.** Thermal, hydro, wind, solar, nuclear, geothermal, tidal and fusion plants, each with capacity factor, fuel needs, cooling water and build time.
- **R11 Grid.** Transmission and distribution, losses, voltage and frequency standards, regional grids, interconnectors, cascading blackouts.
- **R12 Dispatch and storage.** Baseload versus peaking, intermittency, batteries, pumped hydro, hydrogen, demand response, price signals.
- **R13 Electrification.** Demand growth by sector, stranded fuel assets, transition cost, electrified transport and heat.
- **R14 Energy security.** Import dependence, pipelines, strategic reserves, supply shocks, producer cartels.
- **R15 Fuel cycles.** Refining, liquefied gas, enrichment, waste storage, decommissioning.
- **R16 Energy-water link.** Cooling needs, hydropower versus irrigation, desalination energy cost.
- **W25 Space weather.** Solar storms damage grids and satellites.

**Food, water, materials, industry**

- **R17 Agricultural systems.** Fallow and rotation, irrigation, synthetic fertilizer, mechanization, high-yield and engineered crops, yield variance by climate.
- **R18 Food security.** Storage, famine relief, price shocks, aquaculture, fisheries management.
- **R19 Water supply.** Aquifer depletion, desalination, water rights, urban water systems.
- **R20 Materials.** Cement, steel, glass, plastics, chemicals, critical minerals, recycling and a circular economy.
- **R21 Industrial base.** Sector productivity, learning curves, economies of scale, supply chains, semiconductor fabrication.
- **R22 Supply-chain resilience.** Single points of failure, just-in-time versus stockpiles, chokepoints.
- **R23 Standards.** Rail gauges, units, protocols, standards wars.
- **R24 Shipping.** Containerization, port capacity, canal and strait chokepoints.
- **R25 Forestry.** Timber, charcoal, managed versus cleared forest.

**Population, cities, services**

- **P14 Urban planning.** Housing, zoning, slums, transit, congestion, land value. Linked to P34 to P36.
- **P15 Real estate.** Housing markets, mortgages, bubbles. Land values feed site ranking (P34).
- **P16 Public services.** Water, sanitation, healthcare, education, pensions, welfare and their fiscal cost.
- **P17 Transport systems.** Modes, networks, vehicles, airports, speed and capacity.
- **P18 Telecommunications.** Courier, telegraph, telephone, broadcast, internet, satellites. Sets information speed and censorship options.
- **P19 Healthcare systems.** Medicine, vaccines, antibiotics and resistance, chronic disease, aging, pandemic response, pharmaceutical industry.
- **P20 Civil protection.** Emergency response, disaster relief, civil defense.
- **P21 Waste management.** Collection, landfill, recycling, hazardous waste.
- **P22 Quality of life.** Safety, leisure, happiness. Feeds approval and scoring.
- **P23 Aging and pensions.** Dependency ratios and fiscal burden.
- **P24 ★ Land tenure and labor.** Commons, feudal, private and state land. Slavery, serfdom, indenture, wage labor, with abolition and reform dynamics.

**Environment**

- **W18 Pollution and ecosystem services.** Air, water, soil and plastic pollution, ozone, pollinators, fisheries collapse, ocean acidification.
- **W19 Conservation.** Reserves, parks, rewilding, species reintroduction.
- **W23 ★ Contact epidemics.** Immunity histories per population, first-contact mortality, crop and animal exchange along new routes.
- **W24 ★ Transboundary water.** Shared rivers, upstream and downstream conflict, water treaties, dam disputes.

**Economy and finance**

- **E16 Insurance.** Risk pooling for disasters, crops, trade and lives.
- **E17 Property and contract law.** Property rights, courts, enforcement.
- **E18 Business cycles.** Booms, recessions, unemployment, inflation and deflation, central bank tools.
- **E19 Global finance.** Capital flows, foreign investment, sovereign wealth funds, currency crises, speculative attacks.
- **E20 Labor migration.** Remittances, brain drain, guest workers.
- **E21 Trade policy.** Tariffs, quotas, free-trade zones, specialization, resource-curse effects.
- **E22 Intellectual property.** Licensing, copying, patent pools.
- **E23 Aid and development finance.**
- **E24 Illicit economy.** Smuggling, drugs, organized crime, money laundering.
- **E25 Services and tourism.**
- **E26 Digital economy.** Platforms, data as a resource, cryptocurrency, digital sovereignty.

**Society and politics**

- **S11 Education systems.** Schooling, ideology transmission, skilled labor supply.
- **S12 Civil society and press.** NGOs, free press, academia.
- **S13 Elections and parties.** Voting systems, polarization, populism.
- **S14 Courts and constitutions.** Rule of law, judicial independence.
- **S15 State capacity.** Bureaucracy, civil service, corruption.
- **S16 Policing and surveillance.** Privacy, digital authoritarianism.
- **S17 Family and gender.** Fertility norms, household structure, women's participation.
- **S18 Mobility and divides.** Class, urban-rural and ethnic divides.
- **S19 Movements and nationalism.** Separatism, revolutions, reform movements.
- **S20 Information ecosystem.** Public opinion, misinformation, censorship.
- **S21 Entertainment and sport.** Celebrity culture, mass events.
- **S22 Humanitarian crises.** Refugees, famine, aid.
- **S23 Extremism and terrorism.**
- **S24 Philosophy and ideas.** Schools of thought as civic inputs.
- **S25 ★ Culture production and heritage.** Art, architecture, literature, music and heritage sites feed influence, identity and scores.
- **S26 ★ Piracy, banditry and outlaw zones.** They appear wherever state reach is weak and loot is available.

**Knowledge and technology**

- **K11 Science system.** Basic versus applied research, breakthroughs, scientific revolutions.
- **K12 Skilled labor pipeline.** Education to research capacity.
- **K13 Dual-use control.** Export controls and nonproliferation.
- **K14 Technology accidents.** Reactor and chemical accidents, lab leaks, AI failures.
- **K15 Obsolescence.** Stranded assets, retraining, creative destruction.
- **K16 ★ Biotech, automation, AI and geoengineering.** Life extension and gene tech reshape demographics. Automation reshapes labor. AI affects decisions and risk. Geoengineering is a climate response with side effects.

**Entities, administration, territory**

- **N11 ★ Nomadic and pastoral lifeways.** Mobile camps, herd management, seasonal pasture rounds, horse and camel domestication, steppe confederations, raiding versus tribute versus trade.
- **N12 ★ Administrative reach.** Control decays with distance and message speed. Governors gain autonomy, orders arrive late, overstretch drives revolt and fission.
- **N13 ★ Exploration and navigation.** Wind- and current-driven sea lanes, ship types, navigational tech, fog of war, discovery rewards, chart trading, port networks.

* **N14 Ruler perspective.** The player plays the character leading the country. The ruler's traits, health, family and ambitions limit and shape the available actions. Death passes play to the heir, with succession crises where claims conflict. Character detail scales with the era: an anonymous leader for bands and tribes, a named ruler, heir and notables for chiefdoms and early states, house members from 10-year turns, and full Crusader Kings-style detail from 5-year turns. Only notables are simulated per phase, and a ruler who dies mid-turn is replaced by the heir from the next phase.

**Diplomacy**

- **D13 ★ Global commons and climate treaties.** Emissions and fishing quotas, free riding, monitoring, enforcement by sanctions.
- **D14 ★ Territorial claims and sovereignty.** Control versus claim, frontiers, unclaimed zones, recognition.
- **D15 Treaty lifecycle.** Negotiation, ratification limits, withdrawal.
- **D16 Global governance.** A UN-like body, councils, vetoes, courts.
- **D17 Borders and migration policy.** Passports, visas, walls, refugee quotas.
- **D18 Cultural and language diplomacy.**

**Warfare**

- **M14 Arms trade.** Exports, embargoes, dependency.
- **M15 Arms control and deterrence.** Treaties, doctrines, command and control.
- **M16 Civil-military relations.** Coups, military rule.
- **M17 Defense industry.** Procurement, technology cycles, cost overruns.
- **M18 Laws of war.** War crimes, tribunals.
- **M19 Alliances.** Collective defense, burden sharing.
- **M20 Proxy wars and insurgency.**
- **M21 Peacekeeping and reconstruction.**

**Space domain**

- **Scope.** Earth-facing only. Habitats, bases, mining and interplanetary logistics are out of scope. The FTL project is the only reach beyond Earth, as the endgame trigger.
- **SP1 to SP5.** Launch and orbit, satellite services, debris and space law, planetary defense, and the space-derived Earth economy. Detailed in Deep Dives section 5.

**Narrative**

- **X8 Black swans.** Rare high-impact events (supervolcano, novel pandemic, technology surprise).
- **X9 Archaeology and memory.** Ruins of collapsed peoples, rediscovered knowledge, how later peoples remember the past.

## O. Key feedback loops

The interlocking-systems pillar means these loops are where the game's depth comes from.

1. **Climate to migration to conflict.** Drought lowers yields, which drives migration and unrest, which drives conflict and fission.
2. **Energy to industry to emissions.** Cheap energy grows industry, which raises emissions, warming, disasters and costs, and pushes energy policy.
3. **Population to land to biodiversity to disease.** Expansion removes habitat, which raises zoonotic risk and fisheries and pollinator decline.
4. **Tech to education to urbanization to fertility.** Development changes demographics, which changes labor and the fiscal burden.
5. **Trade to contact to epidemics.** New routes spread goods, ideas, crops and disease.
6. **Finance to debt to crisis to unrest.** Credit booms end in crises that topple governments.
7. **Water to dams to food to conflict.** Upstream control changes downstream harvests and treaties.
8. **Information tech to polarization to legitimacy.** Media shape faction strength and policy room.
9. **Reach to overstretch to fission.** Empires grow until communication lag outpaces control.
10. **War to industry to technology.** Conflict pulls investment and speeds diffusion of tech.
11. **Discovery to chain to monopoly to sanctions.** Resource finds create industries, rents, sanctions and substitution.
12. **Culture pull to assimilation to new cultures.** Absorption blends traits, which changes pull.
13. **Sea level to coasts to migration.** Flooding forces moves and reshapes housing and port economies.
14. **Energy security to war to climate policy.** Dependence drives alliances and conflict, which delays decarbonization.
15. **Automation to labor to unrest.** Productivity gains shift jobs, factions and policy.
16. **Grid to everything.** A major blackout hits industry, health, finance and order. It is the most coupled single mechanic in the modern era.

## P. Additions and refinements from the Q&A (confirmed)

These entries continue each ID series and extend or refine the entries above. The details are in Final Master Vision section 9.

### Authoritative dependency layers

These replace the provisional L0 to L5 guesses. They classify mechanics by what they fundamentally need, and feedback loops cross layers explicitly.

- **Layer 0, simulation foundations.** Canonical time, deterministic arithmetic, identity registry, event scheduling, state ownership, conservation ledgers, spatial identifiers, versioned persistence.
- **Layer 1, physical world.** Terrain, water, climate, soils, biomes, animals, resource stocks, hazards, geographic networks.
- **Layer 2, population and material economy.** Demography, migration, settlements, labor, extraction, agriculture, production, transport, consumption, energy, inventories.
- **Layer 3, institutions, knowledge and markets.** Technology, diffusion, education, culture, religion, political authority, government capacity, trade, markets, finance, corporations, factions.
- **Layer 4, strategic interactions.** Diplomacy, treaties, intelligence, espionage, military organization, warfare, occupation, revolutions, state transformations, national and international projects.
- **Layer 5, observation, history and analysis.** Chronicle, scoring, data provenance, causal attribution, comparison, forecasting, counterfactuals, replay, player information access. It observes authoritative data and never manufactures facts.

**Priority for the first detailed specifications.** Critical: population and demographics, resource and production accounting, water and hydrology, energy and electricity, markets and finance, political legitimacy and capacity. High: transport and logistics networks, technology and diffusion, land use and ecological change, disease and public health, military logistics, information and intelligence, historical causal attribution.

### Specification template for every mechanic

Mechanic ID, name, purpose, dependency layer, authoritative owner, state variables (types and units), inputs, outputs, equations, resolution frequency, conservation constraints, feedback connections, actor interaction and authority, information model, AI decision model, performance classification, historical validation, user interface locations (levels 1 to 4), failure modes, testing (invariants, edge cases, convergence, regression), and status (confirmed, proposed, calibration required, implementation verified). The dependency matrix is generated from these records. Superseded by the unified 21-field template in Computational Doctrine section 5, which adds units and dimensions, assumptions, initial and boundary conditions, constraints, explicitly compared candidate algorithms, time and space complexity, numerical stability and approximation error, expected workload and benchmarking.

**ID conventions.** Mechanic IDs are bare series codes (T, W, P, R, K, E, S, N, D, M, X, A, U, L, I, SP), so E1 is always the economy mechanic. Interface workspace IDs always carry the prefix WS- (WS-A1 to WS-H3, registry in Final Master Vision 9.11), so WS-E1 is always the Technology Tree workspace. All interface IDs (LV, CG-, WS-, /V, DS-, AN-, GS-, AP-, XL-) follow the permanent convention in Final Master Vision 9.11 and never reuse mechanic codes; interface levels are LV1 to LV4 because L1 to L10 are mechanics. The prefix is needed because workspace groups A, D and E share letters with the engine, diplomacy and economy series. The specification template's user-interface field cites workspaces by their WS- IDs. Every specification also states how it meets the governing engineering principle: simulate the maximum meaningful complexity with the minimum necessary computation.

### New and refined entries

**Time and turns**

- **T12 Integer clock.** Elapsed seconds from the start, event-driven, with exact calendar boundaries.
- **T13 Interruption classes.** Informational events auto-resolve. Decision-critical events pause Advance Until. Rare designated interruption points exist.
- **T14 Standing-order failure.** Paused, partial or failed states with explained causes, no irreversible action without authority, no automatic refunds.
- **T15 Pacing separation.** Physical travel, strategic deployment, administrative execution and turn tempo are separate quantities.

**World simulation**

- **W26 Catchment water graph.** Directed graph with inflow, runoff, groundwater, withdrawals, reservoirs, evaporation, flood loss and outflow.
- **W27 Hazard, exposure, vulnerability.** Disasters separate the natural event from who is affected and how badly.
- **W28 Ecological thresholds.** Extinction, salinization, aquifer exhaustion and fisheries collapse with hysteresis and recovery delays.
- **W29 Hierarchical climate.** Global trends, spatial cells, monthly summaries and extreme events, calibrated reduced-order models.

**Population and settlement**

- **P25 Cohort key and age operator.** Province, culture, religion, class with age distributions and a time-based age-transition operator.
- **P26 Migration decisions and conservation.** Utility-based destination choice, matched departures and arrivals.
- **P27 Compartmental disease.** Extended susceptible, exposed, infectious, recovered models linked by trade, armies and migration.
- **P28 Households and families.** Statistical households, explicit families for notables.
- **P29 Urban land consumption.** Demand for housing, commerce, industry and infrastructure allocated to eligible cells. Expanded by P34 (26 Nov 2026).
- **P30 Collective behavior.** Group preferences from conditions, producing protest, migration, mobilization or rebellion.
- **P31 Demographic reconciliation.** The population identity checked every update.

**Resources and industry**

- **R26 Conservation ledgers.** Closing stock identity for every resource, with losses and byproducts.
- **R27 Production constraint and substitution.** Output limited by the scarcest requirement, with substitution rules and unsubstitutable bottlenecks.
- **R28 Capital formation and depreciation.** Construction, installation, commissioning, and separate wear, damage, backlog, obsolescence, stranding and destruction.
- **R29 Ownership versus control.** Shares, contracts and legal structures, with ownership summing to the recognized total.
- **R30 Grid failure cascades.** Capacity violations and dependencies that isolate demand and interrupt hospitals, industry, finance and communications.

**Economy and finance**

- **E27 Market clearing.** Deterministic, damped, with rationing and unsold inventory when markets fail.
- **E28 Money regimes.** Creation and destruction only through modeled issuance, lending and repayment.
- **E29 Bank balance sheets.** Assets equal liabilities plus equity, with losses spreading through counterparties.
- **E30 Government finance.** Budget identity, creditor claims, recorded arrears and defaults.
- **E31 Inflation from a basket.** Derived from modeled prices and weights, never a modifier.
- **E32 Foreign exchange regimes.** Pegs, managed floats, market and commodity-backed systems with balanced currency ledgers.
- **E33 Economic aggregation.** Value added without double-counting intermediate goods.
- **E34 Informal-economy accounting.** Subsistence and barter in material balances, with gaps between reported and true figures.
- **E35 Crisis propagation.** Through input-output links, inventories, transport, banks, debt, employment, demand and legitimacy.
- **E36 Nominal and real separation (added 25 Nov 2026).** Physical production, financial claims, liquidity, solvency, debt, ownership, purchasing power and political authority are separate quantities. Authorised monetary and credit operations create claims; they never create goods, trained people, machinery or energy (AX-9). Money issuance works through the money regime (E28) and affects prices, exchange rates and trust, not physical stocks. Extends E28, E31, R26.
- **E37 International financial institutions (added 25 Nov 2026).** World banks and similar bodies are institutions (I1) with capital contributors, funding arrangements, assets, liabilities, lending mandates, governance rights, borrowers, repayment schedules, credit risk and objectives. They never generate money or grant diplomatic bonuses. Extends E6, E19, E23, D6.
- **E38 Credit lifecycle (added 25 Nov 2026).** Every loan, bond and central-bank operation is double-entry: issuing creates the matching asset and liability; repayment, restructuring, default and write-down post to every affected balance sheet, for commercial, sovereign, central-bank and international credit alike. Money amounts are tested for denomination change, high inflation, redenomination and aggregate exposure, with 128-bit aggregation. Extends E6, E7, E29, E30.

**Knowledge**

- **K17 Four knowledge concepts.** Scientific knowledge, technical knowledge, adoption, mastery.
- **K18 Discovery hazard.** Deterministic hazard model from specialization, institutions, materials, contact and need.
- **K19 Mastery.** Accumulated practice that can be lost with people and institutions.
- **K20 Misinformation and scientific uncertainty.** Wrong theories, misread evidence and fabrications, tested and corrected by institutions.
- **K21 Parallel invention provenance.** Independent discoveries keep their own first-discovery claims.

* **K22 Institutional research production.** Organised research originates from institutions, their personnel, infrastructure, resources and activities. Pre-institutional discovery continues through observation, craft practice, experimentation and accumulated experience. Extends K2, K5, K11 and K18.
* **K23 Research domain differentiation.** Output is classified by knowledge domain, subtree, research activity and institutional competence. Output in unrelated disciplines is not freely interchangeable. Transferable knowledge and interdisciplinary contributions remain possible.
* **K24 Institutional capability prerequisites.** Technologies and subtrees may require specific institutional competence, qualified personnel, organisational capacity, research infrastructure and accumulated expertise. Scientific accessibility does not grant research eligibility. Requirements are functional, not institution-name-specific. Extends K1, K4 and K18.
* **K25 Progressive scientific specialisation.** Rising scientific complexity creates demand for specialised disciplines, departments, laboratories, programmes and independent research organisations. Generalist institutions keep historically appropriate capabilities and may develop specialised internal structures.
* **K26 Research competence and maturity.** Competence accumulates through research activity, education, mentorship, experimentation, equipment use and organisational continuity. New institutions inherit only capabilities actually transferred to them. Competence decays through inactivity, disruption, obsolescence or loss of personnel.
* **K27 Research portfolio and allocation.** Institutions allocate personnel, funding, equipment, compute, facilities and other inputs among eligible programmes by mandate, objectives, incentives and authority. Commitments cannot be duplicated across simultaneous programmes.
* **K28 Interdisciplinary research.** Programmes can combine competence from several domains and institutions. Contributions depend on expertise, knowledge compatibility, coordination capacity, information access and contracts.
* **K29 Research activity differentiation.** Basic research, applied research and engineering development have distinct objectives, inputs, institutional requirements and outputs. Discoveries, technical designs, validated prototypes, adoption and mastery are tracked separately.
* **K30 Scientific labour mobility and succession.** Researchers migrate, retire, die, change employers, train successors and found scientific schools. Skilled labour and mentorship networks shape competence, continuity, diffusion and brain drain. Extends K12, K19, P4 and S5.
* **K31 Scientific freedom and research restrictions.** Laws, religious authority, political interference, institutional governance, commercial secrecy, censorship and security restrictions affect eligibility, priorities, publication and collaboration, with context-specific consequences, not universal penalties.
* **K32 Research infrastructure dependencies.** Laboratories, libraries, archives, observatories, experimental facilities, computing systems and testing infrastructure deliver research capability, subject to operating capacity, maintenance, energy, materials, skilled labour and technical compatibility.
* **K33 Scientific collaboration and competition.** Institutions pursue discoveries alone or jointly. Parallel discoveries keep separate provenance. Collaboration creates attributable contributions, and competition shapes incentives, recruitment, secrecy, intellectual property and priorities. Extends K7, K21 and I7.
* **K34 Institutional knowledge continuity and loss.** Dissolution, war, persecution, migration, destruction and decline can reduce research capacity, competence, technical knowledge or mastery independently. Archives, distributed expertise and successor institutions can preserve or recover part of the loss.
* **K35 Scientific discovery and institutional feedback.** Discoveries open new institutional mandates, specialised capabilities and subtrees. Institutional development then widens eligible research and modifies the discovery hazard.
* **K36 Research contribution and provenance accounting.** Discoveries and programmes keep attributable contributions from researchers, institutions, sponsors and collaborators. Financial contribution, scientific contribution, intellectual property ownership and operational control stay distinct. Shared achievements reconcile without duplicated scoring credit.
* **K37 Research aggregation and explainability.** Aggregate research indicators derive from institution-specific and domain-specific activity, and distinguish available capacity, committed capacity, realised output, annual rates, accumulated progress and uncertainty. Aggregate output is not spendable research points.
* **K38 Research resolution and conservation.** Invention, diffusion and competence updates follow the knowledge-system cadence (turn end). Relevant phase events change effective resources and institutional conditions through phase deltas. Calculations use elapsed time, deterministic resolution and seeded discovery streams. Funding, personnel, materials, equipment and compute reconcile with their authoritative systems.
* **K39 Research scalability.** Ordinary researchers are population and occupational aggregates. Notable scientists are characters. Institutions have persistent identities, while departments and programmes are aggregated records unless significance requires more. Resolution evaluates eligible active programmes and their dependencies, not every institution-technology pair.

**Society and government**

- **S27 Authority requirement.** Every order needs legal authority and enforcement capacity. Illegal attempts are explicit political acts.
- **S28 Administrative capacity.** Competence x staffing x reach x resources x legitimacy, minus corruption and disruption.
- **S29 Emergent government forms.** Names derived from institutional configuration.
- **S30 Legitimacy by group.** Aggregate with visible constituents.
- **S31 Institutional continuity.** Institutions outlive officeholders and dynasties.
- **S32 Revolution conditions.** Grievance, organization, leadership, resources, support, security loyalty, response, triggers, outside help.
- **S33 Fragmentation allocation.** Explicit division of territory, people, assets, liabilities and claims.
- **S34 Divided polity (owner-confirmed 26 Nov 2026).** After a coup, civil war or contested succession a state can be neither separated nor unified: one entity and identity, several claimant factions, control resolved per domain (formations, treasury and mint, taxation, administration, courts, diplomacy and recognition, institutions) and per place (provinces, administrative units, parts of a city) through the separate rights relations (I19), with validity and effectiveness tracked apart (L10). Uncaptured components stay common to all factions (currency, laws in force, citizenship and identity, culture and religion, knowledge, honoured treaties, shared institutions and infrastructure) and their effects reach every faction once. The player chooses which faction to lead. Ends in reunification, settlement, formal separation (S33) or persistence. Extends S7, S32, S33, N14, L10. Owner: OS-POLITY.

**Entities and projects**

- **N15 Knowledge states and uncertainty.** Objective state, entity knowledge and player-visible knowledge as separate records.
- **N16 Collapse taxonomy and recovery.** Crisis, collapse, fragmentation, transformation, extinction, with recovery needing surviving capability.
- **N17 Civilization lineage network.** Branching ancestry through splits and mergers.
- **N18 Player career record.** Entities led, dates and outcomes, separate from scoring.

**Diplomacy and war**

- **D19 Treaty architecture.** Parties, obligations, ratification, duration, enforcement, remedies, secrecy.
- **D20 Trust from memory.** Remembered fulfilled and broken commitments, differing by institution and leader.
- **D21 Intelligence estimates.** Ranges, confidence, recency, contradictory assessments.
- **M22 Military hierarchy.** Personnel to strategic command with era-specific forms.
- **M23 Command delay.** Orders in transit, outdated orders, electronic vulnerability.
- **M24 Morale and cohesion.** Casualties, supply, leadership, fatigue, experience, ideology.
- **M25 Occupation versus control.** Needs security forces, cooperation and reach.
- **M26 War termination.** Many end paths, no universal warscore.
- **M27 Postwar accounting.** Casualties, veterans, refugees, damage, debts, transfers, grievances, reconciled with ledgers.
- **M28 Military AI planning.** Objectives to feasible operations from its own intelligence.

**History and narrative**

- **X10 Event identity and causal attribution.** Permanent IDs, direct, contributing and background causes.
- **X11 Counterfactuals.** Exact reruns or labeled approximations.
- **X12 Historical replay.** Same state hashes as the original run.
- **X13 Uncertainty presentation.** Estimate, interval, date, source, reliability, for the player's own statistics as well as foreign ones (P37).
- **X14 Forecasting.** Central, optimistic and adverse scenarios.
- **X15 Narrative selection and explainability checks.** Importance ranking, explanations verified against recorded data.

**Engine and AI**

- **A7 Precision classes.** Exact, fixed-point, statistical, forecast.
- **A8 Adaptive resolution.** Full detail for important actors, integrated rates for quiet regions.
- **A9 Deterministic parallelism.** Disjoint partitions with stable aggregation.
- **A10 Save persistence and migration.** Versioned, hashed, deterministic migrations.
- **A11 Invariant assertions.** Conservation and integrity checks with reproducible diagnostics.
- **A12 Benchmark worlds.** Eight standard scenarios on reference hardware.
- **A13 AI horizons, objectives and coordination.** Hierarchical planning, multi-objective utility, institutional coordination that can fail. No single general-purpose AI: strategic objectives set institutional priorities, institutions allocate mandates, specialised decision systems evaluate feasible alternatives, and individuals act within their roles, each actor with its own interests, information, authority and horizon (revised 25 Nov 2026).

**Interface**

- **U1 Navigation state.** Selection, focus, layers, date, dossier, filters and comparison preserved across levels.
- **U2 Shared entity selection.** One service, many workspace views.
- **U3 Dossiers.** Universal sections plus domain modules.
- **U4 Map layer combinations.** Semantic categories with visible priority.
- **U5 Semantic zoom.** More detail without inventing entities. Global, regional, local and detailed levels show the same state (U13).
- **U6 Data provenance.** Every statistic links to variable, owner, method, inputs and uncertainty.
- **U7 Action states and permissions.** Inspect, plan, issue, resolve, with explained blockers.
- **U8 Alert priority.** Critical, important, informational.
- **U9 Accessibility.** Scaling, contrast, color-independent symbols, keyboard use, search.
- **U10 Screen scaling.** 1920 x 1080 full dense interface, collapsing panels below.

## Q. Living Knowledge Order and Living Historiography (confirmed 24 Nov 2026)

Gameplay requirements are confirmed; formulations, parameters and algorithms are proposed for engineering evaluation (Computational Doctrine sections 18 and 19). IDs continue the existing series after a collision check (highest before: K39, X15, A13, U10). Every entry gets the unified specification template when its pass begins. Rules in Final Master Vision 9.18.

### Knowledge (K40 to K62)

- **K40 Knowledge artifact entity.** Generalised identity for every artifact class, with sparse domain components, permanent IDs and provenance. Significant works individual, ordinary production aggregated. Extends K5, K21. Owner: knowledge system.
- **K41 Claim-level knowledge.** Typed claims, findings, methods and datasets inside artifacts, with relationships (supports, contradicts, replicates, revises, supersedes, translates, summarises, extends, derives from, corroborates, depends on, applies, refutes) and epistemic categories. Used only where it matters.
- **K42 Content, copies and editions.** Content separate from physical and digital carriers, editions, translations, revisions and locations; non-rivalrous content, rivalrous media. Copies are DS-05 assets owned by resource accounting.
- **K43 Custody, ownership, rights and access.** Twelve distinct properties at individual, institution, organisation, government, population-group, civilisation and international levels, with access granularity (metadata to source code). Extends E22, S27.
- **K44 Knowledge lifecycle.** The fourteen states and their transitions, bottlenecks, throughput, costs, delays and failures, non-linear and domain-specific. Extends K3, K4, K17.
- **K45 Acquisition channels.** Acquisition through existing economic, legal, diplomatic, institutional, military, migration, intelligence and information systems; method sets cost, legality, secrecy, provenance, delay and risk.
- **K46 Transmission media.** Era-independent media with latency, reach, bandwidth, reproduction cost, infrastructure, energy, reliability, preservation, censorship, language, security and interoperability. Extends P18, E13, S20.
- **K47 Open-network publication.** Publication scopes reachable by all connected, permitted, capable actors, resolved by scopes and lazy queries.
- **K48 Absorptive capacity.** Discipline-specific, time-based absorption with decay through turnover, collapse and disuse. Extends K4, K26, K30.
- **K49 Forms of knowledge.** Codified, tacit, institutional, embodied, distributed, operational and dormant knowledge, with tacit transfer through apprenticeship, education, migration, employment, joint research and assistance.
- **K50 Operational capability and deployment.** Per actor, region, institution and industry: known, understood, demonstrated, prototype, limited reproduction, commercially feasible, deployed, adopted, unavailable, obsolete, known but no longer reproducible. Deployment consumes real resources through production chains. Extends K29, R21, R27.
- **K51 Capability regression and restoration.** Regression from decayed dependencies with documentation surviving; restoration from surviving knowledge, expertise, infrastructure and relationships. Extends K10, K34.
- **K52 Institutional knowledge collections.** Owned, custodial, licensed, shared, restricted, untranslated and damaged holdings; collection capabilities (cataloguing, translation, authentication, retrieval, dissemination); survival through succession and collapse. Extends I1, I15, K32.
- **K53 Knowledge synthesis.** Universal, recursive research capability producing new attributable artifacts; feasibility and quality from coverage, independence, reliability, competence and hard prerequisites, never book counts.
- **K54 Source independence and provenance.** Copies, translations, summaries, repetition and circular citation never count as independent evidence; sparse dependency structures.
- **K55 Theories and hypotheses.** Bounded hypothesis spaces over accessible evidence, with no access to hidden simulation variables; theories can be correct, approximate, conditional, incomplete, wrong or superseded. Extends K20.
- **K56 Peer review, publication, replication and consensus.** Historically situated, fallible scrutiny; replication distinct from acceptance; consensus distinct from political, public, religious and commercial acceptance.
- **K57 Grand Intellectual Works.** Classification of exceptional works; competing, superseding, never scripted, never objective truth.
- **K58 Intellectual first-mover, diffusion and obsolescence.** Prestige, talent and influence with diffusion, saturation, duplication limits and obsolescence. Extends K7.
- **K59 Intellectual diplomacy and knowledge agreements.** Access, exchange, joint research, licensing, translation, confidentiality, export restrictions, consortia, sanctions, assistance and repatriation, distinct from training or industrial transfer. Extends D19, I16.
- **K60 Intellectual genealogy and legacy.** Ancestry and influence of contributions across civilisations, feeding legacy and significance without rewarding volume.
- **K61 Knowledge benefit rule.** Benefits only through authorised systems with declared owner, scope, conditions and duration; no title bonuses, no duplicated benefits.
- **K62 Comparative disciplinary research.** Constitutional, sociological, demographic, geological, environmental, economic, financial and other disciplines on the shared architecture; findings never create resources or change natural laws.

### Truth, evidence, belief and consequence (added 24 Nov 2026)

Implements the Truth–Evidence–Belief–Consequence Separation axiom (Final Master Vision, foundational axioms). IDs continue K62, D21.

- **K63 Separated information properties.** For any information item: objective truth value (event store, never actor-readable), actual evidence, evidentiary strength, perceived credibility per actor, belief per actor or group, institutional acceptance, public acceptance and strategic utility, each a separate quantity. Extends K41, K56, S20, A8. Owner: knowledge state, with acceptance owned by institutions and public opinion.
- **K64 Credibility assessment.** Actors judge credibility from perceived evidence, source reputation, corroboration they can see, prior beliefs and incentives, never from objective truth; perceived evidence can differ from actual evidence; repetition and derivative copies are not corroboration (K54); credibility is not correctness. Extends K20, K54, D21.
- **K65 Consequence persistence.** Decisions, transactions, treaties, purges, panics, market moves and wars taken on accepted information stay in the world when the information is later corrected or discredited; correction is a new event with its own consequences (credibility loss, grievances, claims), not a reversal. Extends X10, X24.
- **D22 Information leverage.** Possessing, threatening to disclose, disclosing, circulating or suppressing information as leverage; value comes from the target's expected consequences (its legitimacy, market, alliance or personal exposure), not from truth; leverage is target-specific and issue-specific and decays or flips through counter-evidence, verification, pre-emptive disclosure, changed circumstances and counter-leverage. Acts through existing diplomacy, politics, markets and character systems. Extends D11, S9, S20, D1.
- **D23 Belief-based decisions and anticipation.** AI and institutional decisions use accessible information, beliefs, expectations, incentives and estimates of how other actors will respond to what those actors are believed to believe; no access to objective truth. Extends M28, D21, A8.

### History (X16 to X27)

- **X16 Historiography layers.** Objective, observed, recorded, interpreted and published history on the knowledge architecture, keeping distinct what occurred, what contemporaries believed, what later scholars concluded and what consequences followed from those beliefs (K63, K65). Extends X1, X2, X10.
- **X17 Universal historical coverage and contextual significance.** Any meaningful entity, place, system or subject, including extinct and dissolved ones. Extends X15.
- **X18 Geographic and entity continuity.** Histories follow permanent identities across division, merger, transfer, occupation and renaming without duplicating events.
- **X19 Historical memory forms.** Oral tradition to digital archives, emerging from capability.
- **X20 Historians, authors and perspectives.** Situated, grounded bias; competing accounts; characters and institutions as authors. Extends S5, S20.
- **X21 Editions, revisionism and time distinctions.** Dated, preserved editions; eight distinct times; no future knowledge in past works or decisions.
- **X22 Archives, evidence and archaeology.** Loss and recovery of evidence; archaeology derived only from real surviving evidence. Extends X9.
- **X23 Natural and physical history.** Independent of political ownership; reconstructable by later science.
- **X24 Historical memory as a gameplay input.** Legitimacy, identity, claims, grievances and propaganda only through authorised systems.
- **X25 Publication rendering architecture.** Deterministic offline baseline renderer; optional replaceable language-model renderers with claim validation and fallback; versioned publication metadata.
- **X26 Historical reader and intellectual library.** Reading, navigation, search, edition comparison, evidence and dependency inspection, uncertainty indicators, delegation-friendly collection views.
- **X27 Export.** Open formats without DRM, respecting access modes; omniscient export only in debug.
- **X28 FTL horizon value (owner-confirmed 24 Nov 2026).** The real benefits of FTL lie beyond Earth and after the game ends, so no simulated system can produce them. They are represented by one hard-coded, declared incentive: the declared exception to Universal Emergent Causality. The declared value is public and identical for all, but its weight in each actor's valuation scales with that actor's credible institutional awareness, feasible research paths and expected consequences of investment, so it has no practical influence in prehistory and early civilisation (revised 25 Nov 2026). FTL stays explicitly speculative physics, and the simulation does not guarantee it. It enters only declared valuation paths (AI and institutional utility for FTL-related research, institutions, funding and sponsorship; the value of FTL sponsor shares; the existing FTL contribution score) and does so only where an FTL-related option exists. It never grants research progress, capability or project stages; those still need the full capability path (K50, X4). Its size and any change over time are calibration parameters, not yet set.
- **X29 Wonders of the World (owner-confirmed 26 Nov 2026).** At the end of each of four world eras (Prehistoric, Ancient, Middle Ages, Modern), seven great works (R32) completed worldwide in that era are designated wonders automatically: 28 per campaign. Proposed era windows on fixed dates, half-open: 10,000 to 3000 BCE, 3000 BCE to 500 CE, 500 to 1500 CE, 1500 CE to the end of the campaign; the designation runs at the first turn boundary at or after each end date, and at campaign end for the last era. Score: the owner's factors grandeur (physical scale), cost (real inputs normalised by world output at construction) and uniqueness (distinctness from other great works), plus proposed technical ambition (capability relative to the world frontier) and renown (how widely the work is known); each factor ranked within the era's candidates, weighted sum, stable tie-break by completion date then identity. Damaged or destroyed works stay eligible. If fewer than seven great works qualify, the list is completed from named signature landmarks, and otherwise stays short, never padded. Recorded in the historiography (X16) and the Heritage score (X6); descriptive only, no simulated bonus. Owner: OS-OBS.

### Engine and interface

- **A14 Information access modes.** Normal, authorised perspective, debug tag-switch and omniscient developer modes, with no leakage into actor knowledge and no effect on state hashes.
- **A15 Knowledge and history storage scaling.** Deduplicated content, shared scopes, sparse access and dependency graphs, lazy queries, candidate filtering, event-driven invalidation and cached rendering.
- **A16 Mandates and delegation (added 25 Nov 2026).** In every domain the ruler can set mandates, appoint institutional leadership, allocate resources, set restrictions and intervene on exceptions, while institutions and characters carry out the work with their own competence, reliability, interests and authority. Close management is always possible but never compulsory. Delegation shifts decisions and risk, never grants an efficiency bonus. Generalises M42 and I14; works with T2, T10 and the hierarchical AI in A13.
- **U11 Knowledge and historiography interface integration.** WS-E2 and WS-H2 extensions, WS-E2/V2, WS-H2/V1, DS-07 and dossier modules (Final Master Vision 9.11).

### Dependencies and feedback loops (inputs to Pass 2b)

- **Owners.** Objective history: event store (X10). Artifacts and claims: knowledge system (K40, K41). Copies: resource accounting (K42). Access and rights: knowledge system with legal inputs (K43). Actor knowledge: knowledge state (A8 knowledge deltas). Competence and absorption: institutions (K26, K48). Capability and deployment: production and industry (K50). Historical memory beliefs: culture and politics (X24). Rendering: presentation only, owning nothing.
- **Loops.** Access, absorption, research, synthesis, new artifacts, more access; competence, research output, prestige, funding and talent, more competence (saturation needed); deployment, productivity, taxes, research funding; infrastructure collapse, competence decay, capability loss, restoration demand; historical memory, legitimacy, policy, new events, new history; open publication, faster diffusion, reduced first-mover rents.
- **Duplicated-benefit guard.** A competence gain affects output only through the research production function, never through a second bonus.

## R. Defining systems: governance, institutions, warfare

Detail in Final Master Vision sections 9.15 to 9.17. Note on IDs: your proposed M14 to M26 collided with IDs already in use (M14 to M28), so the new warfare entries are numbered M29 to M41, and each lists the existing entries it absorbs or extends.

### Living Constitutional Order (new L series)

- **L1 Multidimensional political model.** Continuous, possibly contradictory dimensions of power distribution, rights, participation, centralisation, coercion and ownership. Named forms (S29) are only derived labels. Replaces any government-type selection.
- **L2 Civilisational Governance Ledger.** Persistent record of executive acts, legislation, judicial precedents, political practices, social contracts, institutional behaviour and crises across dynasties.
- **L3 Claim, law and practice.** What a government claims, what its laws prescribe and how it behaves are tracked separately and all visible.
- **L4 Five-layer legal order.** Written law, interpretation, enforcement, public acceptance and precedent, each influencing the others.
- **L5 Precedent weight and constitutional inertia.** Weight grows with repetition, duration, enforcement, adoption, legitimacy, resistance overcome and significance, turning repeated practice into convention.
- **L6 Codification event chain.** Conventions, councils, assemblies, decrees, settlements and peace treaties producing documentary forms (Constitution, Imperial Charter, Sacred Legal Canon, Revolutionary Manifesto, Foundational Covenant, Customary Law Compendium, Party Charter) available only where history supports them. Article-by-article negotiation, faction concessions, imposition or abandonment.
- **L7 Governing documents as artifacts.** Readable text, amendments, provenance of every article, faction positions, interpretation, compliance status and version history. Every substantive provision maps to an operative rule.
- **L8 Constitutional erosion and failure.** Exploited provisions, indefinite emergencies, hollowed elections, courts and legislatures while the text survives.
- **L9 Rupture, inheritance and reinterpretation.** Revolution, conquest, collapse, reformation and fragmentation invalidate or reinterpret parts of the order without erasing them. Successors can share a document and read it differently.
- **L10 Legal validity and revolution.** Validity chains to a basic norm, and a revolution succeeds legally when the new order becomes effective by and large (Kelsen). Validity and effectiveness are tracked separately.
- **Extends:** S2, S13, S14, S19, S27 to S33, N7, D3.

### Autonomous institutions (new I series)

- **I1 Institution entity model.** Identity, founding history, legal status, governance, leadership, members or shareholders, assets, capital, revenue, liabilities, objectives, influence, technology, and optional jurisdiction, diplomacy and armed forces.
- **I2 Institution types.** Guilds, trading companies, chartered and joint-stock companies, listed and private firms, state enterprises, academies, research associations, universities, conglomerates, professional federations, trade leagues, banks, syndicates, multinationals, international organisations.
- **I3 Institutional lifecycle.** Formation, expansion, consolidation, fragmentation, merger, acquisition, nationalisation, privatisation, listing, dissolution, transformation, all emergent.
- **I4 Separated power dimensions.** Ownership, legal authority, operational control, political influence, territorial administration and effective sovereignty simulated independently.
- **I5 Company sovereignty.** Chartered companies with monopolies, private forces, territorial administration, taxation and treaty-making, holding effective sovereignty without statehood.
- **I6 Institutional governance evolution.** Hereditary ownership, partnerships, shareholder assemblies, boards, cooperatives, state appointment, councils, hybrids.
- **I7 Research institutions as actors.** Originate discoveries, own IP, set standards, steer industry, and in late eras rival states in compute, AI, finance and strategic technology. Internal research structure and competence in I11 to I18 and K22 to K39.
- **I8 Regulatory capture and political finance.** Capturing regulators, funding movements, dominating supply chains, becoming indispensable.
- **I9 Institutional memory and agency.** Persistent, independently deciding actors with competing interests.
- **I10 Charters in legal memory.** Charters, monopolies, autonomy, nationalisation precedents and public-private conflicts written into the Governance Ledger (L2).
- **I11 Internal institutional hierarchy.** Institutions may contain departments, faculties, laboratories, programmes, branches and subsidiaries, each with mandates, personnel, budgets, capabilities and reporting lines. Separate legal identity only where appropriate.
- **I12 Institutional research reform.** Institutions change research mandates, governance, structure, competence and specialisation through reforms that need authority, resources, personnel, prerequisites and implementation time, and may meet institutional or political resistance.
- **I13 Research institution formation.** New research organisations and units emerge through public founding, private initiative, scholarly association, religious patronage, corporate investment, fragmentation or joint sponsorship.
- **I14 Institutional research autonomy.** Research institutions choose programmes and allocation within their governance, contracts, legal authority and funding. Government influence does not mean unrestricted operational control.
- **I15 Institutional competence transfer.** Mergers, acquisitions, fragmentation, nationalisation, privatisation and restructuring transfer actual personnel, assets, records, intellectual property and relationships. Capabilities are never copied or duplicated automatically.
- **I16 Institutional research networks.** Collaboration, mentorship, funding, licensing, shared infrastructure and knowledge exchange, across borders where legal, diplomatic and security constraints allow.
- **I17 Institutional concentration and resilience.** Concentrated and distributed research ecosystems differ in coordination costs, economies of scale, dependencies, vulnerabilities and recovery, emerging from their actual structures and resources.
- **I18 Institutional scientific identity and memory.** Reputations, traditions, scientific schools, discoveries, internal precedents and experience persist through leadership changes and can be inherited, transformed or lost through succession.
- **Extends:** I1 to I10 (for I11 to I18), E8, E9, E19, E22, E26, K5, K11, K12, K17 to K39, D6, D8, N7, N8, S5 and the Living Constitutional Order.

### Warfare: amendments to existing entries

| ID | Change | Adds |
| --- | --- | --- |
| M1 Forces by era | Retain and expand | Military organisation, recruitment institutions, force composition |
| M2 Logistics and attrition | Major expansion | Physical supply networks, inventories, replenishment, maintenance |
| M3 Terrain and fortification | Major expansion | Frontage, accessibility, defensive depth, operational geography |
| M4 Combat resolution | Major expansion | Engagement stages, loss types, cohesion, retreat, pursuit |
| M5 Naval and amphibious | Expand | Sea control, convoys, naval logistics, landing capacity |
| M6 Air and space | Expand | Air operations, sensors, air defence, satellite dependency |
| M7 Sieges | Expand | Fortification condition, isolation, civilian consequences, relief |
| M8 Occupation | Major expansion | Civil administration, legitimacy, resistance networks, integration |
| M9 War support | Major expansion | Political authorisation, public opinion, fiscal endurance |
| M10 Military industry | Major expansion | Equipment inventories, procurement, production, repair |
| M11 Nuclear weapons | Expand | Deterrence, escalation, political authorisation, humanitarian consequences |
| M12 Mercenaries and irregulars | Expand | Contracts, patronage, loyalties, irregular organisations |
| M13 Doctrine and tradition | Major expansion | Institutional learning, training, organisational adaptation |

### Warfare: new entries

| ID | System | Purpose | Absorbs or extends |
| --- | --- | --- | --- |
| M29 | Strategic command and war planning | Connect political objectives to operations | M9, M28 |
| M30 | Military institutions and command authority | Hierarchy, staff capacity, command friction | M22, M23 |
| M31 | Operational campaign engine | Movement, reserves, objectives, engagements, campaign plans | M2, M4 |
| M32 | Military intelligence and reconnaissance | Observation into uncertain operational knowledge | D21 |
| M33 | Military personnel lifecycle | Recruitment by demography, training, casualties, veterans, demobilisation | M1, M27 |
| M34 | Equipment lifecycle and readiness | Condition, repair, replacement, obsolescence | M10, M17 |
| M35 | Military engineering | Fortifications, bridges, fieldworks, infrastructure restoration | M3, M7 |
| M36 | Coalition and allied command | Forces under differing political and military authorities | M19 |
| M37 | War termination and peace implementation | Negotiated settlements, ceasefires, enforcement, reparations | M26, D10 |
| M38 | Military political economy | War finance, industrial conversion, procurement, fiscal stress | M17, E30 |
| M39 | Civil-military relations | Loyalty, political interference, coups, institutional conflict | M16 |
| M40 | Strategic deterrence and escalation | Coercion, credibility, retaliation, escalation | M11, M15 |
| M41 | Military learning and institutional memory | Lessons, doctrine revision, experience transfer, decay | M13 |

- **M42 Command delegation.** Direct, Guided and Autonomous modes that redistribute decisions, information, authorisation time and commander-independence risk, never an efficiency bonus. Personal command depends on era and institutions.
- **M43 Conditional military interruptions.** Campaigns advance under standing orders until a strategically significant event needs authorisation (rules to be specified and tested). Extends T10 and T13.
- **M44 Event-driven military time.** Operational, engagement and consequence events resolve at their own timestamps inside the strategic phase. Extends T9.

## T. The Living City and Living Engraved Cartography (owner-directed, 26 Nov 2026)

Rules in Final Master Vision 9.20; engineering in Computational Doctrine 22. IDs continue P31, R30, I18, A16 and U11 after a collision check. Extends P5 to P10, P14, P15, P29, R8, R28, R29, N7, N8, X3, T2, M42, A16, I1 to I18 and U1 to U11 rather than duplicating them. Owners use existing systems; no parallel population, treasury, land, construction or institution ledger.

- **P32 Nested geography and settlement-driven subdivision.** Natural zone ancestry, provinces carved when a promoted town or city's catchment meets deterministic eligibility, administrative regions as province groupings, one city per administrative region with lawful reorganisation for a second, dated versions and full ancestry. Conserves people, stocks, debt, scoring and territory. Extends P5, P6, N12, S33. Owner: OS-POLITY, geometry identities in OS-IDENT.
- **P33 Settlement identity and lifecycle.** Stable identity through growth, promotion, decline, renaming, ownership change, destruction and reconstruction; physical absorption into a conurbation is distinct from legal annexation. Extends P5. Owner: OS-SETTLE.
- **P34 Urban land development.** Candidate-area suitability and ranking, independent candidate states, organic, incentivised, planned and compulsory growth, several emergent directions, reserves kept in prior use until converted, and separate land, floor, capacity and occupancy accounts. Extends P10, P14, P15, P29. Owner: OS-SETTLE.
- **P35 Districts, specialisation, mixed use and profiles.** Repeatable data-driven district families with separate identity, specialisations, actual and designated use; mixed use without extra districts; development profiles as objectives and weights; split, merge and lineage; the number of districts a city sustains is derived from multidimensional capacity (R33), never a slot count (26 Nov 2026). Extends P7, P14. Owner: OS-SETTLE.
- **P36 Employer and institutional housing and services.** Company colonies, railway and mining settlements, cooperative and employer housing with rent-free, subsidised, tied or market terms paid for by the provider; separate places, households, workforce and occupancy; services as real assets; dependence risk on closure. Extends P14, P16, E14, I1. Owner: OS-SETTLE, with costs in OS-MARKET and assets in OS-PROD.
- **P37 Censuses and population knowledge (owner-confirmed 26 Nov 2026).** Population is known only through counts: censuses, registers, tax, parish and conscription rolls and estimates, each a record with unit, date, method, coverage, estimate and accuracy, never exact. Made by whoever holds authority and capability: village councils, city administrations, provincial and state offices, and the army as a special project in areas where it is actively deployed. Accuracy depends on method, records and literacy, administrative reach, cooperation, conflict and migration, and decays with time since the count; counts by different authorities can disagree. Taxation, conscription, planning and AI use counts, not truth. Extends P1, X13, K63, N8. Owner: OS-INFO.
- **P38 Administrative units below the province (owner-confirmed 26 Nov 2026).** Municipalities, city wards and boroughs, village councils and special-purpose areas as jurisdiction relations (I19 administration) over geography at any level of the nesting, with dated versions and ancestry; created, merged, split and redrawn by the governing authority; need not match physical districts; can carry authority, records, taxes and services; never create city capacity. A city can be divided among several units, including units held by rival factions (S34). Extends P32, S15, N12. Owner: OS-POLITY, geometry identities in OS-IDENT.
- **R31 Shared construction project lifecycle.** One lifecycle for every actor, from inspection through commissioning, operation, maintenance, damage, retirement, demolition and reuse; real inputs consumed once; no instant completion; deterministic allocation of shared resources; commissioning prerequisites separate from physical completion. Extends R8, R28, N7. Owner: OS-PROD.
- **R32 Building families, significant assets and landmarks.** Aggregated fabric versus individual assets; classes from generic fabric to great works; branching families; compounds without double counting; persistent identity, naming, lineage, reuse and heritage; benefits only through real mechanisms. Extends P7, X3, N8, S25. Owner: OS-PROD.
- **R33 Multidimensional capacity and functional proximity.** Site, floor, housing, utility, transport and logistics, workforce, materials and construction, finance, administrative, environmental, legal and technical capacity as separate quantities; proximity effects through accessibility, networks, catchments and exposure; network and distance effects beyond contiguity, separate from fixed typed adjacency (P8); a pair of places counts through one channel only (revised 26 Nov 2026). Owner: OS-SETTLE.
- **I19 Land and development rights.** Ownership, lease, concession, access, use, development, operation, administration, taxation, sovereignty, influence and control as separate relations with full terms and provenance; exclusive versus nonexclusive validation; grants never confer sovereignty or direct control. Extends I4, I5, R29, E17. Owner: OS-LEDGER.
- **I20 Development proposals, competition and partnerships.** Proposal records, resolution through existing allocation, market and political processes on one snapshot, deterministic tie-breaks, no simultaneous exclusive grants; shared infrastructure capacity reservation and cost-sharing through existing contracts; privatisation, nationalisation, concessions, bankruptcy and expropriation reconciled. Extends D19, E8, I3, I16. Owner: OS-POLITY for allocation decisions, contracts in OS-DIPLO and OS-MARKET as today.
- **A17 Urban delegation and development packages.** Direct, Guided and Autonomous management of lawfully delegable development; standing orders with priorities, spending limits, constraints, expiry, triggers and exception policy; packages that generate real component projects; same rules and information for AI. Extends A16, M42, T2, T10. Owner: OS-AI (orders through OS-TIME).
- **U12 The Living City view (WS-A1/V1).** Full-screen dismissible view inside WS-A1 with seven tabs, the district register, inspector, catalogue, proposals, partners, dependencies and actions, linked to existing dossiers and analyses. Owner: OS-UI (owns nothing authoritative).
- **U13 Cartographic presentation.** One canonical geometry and a separate display transform (candidates evaluated, ADR-050), four semantic zoom levels, six compositions, eighteen filter families, twelve expressions, knowledge-gated display and picking, Living Engraved Cartography art rules, tiled GPU rendering with LOD. Extends U4, U5. Owner: OS-UI.
- **U14 Urban map layers, selection and planning preview.** Urban filters, city, district and building selection states, the boundary and hatch grammar, landmark labels, and planning previews that never change state. Owner: OS-UI.

**Dependencies (inputs to Pass 2b).** Housing and jobs with migration and wages; construction with credit and material demand; new land use with food, habitat, runoff and pollution; transport with access, congestion and land value; utility capacity with feasible development and output; charters with project authority; service access with health, education and competence; population and revenue with local service demand; milestones with chronicle and scoring only through existing paths. Loops are checked for strongly connected components and given a bounded solver or a justified lag.

## S. Next pass

1. Assign every entry to a dependency layer and list what it needs and feeds.
2. Build the interaction matrix (system by system).
3. Flag the entries that carry the most coupling, since they need the most careful specification.
4. Mark mechanics with real-world calibration cases for the validation suite.

**Proposed matrix inputs for institutional research.** These feed the dependency and interaction matrix passes and are not final dependency-layer assignments.

| System | Supplies to research | Receives from research |
| --- | --- | --- |
| Population | Educated and specialised labour | Education, health and productivity improvements |
| Economy | Funding, capital, wages, incentives | New industries and productivity |
| Resources | Materials, experimental inputs | New extraction and processing methods |
| Energy | Power, cooling, operating capacity | New generation and storage technologies |
| Infrastructure | Laboratories, networks, compute | Improved infrastructure capabilities |
| Institutions | Governance, competence, organisation | New mandates and institutional forms |
| Government | Laws, funding, permissions, restrictions | Scientific advice and strategic capabilities |
| Characters | Leadership, expertise, mentorship | Reputation, careers, historical significance |
| Diplomacy | Collaboration, treaties, restrictions | Scientific influence and dependencies |
| Intelligence | Acquired knowledge, security | New intelligence capabilities |
| Military | Research demand, funding, doctrine | Technologies and operational capabilities |
| History | Prior discoveries, institutional lineage | Scientific events and historical records |
| Special projects | Objectives, pooled resources | Research milestones and project competence |
