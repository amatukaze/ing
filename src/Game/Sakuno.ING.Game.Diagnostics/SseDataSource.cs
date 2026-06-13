using System.Net.ServerSentEvents;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sakuno.ING.Game.Services;

namespace Sakuno.ING.Game.Diagnostics;

internal record RawApiMessage(string Api, string Request, string Response);

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(RawApiMessage))]
internal partial class RawApiMessageSerializerContext : JsonSerializerContext;

internal sealed class SseDataSource(
    DiagnosticsOptions options,
    HttpClient httpClient,
    MasterDataService masterDataService,
    PlayerDataService playerDataService,
    ILogger<SseDataSource> logger) : BackgroundService, IApiMessageProvider
{
    private readonly Subject<ApiMessage> _apiMessages = new();
    public IObservable<ApiMessage> ApiMessages => _apiMessages.AsObservable();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (options.SseEndpoint is null)
        {
            logger.LogInformation("No endpoint configured, ignore");
            return;
        }

        await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);

        logger.LogInformation("Connecting to SSE endpoint {Endpoint}", options.SseEndpoint);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, options.SseEndpoint);
                request.Headers.Add("Accept", "text/event-stream");

                using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, stoppingToken);
                response.EnsureSuccessStatusCode();

                logger.LogInformation("SSE connection established");

                await using var stream = await response.Content.ReadAsStreamAsync(stoppingToken);

                await foreach (var item in SseParser.Create(stream, (_, data) => JsonSerializer.Deserialize(data, RawApiMessageSerializerContext.Default.RawApiMessage)!).EnumerateAsync(stoppingToken))
                {
                    var (api, rawRequest, rawResponse) = item.Data;

                    logger.LogInformation("Api {Api} received", api);

                    _apiMessages.OnNext(new(api, Encoding.UTF8.GetBytes(rawRequest), new(Encoding.UTF8.GetBytes(rawResponse))));
                }

                logger.LogInformation("SSE connection ended");
                break;
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "SSE connection error, retrying in 5 seconds...");
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception while handling message");
                break;
            }
        }
    }
}
