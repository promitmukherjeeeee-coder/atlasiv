# ADR-038: Twenty-eight owner systems with single-writer ownership

Group in the original registry: Additions, 24 Nov 2026 (knowledge and historiography amendment, section 59). See [README.md](README.md) for the group note.

Every authoritative variable has exactly one owner system; other systems change it only through declared events, orders or ledger transactions; knowledge (OS-KNOW), actor belief (OS-INFO) and capability (OS-PROD) are separate owners. Alternatives: per-mechanic owners; shared mutable state. Reason: determinism, conservation and causal tracing. Consequences: the system-level dependency draft shows one coupled core of 17 systems once phase-lagged edges are removed, so Pass 2b must assign each cross-system link a timing (same stage, next phase or turn end) and Pass 3 a solver contract where a loop closes inside one update. Status: proposed, confirmed in Pass 2.
