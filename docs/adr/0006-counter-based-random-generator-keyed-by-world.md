# ADR-006: Counter-based random generator keyed by world seed, turn, phase, system, entity and draw index; System.Random excluded

- **Alternatives considered:** Stateful generators per system
- **Reason and evidence:** Separated reproducible streams; unrelated changes do not shift history
- **Status:** Delegated, Pass 1 item for owner confirmation
