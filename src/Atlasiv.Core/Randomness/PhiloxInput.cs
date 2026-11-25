namespace Atlasiv.Core.Randomness;

/// <summary>The Philox counter and key for one draw, and which of the four output words it takes.</summary>
/// <param name="Counter">The 256-bit counter.</param>
/// <param name="Key0">Key word 0.</param>
/// <param name="Key1">Key word 1.</param>
/// <param name="Word">The output word used, 0 to 3.</param>
public readonly record struct PhiloxInput(Block4x64 Counter, ulong Key0, ulong Key1, int Word);
