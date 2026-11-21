using Avalonia.VisualTree;
using SkiaSharp;
using Sakuno.ING.Shell.Views.Overall;

namespace Sakuno.ING.Shell.UIComponentAutomation.Tests;

public class FleetTabScenarioTests
{
    [Fact]
    public void OpenInspectClickInspectCaptureUsesRealFleetSelection()
    {
        using var session = new UiComponentScenarioCatalog().Open(UiComponentScenarioCatalog.FleetTabWithFleets);
        Assert.IsType<FleetTab>(session.Root);

        var before = session.Inspect();
        var button = Assert.Single(before.Controls, control => control.AutomationId == "FleetTab_2");
        Assert.Equal(typeof(Button).FullName, button.Type);
        Assert.True(button.IsVisible);
        Assert.True(button.IsEnabled);
        Assert.True(button.Bounds.Width > 0);
        Assert.True(button.Bounds.Height > 0);
        Assert.Contains(before.Controls, control => control.Type == typeof(TextBlock).FullName && control.Text == "2");
        Assert.Equal("1", before.State["SelectedFleetId"]);

        Assert.Equal(ControlInteractionStatus.NotFound, session.Click("FleetTab_Missing").Status);
        Assert.Equal("1", session.Inspect().State["SelectedFleetId"]);
        Assert.Equal(ControlInteractionStatus.Success, session.Click("FleetTab_2").Status);

        var after = session.Inspect();
        Assert.Equal("2", after.State["SelectedFleetId"]);

        var capture = session.CaptureScreenshot();
        Assert.Equal(UiComponentScenarioCatalog.FleetTabWithFleets, capture.ScenarioId);
        Assert.Equal(800, capture.Width);
        Assert.Equal(450, capture.Height);
        AssertNonEmptyPng(capture);

        Directory.CreateDirectory(Repository.TestResults);
        File.WriteAllBytes(Path.Combine(Repository.TestResults, "FleetTab-WithFleets.png"), capture.Png);
    }

    private static void AssertNonEmptyPng(ScreenshotCapture capture)
    {
        Assert.True(capture.Png.Length > 8);
        Assert.Equal([137, 80, 78, 71, 13, 10, 26, 10], capture.Png[..8]);

        using var bitmap = SKBitmap.Decode(capture.Png);
        Assert.NotNull(bitmap);
        Assert.Equal(capture.Width, bitmap.Width);
        Assert.Equal(capture.Height, bitmap.Height);
        Assert.True(bitmap.Pixels.Distinct().Count() > 1);
        Assert.Contains(bitmap.Pixels, pixel => pixel.Alpha > 0);
    }
}
