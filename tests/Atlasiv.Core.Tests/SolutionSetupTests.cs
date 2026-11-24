using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
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
    public void Core_library_does_not_load_any_Godot_assembly()
    {
        var godotReferences = CoreAssembly.GetReferencedAssemblies()
            .Select(a => a.Name ?? string.Empty)
            .Where(IsGodot)
            .ToArray();

        Assert.Empty(godotReferences);
    }

    [Fact]
    public void Core_library_has_no_Godot_package_anywhere_in_its_dependency_tree()
    {
        // The deps.json written next to the test binaries lists every package and
        // project reference, used or not, with their transitive dependencies.
        var depsPath = Path.Combine(AppContext.BaseDirectory, "Atlasiv.Core.Tests.deps.json");
        using var deps = JsonDocument.Parse(File.ReadAllText(depsPath));
        var target = deps.RootElement.GetProperty("targets").EnumerateObject().Single().Value;

        var closure = DependencyClosure(target, "Atlasiv.Core");

        Assert.Contains("Atlasiv.Core", closure);
        Assert.DoesNotContain(closure, IsGodot);
    }

    [Fact]
    public void Tests_run_under_the_invariant_culture()
    {
        Assert.Equal(CultureInfo.InvariantCulture.Name, CultureInfo.CurrentCulture.Name);
        Assert.Equal(CultureInfo.InvariantCulture.Name, CultureInfo.CurrentUICulture.Name);
        Assert.Equal("1234.5", 1234.5m.ToString(CultureInfo.CurrentCulture));
    }

    [Property]
    public bool FsCheck_is_wired_up(int a, int b) => (long)a + b == (long)b + a;

    private static bool IsGodot(string name) =>
        name.StartsWith("Godot", StringComparison.OrdinalIgnoreCase);

    private static SortedSet<string> DependencyClosure(JsonElement target, string root)
    {
        var entries = target.EnumerateObject()
            .ToDictionary(p => p.Name.Split('/')[0], p => p.Value, StringComparer.Ordinal);
        var seen = new SortedSet<string>(StringComparer.Ordinal);
        var pending = new Stack<string>();
        pending.Push(root);

        while (pending.Count > 0)
        {
            var name = pending.Pop();
            if (!seen.Add(name) || !entries.TryGetValue(name, out var entry))
            {
                continue;
            }

            if (entry.TryGetProperty("dependencies", out var children))
            {
                foreach (var child in children.EnumerateObject())
                {
                    pending.Push(child.Name);
                }
            }
        }

        return seen;
    }
}
