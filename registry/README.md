# ATLASIV registries (draft 0.1.0, 24 Nov 2026)

Machine-readable companions to the master documents. These four files go into the repository with Pass 2. All of them are drafts: the documents remain the source of truth until Pass 2 confirms the files.

| File | What it is | Engineering document | Confirmed in |
| --- | --- | --- | --- |
| `interface_registry.yaml` | Every interface element and ID: 4 levels, 8 groups, 28 workspaces, 10 showcase views, 7 dossier types, 6 analysis types, 5 global systems, 6 analytical patterns, the 13-item menu, linked mechanics per workspace | G (Interface Architecture Registry) | Pass 2, completed in Pass 10 |
| `mechanic_registry.yaml` | All 407 mechanic IDs with name, series, owner system, dependency layer and the links each entry declares in the documents | Input to D | Pass 2b |
| `state_ownership.yaml` | 28 owner systems, the state each one alone may write, and the ownership rules | Seed of B (World State and Data Schema) | Pass 2 |
| `system_dependency_map.yaml` | System-to-system flows taken from the documents (feedback loops, research matrix, knowledge dependencies) plus flows computed from declared mechanic links, and the feedback cycles they form | Draft of D at system level | Pass 2b |

What is not in these files yet, because it needs each mechanic's 21-field specification first: variable-level inputs and outputs, equations, update timing for each link, actions and access rules per workspace, cross-links (XL-), and interface validation tests.

There is no modding, plugin or extension interface. Only the source code is published.
