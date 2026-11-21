using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace Sakuno.ING.Shell.UIComponentAutomation.Tests;

public class McpTransportTests
{
    [Fact]
    public async Task RealStdioServerSupportsCompleteWorkflowsAndCleanShutdown()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(45));
        var configuration = new DirectoryInfo(AppContext.BaseDirectory).Parent!.Name;
        var serverPath = Path.Combine(
            Repository.Root,
            "src",
            "Shell",
            "Sakuno.ING.Shell.UIComponentAutomation",
            "bin",
            configuration,
            "net11.0",
            "Sakuno.ING.Shell.UIComponentAutomation.dll");
        Assert.True(File.Exists(serverPath), $"Built server not found: {serverPath}");

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = Repository.Root,
        };
        startInfo.ArgumentList.Add(serverPath);
        using var process = Process.Start(startInfo)!;
        await using var standardOutput = new RecordingReadStream(process.StandardOutput.BaseStream);
        var stderr = process.StandardError.ReadToEndAsync(timeout.Token);

        try
        {
            var transport = new StreamClientTransport(process.StandardInput.BaseStream, standardOutput);
            var client = await McpClient.CreateAsync(transport, cancellationToken: timeout.Token);

            var tools = await client.ListToolsAsync(cancellationToken: timeout.Token);
            Assert.Equal(
                [
                    "capture_screenshot",
                    "click",
                    "close_scenario",
                    "input_text",
                    "inspect_component",
                    "list_scenarios",
                    "open_scenario",
                    "reset_scenario",
                ],
                tools.Select(tool => tool.Name).Order().ToArray());
            Assert.All(tools, tool =>
            {
                Assert.Equal("object", tool.ProtocolTool.InputSchema.GetProperty("type").GetString());
                Assert.True(tool.ProtocolTool.OutputSchema.HasValue);
            });

            var catalog = await Call(client, "list_scenarios", timeout.Token);
            Assert.Equal(2, catalog.StructuredContent!.Value.GetProperty("scenarios").GetArrayLength());

            await Call(client, "open_scenario", timeout.Token, new()
            {
                ["scenarioId"] = UiComponentScenarioCatalog.FleetTabWithFleets,
            });
            Assert.Equal("1", State(await Call(client, "inspect_component", timeout.Token), "SelectedFleetId").GetString());
            await Call(client, "click", timeout.Token, new() { ["identifier"] = "FleetTab_2" });
            Assert.Equal("2", State(await Call(client, "inspect_component", timeout.Token), "SelectedFleetId").GetString());
            await AssertScreenshot(await Call(client, "capture_screenshot", timeout.Token), UiComponentScenarioCatalog.FleetTabWithFleets);

            await Call(client, "reset_scenario", timeout.Token);
            Assert.Equal("1", State(await Call(client, "inspect_component", timeout.Token), "SelectedFleetId").GetString());

            await Call(client, "open_scenario", timeout.Token, new()
            {
                ["scenarioId"] = UiComponentScenarioCatalog.MaterialsWithData,
            });
            var materials = await Call(client, "inspect_component", timeout.Token);
            var expected = new Dictionary<string, int>
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
            foreach (var (name, value) in expected)
                Assert.Equal(value, State(materials, name).GetInt32());
            await AssertScreenshot(await Call(client, "capture_screenshot", timeout.Token), UiComponentScenarioCatalog.MaterialsWithData);

            await Call(client, "close_scenario", timeout.Token);
            var noActive = await Call(client, "inspect_component", timeout.Token);
            Assert.True(noActive.IsError);
            Assert.Equal("no_active_scenario", noActive.StructuredContent!.Value.GetProperty("code").GetString());
            Assert.False((await Call(client, "list_scenarios", timeout.Token)).IsError);

            await client.DisposeAsync();
            process.StandardInput.Close();
            await process.WaitForExitAsync(timeout.Token);
            Assert.Equal(0, process.ExitCode);
            AssertProtocolOnly(standardOutput.CapturedBytes);
        }
        finally
        {
            if (!process.HasExited)
            {
                process.StandardInput.Close();
                try
                {
                    await process.WaitForExitAsync(new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token);
                }
                catch (OperationCanceledException)
                {
                    process.Kill(entireProcessTree: true);
                }
            }
        }

        Assert.Equal(string.Empty, await stderr);
    }

    private static ValueTask<CallToolResult> Call(
        McpClient client,
        string name,
        CancellationToken cancellationToken,
        Dictionary<string, object?>? arguments = null) =>
        client.CallToolAsync(name, arguments ?? new(), cancellationToken: cancellationToken);

    private static JsonElement State(CallToolResult result, string name) =>
        result.StructuredContent!.Value.GetProperty("state").GetProperty(name);

    private static Task AssertScreenshot(CallToolResult result, string scenarioId)
    {
        Assert.False(result.IsError);
        var image = Assert.IsType<ImageContentBlock>(Assert.Single(result.Content));
        Assert.Equal("image/png", image.MimeType);
        var png = image.DecodedData.ToArray();
        Assert.Equal([137, 80, 78, 71, 13, 10, 26, 10], png[..8]);
        Assert.Equal(scenarioId, result.StructuredContent!.Value.GetProperty("scenarioId").GetString());
        Assert.Equal(800, result.StructuredContent!.Value.GetProperty("width").GetInt32());
        Assert.Equal(450, result.StructuredContent!.Value.GetProperty("height").GetInt32());
        return Task.CompletedTask;
    }

    private static void AssertProtocolOnly(byte[] output)
    {
        var lines = Encoding.UTF8.GetString(output).Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.NotEmpty(lines);
        foreach (var line in lines)
        {
            using var message = JsonDocument.Parse(line);
            Assert.Equal("2.0", message.RootElement.GetProperty("jsonrpc").GetString());
        }
    }

    private sealed class RecordingReadStream(Stream inner) : Stream
    {
        private readonly MemoryStream _captured = new();

        public byte[] CapturedBytes => _captured.ToArray();
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = inner.Read(buffer, offset, count);
            _captured.Write(buffer, offset, read);
            return read;
        }

        public override async ValueTask<int> ReadAsync(
            Memory<byte> buffer,
            CancellationToken cancellationToken = default)
        {
            var read = await inner.ReadAsync(buffer, cancellationToken);
            _captured.Write(buffer[..read].Span);
            return read;
        }

        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
