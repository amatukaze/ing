using System.Text.Json;
using ModelContextProtocol.Protocol;

namespace Sakuno.ING.Shell.UIComponentAutomation;

internal static class McpToolResults
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public static CallToolResult Success<T>(T value)
    {
        var content = JsonSerializer.SerializeToElement(value, SerializerOptions);
        return new()
        {
            Content = [new TextContentBlock { Text = content.GetRawText() }],
            StructuredContent = content,
            IsError = false,
        };
    }

    public static CallToolResult Screenshot(ScreenshotCapture capture) =>
        new()
        {
            Content = [ImageContentBlock.FromBytes(capture.Png, "image/png")],
            StructuredContent = Serialize(new ScreenshotMetadata(capture.ScenarioId, capture.Width, capture.Height)),
            IsError = false,
        };

    public static CallToolResult Error(UiComponentAutomationError error)
    {
        var content = Serialize(error);
        return new()
        {
            Content = [new TextContentBlock { Text = content.GetRawText() }],
            StructuredContent = content,
            IsError = true,
        };
    }

    public static async Task<CallToolResult> Execute(Func<Task<CallToolResult>> operation)
    {
        try
        {
            return await operation();
        }
        catch (UiComponentAutomationException exception)
        {
            return Error(exception.Error);
        }
        catch (Exception exception)
        {
            return Error(UiComponentAutomationErrors.HostFailure("mcp", exception).Error);
        }
    }

    private static JsonElement Serialize<T>(T value) => JsonSerializer.SerializeToElement(value, SerializerOptions);
}
