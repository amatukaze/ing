using System.Xml.Linq;

namespace Sakuno.ING.Shell.UIComponentAutomation.Tests;

public class ProjectStructureTests
{
    [Fact]
    public void ProjectsAreInSolutionAndProductionDoesNotReferenceAutomation()
    {
        var solution = File.ReadAllText(Path.Combine(Repository.Root, "ing.slnx"));
        Assert.Contains("Sakuno.ING.Shell.UIComponentAutomation/Sakuno.ING.Shell.UIComponentAutomation.csproj", solution);
        Assert.Contains("Sakuno.ING.Shell.UIComponentAutomation.Tests/Sakuno.ING.Shell.UIComponentAutomation.Tests.csproj", solution);

        var productionProjects = Directory.EnumerateFiles(Repository.Root, "*.csproj", SearchOption.AllDirectories)
            .Where(path => !path.Contains("UIComponentAutomation", StringComparison.Ordinal));
        foreach (var project in productionProjects)
        {
            Assert.DoesNotContain(File.ReadAllText(project), "UIComponentAutomation", StringComparison.Ordinal);
        }
    }

    [Fact]
    public void AutomationDependenciesUseCentralPackageManagement()
    {
        var packages = XDocument.Load(Path.Combine(Repository.Root, "Directory.Packages.props"));
        var versions = packages.Descendants("PackageVersion")
            .ToDictionary(element => element.Attribute("Include")!.Value, element => element.Attribute("Version")!.Value);

        Assert.Equal("$(AvaloniaVersion)", versions["Avalonia.Headless"]);
        Assert.Equal("$(AvaloniaVersion)", versions["Avalonia.Skia"]);
        Assert.False(string.IsNullOrWhiteSpace(versions["ModelContextProtocol"]));

        var project = XDocument.Load(Path.Combine(
            Repository.Root,
            "src",
            "Shell",
            "Sakuno.ING.Shell.UIComponentAutomation",
            "Sakuno.ING.Shell.UIComponentAutomation.csproj"));
        Assert.All(project.Descendants("PackageReference"), reference => Assert.Null(reference.Attribute("Version")));
        Assert.DoesNotContain(project.Descendants("ProjectReference"), reference =>
            reference.Attribute("Include")!.Value.Contains("Sakuno.ING.Shell\\Sakuno.ING.Shell.csproj", StringComparison.Ordinal));
    }
}
