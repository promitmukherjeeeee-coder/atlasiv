using System.IO;
using Atlasiv.Core.Tests.Numerics;
using Xunit;

namespace Atlasiv.Core.Tests.Randomness;

/// <summary>
/// Hard rule 3: no System.Random. The ban itself is enforced by the compiler: every project
/// runs Microsoft's banned-API analyzer over BannedSymbols.txt, and any use of a listed
/// symbol is error RS0030, which fails the build (ENG-008). These tests keep that wiring in place.
/// </summary>
public sealed class NoSystemRandomTests
{
    [Theory]
    [InlineData("T:System.Random;")]
    [InlineData("T:System.Security.Cryptography.RandomNumberGenerator;")]
    [InlineData("M:System.Guid.NewGuid;")]
    public void The_banned_symbol_list_names_every_unseeded_random_source(string entry)
    {
        var banned = File.ReadAllText(Path.Combine(NoFloatingPointTests.RepositoryRoot(), "BannedSymbols.txt"));

        Assert.Contains(entry, banned, System.StringComparison.Ordinal);
    }

    [Fact]
    public void Every_project_runs_the_banned_api_analyzer()
    {
        // Directory.Build.props applies to every project in the repository.
        var props = File.ReadAllText(Path.Combine(NoFloatingPointTests.RepositoryRoot(), "Directory.Build.props"));

        Assert.Contains("Microsoft.CodeAnalysis.BannedApiAnalyzers", props, System.StringComparison.Ordinal);
        Assert.Contains("BannedSymbols.txt", props, System.StringComparison.Ordinal);
    }
}
