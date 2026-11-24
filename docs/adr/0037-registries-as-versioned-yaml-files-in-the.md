# ADR-037: Registries as versioned YAML files in the repository

Group in the original registry: Additions, 24 Nov 2026 (knowledge and historiography amendment, section 59). See [README.md](README.md) for the group note.

interface\_registry.yaml, mechanic\_registry.yaml, state\_ownership.yaml and system\_dependency\_map.yaml live in the repository, are validated by schema, and generate the interface chart and the dependency checks. Alternatives: registries kept only in the documents; a database. Reason: diffable, reviewable, readable by code and by Claude Code. Status: delegated; draft 0.1.0 created 24 Nov 2026, committed in Pass 2.
