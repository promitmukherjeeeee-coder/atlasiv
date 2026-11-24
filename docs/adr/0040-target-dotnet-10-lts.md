# ADR-040: Target .NET 10 (LTS)

- **Status:** Proposed (Pass 1, J0). Awaiting owner review.
- **Context:** Spec section 12 and PASS1_PLAN J0 ask for "the long-term-support version that the current stable Godot 4.x .NET build supports", with the source checked. ADR-001 puts the simulation core in a headless .NET library; ADR-002 puts the client in Godot 4 with C#.

## Decision

Every project targets `net10.0`. The SDK is pinned in `global.json` to the 10.0 band (`10.0.100`, rolling forward to the latest installed 10.0 feature band, no previews).

## Evidence (checked 24 Nov 2026)

- **.NET 8 is out of support.** .NET 8 (LTS) and .NET 9 (STS) both reached end of support on 10 Nov 2026. .NET 10 is the current LTS release, supported until Nov 2028.
  Sources: Microsoft, ".NET and .NET Core Support Policy", <https://dotnet.microsoft.com/platform/support/policy/dotnet-core>; .NET Blog, ".NET 8 and .NET 9 end of support", <https://devblogs.microsoft.com/dotnet/dotnet-8-9-end-of-support/>.
- **Godot 4.8 requires .NET 10.** Godot 4.8 raises the minimum .NET version for C# projects from 8 to 10 (GH-123738, "Upgrade packages and minimum TFM required to net10.0").
  Sources: "Dev snapshot: Godot 4.8 dev 7", <https://godotengine.org/article/dev-snapshot-godot-4-8-dev-7/>; "Dev snapshot: Godot 4.8 beta 4", <https://godotengine.org/article/dev-snapshot-godot-4-8-beta-4/>.
- **Godot 4.7 also works with .NET 10.** On the day of this record, the newest stable Godot is 4.7.2 (18 Aug 2026), and 4.8 is at beta 4 (18 Nov 2026), with stable expected in Q4 2026. Godot's C# packages set only a *minimum* target framework, and projects may target a newer one, so a .NET 10 library works with 4.7 as well as 4.8.
  Sources: Godot download archive, <https://godotengine.org/download/archive/>; Godot release policy, <https://docs.godotengine.org/en/stable/about/release_policy.html>; Godot docs, "C# basics" ("The version targeted by Godot's C# packages is only the minimum version that your project can target, but you are always free to target a newer .NET version"), <https://docs.godotengine.org/en/latest/tutorials/scripting/c_sharp/c_sharp_basics.html>.

## Alternatives considered

- **.NET 8.** Rejected: out of support since 10 Nov 2026, and below Godot 4.8's minimum.
- **.NET 9.** Rejected: a short-term release, also out of support since 10 Nov 2026.
- **.NET 11 (preview).** Rejected: not released, not LTS, and not supported by any stable Godot.

## Consequences

- Pass 1 does not touch Godot, so the 4.7 to 4.8 question does not block any Pass 1 job. When the client starts, it uses Godot 4.8 or later (4.7 also works with this choice).
- The next LTS move (to .NET 12) is a later decision, expected before Nov 2028.

## Related

ADR-001 (core in .NET, headless), ADR-002 (Godot 4 with C#), ADR-041 (build and CI settings).
