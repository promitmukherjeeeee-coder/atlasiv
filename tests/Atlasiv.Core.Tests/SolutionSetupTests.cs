using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FsCheck.Xunit;
using Xunit;

namespace Atlasiv.Core.Tests;

/// <summary>
/// J0 checks: the solution is wired up, the library is engine-free, and
/// tests run under the invariant culture.
/// </summary>
public sealed class SolutionSetupTests
{
    private static readonly Assembly CoreAssembly = typeof(CoreAssemblyMarker).Assembly;

    [Fact]
    public void Placeholder_test_runs_against_the_core_library()
    {
        Assert.Equal("Atlasiv.Core", CoreAssembly.GetName().Name);
    }

    [Fact]
    public void Core_library_has_no_Godot_reference()
    {
        var godotReferences = CoreAssembly.GetReferencedAssemblies()
            .Where(a => a.Name is not null && a.Name.StartsWith("Godot", StringComparison.OrdinalIgnoreCase))
            .Select(a => a.Name)
            .ToArray();

        Assert.Empty(godotReferences);
    }

    [Fact]
    public void Tests_run_under_the_invariant_culture()
    {
        Assert.Equal(CultureInfo.InvariantCulture.Name, CultureInfo.CurrentCulture.Name);
        Assert.Equal(CultureInfo.InvariantCulture.Name, CultureInfo.CurrentUICulture.Name);
        Assert.Equal("1234.5", 1234.5m.ToString(CultureInfo.CurrentCulture));
    }

    [Property]
    public bool FsCheck_is_wired_up(long a, long b) => unchecked(a + b) == unchecked(b + a);
}
