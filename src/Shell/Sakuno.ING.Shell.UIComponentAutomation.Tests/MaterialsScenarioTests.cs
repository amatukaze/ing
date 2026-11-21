using Sakuno.ING.ViewModels.Homeport;
using SkiaSharp;

namespace Sakuno.ING.Shell.UIComponentAutomation.Tests;

public class MaterialsScenarioTests
{
    private static readonly IReadOnlyDictionary<string, int> Expected = new Dictionary<string, int>
    {
        ["Fuel"] = 1101,
        ["Bullet"] = 1202,
        ["Steel"] = 1303,
        ["Bauxite"] = 1404,
        ["InstantBuild"] = 15,
        ["InstantRepair"] = 26,
        ["Development"] = 37,
        ["Improvement"] = 48,
    };

    [Fact]
    public void MaterialsResolvesViewModelThroughInheritedContainerAndRendersValues()
    {
        using var session = new UiComponentScenarioCatalog().Open(UiComponentScenarioCatalog.MaterialsWithData);
        var view = Assert.IsType<Views.Overall.Materials>(session.Root);
        var viewModel = UiComponentHost.Dispatch(() => Assert.IsType<MaterialsViewModel>(view.DataContext));
        Assert.Same(session.Scope.Resolve<MaterialsViewModel>(), viewModel);

        var inspection = session.Inspect();
        Assert.Equal(UiComponentScenarioCatalog.MaterialsWithData, inspection.ScenarioId);
        foreach (var (name, value) in Expected)
        {
            Assert.Equal(value, inspection.State[name]);
            Assert.Contains(inspection.Controls, control =>
                control.Type == typeof(TextBlock).FullName
                && control.Text == value.ToString()
                && control.IsVisible);
        }

        var capture = session.CaptureScreenshot();
        Assert.Equal(UiComponentScenarioCatalog.MaterialsWithData, capture.ScenarioId);
        Assert.Equal(800, capture.Width);
        Assert.Equal(450, capture.Height);
        Assert.Equal([137, 80, 78, 71, 13, 10, 26, 10], capture.Png[..8]);
        using var bitmap = SKBitmap.Decode(capture.Png);
        Assert.NotNull(bitmap);
        Assert.True(bitmap.Pixels.Distinct().Count() > 1);

        Directory.CreateDirectory(Repository.TestResults);
        File.WriteAllBytes(Path.Combine(Repository.TestResults, "Materials-WithData.png"), capture.Png);
    }
}
