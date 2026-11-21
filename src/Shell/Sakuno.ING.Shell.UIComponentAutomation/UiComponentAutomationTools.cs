using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace Sakuno.ING.Shell.UIComponentAutomation;

[McpServerToolType]
public sealed class UiComponentAutomationTools
{
    private readonly UiComponentScenarioCatalog _catalog;
    private readonly UiComponentSessionCoordinator _coordinator;

    public UiComponentAutomationTools(
        UiComponentScenarioCatalog catalog,
        UiComponentSessionCoordinator coordinator)
    {
        _catalog = catalog;
        _coordinator = coordinator;
    }

    [McpServerTool(Name = "list_scenarios", ReadOnly = true, UseStructuredContent = true, OutputSchemaType = typeof(ScenarioListResult))]
    [Description("List the explicitly registered UI Component Scenarios.")]
    public CallToolResult ListScenarios() =>
        McpToolResults.Success(new ScenarioListResult(_catalog.Scenarios.Select(scenario =>
            new ScenarioToolInfo(scenario.Id, scenario.ComponentName, scenario.Width, scenario.Height)).ToArray()));

    [McpServerTool(Name = "open_scenario", UseStructuredContent = true, OutputSchemaType = typeof(ScenarioOperationResult))]
    [Description("Open a registered scenario, replacing the active scenario only after successful construction and layout.")]
    public Task<CallToolResult> OpenScenario(
        [Description("Stable scenario identifier returned by list_scenarios.")] string scenarioId) =>
        McpToolResults.Execute(async () =>
        {
            await _coordinator.OpenAsync(scenarioId);
            return McpToolResults.Success(new ScenarioOperationResult(scenarioId));
        });

    [McpServerTool(Name = "inspect_component", ReadOnly = true, UseStructuredContent = true, OutputSchemaType = typeof(InspectionResult))]
    [Description("Inspect a bounded visual-tree fragment and the scenario's read-only state snapshot.")]
    public Task<CallToolResult> InspectComponent(
        [Description("Optional unique AutomationId or Name of the inspection root.")] string? rootIdentifier = null,
        [Description("Maximum visual-tree depth, from 0 through 8."), Range(0, 8)] int maxDepth = 4,
        [Description("Maximum number of controls, from 1 through 200."), Range(1, 200)] int maxNodes = 100) =>
        McpToolResults.Execute(async () =>
            McpToolResults.Success(await _coordinator.InspectAsync(rootIdentifier, maxDepth, maxNodes)));

    [McpServerTool(Name = "click", UseStructuredContent = true, OutputSchemaType = typeof(OperationResult))]
    [Description("Click a uniquely identified visible and enabled control using real headless input.")]
    public Task<CallToolResult> Click(
        [Description("Unique AutomationId or Name of the target control.")] string identifier) =>
        McpToolResults.Execute(async () =>
        {
            await _coordinator.ClickAsync(identifier);
            return McpToolResults.Success(new OperationResult("ok"));
        });

    [McpServerTool(Name = "input_text", UseStructuredContent = true, OutputSchemaType = typeof(OperationResult))]
    [Description("Type text into a uniquely identified visible and enabled TextBox using real headless input.")]
    public Task<CallToolResult> InputText(
        [Description("Unique AutomationId or Name of the target TextBox.")] string identifier,
        [Description("Text to enter at the current caret position.")] string text) =>
        McpToolResults.Execute(async () =>
        {
            await _coordinator.InputTextAsync(identifier, text);
            return McpToolResults.Success(new OperationResult("ok"));
        });

    [McpServerTool(Name = "capture_screenshot", ReadOnly = true, UseStructuredContent = true, OutputSchemaType = typeof(ScreenshotMetadata))]
    [Description("Capture the active scenario as a PNG image with scenario and dimension metadata.")]
    public Task<CallToolResult> CaptureScreenshot() =>
        McpToolResults.Execute(async () =>
            McpToolResults.Screenshot(await _coordinator.CaptureScreenshotAsync()));

    [McpServerTool(Name = "reset_scenario", UseStructuredContent = true, OutputSchemaType = typeof(ScenarioOperationResult))]
    [Description("Replace the active scenario with a newly constructed initial instance of the same scenario.")]
    public Task<CallToolResult> ResetScenario() =>
        McpToolResults.Execute(async () =>
        {
            var scenarioId = (await _coordinator.InspectAsync()).ScenarioId;
            await _coordinator.ResetAsync();
            return McpToolResults.Success(new ScenarioOperationResult(scenarioId));
        });

    [McpServerTool(Name = "close_scenario", UseStructuredContent = true, OutputSchemaType = typeof(OperationResult))]
    [Description("Close the active scenario and dispose its owned resources.")]
    public Task<CallToolResult> CloseScenario() =>
        McpToolResults.Execute(async () =>
        {
            await _coordinator.CloseAsync();
            return McpToolResults.Success(new OperationResult("ok"));
        });
}
