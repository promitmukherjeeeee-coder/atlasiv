# ADR-029: Rendering separated from simulation; offline baseline; optional language-model renderers

Group in the original registry: Additions, 24 Nov 2026 (knowledge and historiography amendment, section 59). See [README.md](README.md) for the group note.

A deterministic offline renderer is always available; optional language-model renderers are replaceable, validated against claims, fall back on failure, and never change state hashes (X25). Alternatives: required online model. Status: owner-confirmed; renderer selection proposed for Pass 10.
