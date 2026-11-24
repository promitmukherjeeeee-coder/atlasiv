# ADR-004: Authoritative state uses integers and per-quantity scaled fixed point, declared with unit and scale in the data schema; 128-bit intermediates

- **Alternatives considered:** Single 32.32 format; floating point everywhere
- **Reason and evidence:** Bitwise replay across machines; order-independent parallel sums; a single 32.32 format overflows above about 2.1 billion
- **Status:** Delegated, Pass 1 item for owner confirmation
