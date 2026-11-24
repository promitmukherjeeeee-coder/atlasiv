using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace Atlasiv.Core.Tests.Numerics;

/// <summary>
/// Hard rules 2 and 4: the simulation library contains no floating-point types or literals
/// and calls no library math functions (Math, MathF), because their results can differ
/// between platforms and runtimes. This scans every C# source file in src/Atlasiv.Core,
/// ignoring comments and string literals.
/// </summary>
public sealed partial class NoFloatingPointTests
{
    [Fact]
    public void The_simulation_library_has_no_floating_point_or_library_math()
    {
        var sourceRoot = Path.Combine(RepositoryRoot(), "src", "Atlasiv.Core");
        var files = Directory.GetFiles(sourceRoot, "*.cs", SearchOption.AllDirectories)
            .Where(f => !f.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Any(part => part is "bin" or "obj"))
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.Contains(files, f => f.EndsWith("FixedMath.cs", StringComparison.Ordinal)); // the scan sees the numerics

        var findings = files
            .SelectMany(file => File.ReadAllLines(file).Select((line, index) => (file, number: index + 1, line)))
            .Where(l => Violates(l.line))
            .Select(l => $"{Path.GetRelativePath(sourceRoot, l.file)}:{l.number}: {l.line.Trim()}")
            .ToArray();

        Assert.True(findings.Length == 0, "Floating point or library math found:\n" + string.Join("\n", findings));
    }

    [Theory]
    [InlineData("double x = 1;")]
    [InlineData("float y;")]
    [InlineData("var h = (Half)1;")]
    [InlineData("var e = Math.Exp(z);")]
    [InlineData("var e = System.MathF.Sqrt(z);")]
    [InlineData("var r = 1.5;")]
    [InlineData("var r = 2e3;")]
    [InlineData("var r = 3f;")]
    [InlineData("var r = 4d;")]
    [InlineData("var d = Double.Parse(s);")]
    [InlineData("var d = BigInteger.Log(x);")]
    public void The_scan_catches_floating_point(string line)
    {
        Assert.True(Violates(line));
    }

    [Theory]
    [InlineData("var e = FixedMath.Exp(x); // a double check: Math.Exp(1.5) is never used")]
    [InlineData("throw new X(\"no double here, 1.5\");")]
    [InlineData("private static readonly Int128 Ln2 = new(0xB172_17F7UL, 0xD1CF_79AB_C9E3_B398UL);")]
    [InlineData("var w = WideMath.Magnitude(v);")]
    [InlineData("/// <summary>Returns a double-checked value.</summary>")]
    public void The_scan_ignores_comments_strings_and_hex(string line)
    {
        Assert.False(Violates(line));
    }

    private static bool Violates(string line)
    {
        var code = LineComment().Replace(StringLiteral().Replace(line, "\"\""), string.Empty);
        code = HexLiteral().Replace(code, "0");
        return Forbidden().IsMatch(code);
    }

    private static string RepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Atlasiv.slnx")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName ?? throw new InvalidOperationException("Atlasiv.slnx not found above the test binaries.");
    }

    [GeneratedRegex("@?\\$?\"(?:[^\"\\\\]|\\\\.)*\"")]
    private static partial Regex StringLiteral();

    [GeneratedRegex("//.*$")]
    private static partial Regex LineComment();

    [GeneratedRegex("\\b0[xXbB][0-9A-Fa-f_]+[uUlL]*\\b")]
    private static partial Regex HexLiteral();

    // Floating-point type names, library math classes, BigInteger's double-valued logarithms,
    // and floating-point literals (1.5, 2e3, 3f, 4d, 5m).
    [GeneratedRegex(
        "\\b(float|double|decimal|Half|Single|Double|Decimal|Math|MathF)\\b"
        + "|\\bBigInteger\\.Log(10)?\\b"
        + "|\\b\\d[\\d_]*\\.\\d"
        + "|\\b\\d[\\d_]*(\\.\\d[\\d_]*)?[eE][+-]?\\d"
        + "|\\b\\d[\\d_]*[fFdDmM]\\b")]
    private static partial Regex Forbidden();
}
