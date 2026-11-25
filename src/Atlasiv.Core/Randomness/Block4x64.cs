namespace Atlasiv.Core.Randomness;

/// <summary>Four 64-bit words: a Philox counter or one Philox output block.</summary>
/// <param name="W0">Word 0.</param>
/// <param name="W1">Word 1.</param>
/// <param name="W2">Word 2.</param>
/// <param name="W3">Word 3.</param>
public readonly record struct Block4x64(ulong W0, ulong W1, ulong W2, ulong W3)
{
    /// <summary>Word <paramref name="index"/> (0 to 3; only the low two bits are used).</summary>
    public ulong this[int index] => (index & 3) switch
    {
        0 => W0,
        1 => W1,
        2 => W2,
        _ => W3,
    };
}
