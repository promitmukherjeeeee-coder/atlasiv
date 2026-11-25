# Domain Docs

How the engineering skills should consume this repo's domain documentation when exploring the codebase.

## Before exploring, read these

- **`GLOSSARY.md`** at the repo root.
- **`docs/07_architectural_decision_registry.md`**: the owner's Architectural Decision Registry (ADR-001 onward). Read-only; read the decisions that touch the area you're about to work in.
- **`docs/decisions-log.md`**: engineering decisions taken while building (ENG-001 onward). New engineering decisions are added here, never as separate ADR files.

## Numbering

`ADR-` numbers belong only to the owner's registry. Engineering decisions use `ENG-` numbers in the decisions log (ENG-001). Never create a `docs/adr/` folder or invent an `ADR-` number.

## Use the glossary's vocabulary

When your output names a domain concept (in an issue title, a refactor proposal, a hypothesis, a test name), use the term as defined in `GLOSSARY.md`. Don't drift to synonyms the glossary explicitly avoids.

If the concept you need isn't in the glossary yet, that's a signal: either you're inventing language the project doesn't use (reconsider) or there's a real gap (note it for `/domain-modeling`).

## Flag decision conflicts

If your output contradicts a registry ADR or a decisions-log entry, surface it explicitly rather than silently overriding:

> _Contradicts ENG-003 (build, test and CI settings), but worth reopening because…_
