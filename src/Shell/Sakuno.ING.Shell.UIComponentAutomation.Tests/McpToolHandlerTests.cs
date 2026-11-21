using Avalonia.Automation;
using ModelContextProtocol.Protocol;

namespace Sakuno.ING.Shell.UIComponentAutomation.Tests;

public class McpToolHandlerTests
{
    [Fact]
    public async Task HandlersReturnTypedResultsAndRecoverableCoreErrors()
    {
        var catalog = new UiComponentScenarioCatalog();
        using var coordinator = new UiComponentSessionCoordinator(catalog);
        var tools = new UiComponentAutomationTools(catalog, coordinator);

        var list = tools.ListScenarios();
        Assert.False(list.IsError);
        Assert.Equal(2, list.StructuredContent!.Value.GetProperty("scenarios").GetArrayLength());

        Assert.False((await tools.OpenScenario(UiComponentScenarioCatalog.FleetTabWithFleets)).IsError);
        Assert.Equal("1", (await tools.InspectComponent()).StructuredContent!.Value
            .GetProperty("state").GetProperty("SelectedFleetId").GetString());
        Assert.False((await tools.Click("FleetTab_2")).IsError);
        Assert.Equal("2", (await tools.InspectComponent()).StructuredContent!.Value
            .GetProperty("state").GetProperty("SelectedFleetId").GetString());

        var capture = await tools.CaptureScreenshot();
        Assert.False(capture.IsError);
        var image = Assert.IsType<ImageContentBlock>(Assert.Single(capture.Content));
        Assert.Equal("image/png", image.MimeType);
        Assert.Equal([137, 80, 78, 71, 13, 10, 26, 10], image.DecodedData.ToArray()[..8]);
        Assert.Equal(800, capture.StructuredContent!.Value.GetProperty("width").GetInt32());

        Assert.False((await tools.ResetScenario()).IsError);
        Assert.Equal("1", (await tools.InspectComponent()).StructuredContent!.Value
            .GetProperty("state").GetProperty("SelectedFleetId").GetString());
        Assert.False((await tools.CloseScenario()).IsError);

        var noActive = await tools.InspectComponent();
        Assert.True(noActive.IsError);
        Assert.Equal("no_active_scenario", noActive.StructuredContent!.Value.GetProperty("code").GetString());
        Assert.Equal("No UI Component Scenario is active.", noActive.StructuredContent!.Value.GetProperty("message").GetString());

        var unknown = await tools.OpenScenario("Unknown");
        Assert.True(unknown.IsError);
        Assert.Equal("scenario_not_found", unknown.StructuredContent!.Value.GetProperty("code").GetString());
        Assert.Equal("Unknown", unknown.StructuredContent!.Value.GetProperty("details").GetProperty("scenarioId").GetString());
        Assert.False(tools.ListScenarios().IsError);
    }

    [Fact]
    public async Task InputTextHandlerUsesTypedParametersAndReturnsUpdatedInspection()
    {
        var definition = new ScenarioDefinition(
            "Tests/Handlers/TextInput",
            "TextBox",
            createContent: _ =>
            {
                var textBox = new TextBox();
                AutomationProperties.SetAutomationId(textBox, "Input");
                return new(textBox, () => new Dictionary<string, object?>());
            });
        var catalog = new UiComponentScenarioCatalog(definition);
        using var coordinator = new UiComponentSessionCoordinator(catalog);
        var tools = new UiComponentAutomationTools(catalog, coordinator);
        await tools.OpenScenario(definition.Id);

        var input = await tools.InputText("Input", "MCP text");

        Assert.False(input.IsError);
        Assert.Equal("ok", input.StructuredContent!.Value.GetProperty("status").GetString());
        var inspection = await tools.InspectComponent();
        Assert.Equal("MCP text", inspection.StructuredContent!.Value.GetProperty("controls")[0].GetProperty("text").GetString());
    }
}
