using Avalonia.Automation;
using Avalonia.Controls;

namespace Sakuno.ING.Shell.UIComponentAutomation.Tests;

public class ControlInteractionTests
{
    [Fact]
    public void MissingAndAmbiguousIdentifiersDoNotInteract()
    {
        var firstClicks = 0;
        var secondClicks = 0;
        var uniqueClicks = 0;
        var definition = new ScenarioDefinition(
            "Tests/Buttons/Identifiers",
            "StackPanel",
            createContent: _ =>
            {
                var root = new StackPanel();
                root.Children.Add(CreateButton("Duplicate", () => firstClicks++));
                root.Children.Add(CreateButton("Duplicate", () => secondClicks++));
                var unique = new Button { Name = "UniqueName", Content = "Unique" };
                unique.Click += (_, _) => uniqueClicks++;
                root.Children.Add(unique);
                return new(root, () => new Dictionary<string, object?>());
            });
        using var session = UiComponentSession.Open(definition);

        Assert.Equal(ControlInteractionStatus.NotFound, session.Click("Missing").Status);
        Assert.Equal(ControlInteractionStatus.Ambiguous, session.Click("Duplicate").Status);
        Assert.Equal(0, firstClicks);
        Assert.Equal(0, secondClicks);

        Assert.Equal(ControlInteractionStatus.Success, session.Click("UniqueName").Status);
        Assert.Equal(1, uniqueClicks);
    }

    [Fact]
    public void InspectionDepthAndNodeCountAreBounded()
    {
        using var session = new UiComponentScenarioCatalog().Open(UiComponentScenarioCatalog.FleetTabWithFleets);

        var result = session.Inspect(maxDepth: 1, maxNodes: 1);

        Assert.Single(result.Controls);
        Assert.Equal(0, result.Controls[0].Depth);
        Assert.True(result.IsTruncated);
    }

    private static Button CreateButton(string automationId, Action clicked)
    {
        var button = new Button { Content = automationId };
        AutomationProperties.SetAutomationId(button, automationId);
        button.Click += (_, _) => clicked();
        return button;
    }
}
