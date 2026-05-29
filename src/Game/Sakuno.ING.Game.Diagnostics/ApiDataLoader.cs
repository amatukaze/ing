using System.Buffers;
using System.IO.Compression;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using MessagePack;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sakuno.ING.Game.Services;

namespace Sakuno.ING.Game.Diagnostics;

internal sealed class ApiDataLoader(
    ApiDataLoaderOptions options,
    MasterDataService masterDataService,
    PlayerDataService playerDataService,
    ILogger<ApiDataLoader> logger) : BackgroundService, IApiMessageProvider
{
    private readonly Subject<ApiMessage> _apiMessages = new();
    public IObservable<ApiMessage> ApiMessages => _apiMessages.AsObservable();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        logger.LogInformation("Loading {Data}", options.Path);

        var zip = await ZipFile.OpenReadAsync(options.Path, stoppingToken);

        var masterData = zip.GetEntry("master-data.msgpack")!;
        var masterDataContent = Encoding.UTF8.GetBytes("{\"api_result\":1,\"api_data\":" + await ConvertMsgpack(masterData.Open(), stoppingToken) + "}");

        _apiMessages.OnNext(new("api_start2/getData", ReadOnlyMemory<byte>.Empty, new ReadOnlySequence<byte>(masterDataContent)));

        for (var i = 0; i < (zip.Entries.Count - 1) / 2; i++)
        {
            var rawMetadata = await new StreamReader(zip.GetEntry($"{i}.txt")!.Open()).ReadToEndAsync(stoppingToken);
            var metadata = rawMetadata.Split("\r\n");
            var api = metadata[0];
            var request = Encoding.UTF8.GetBytes(metadata[1]);
            var response = Encoding.UTF8.GetBytes(await ConvertMsgpack(zip.GetEntry($"{i}.msgpack")!.Open(), stoppingToken));

            _apiMessages.OnNext(new(api, new(request), new ReadOnlySequence<byte>(response)));

            logger.LogInformation("Api {Api} loaded", api);
        }
    }

    private async ValueTask<string> ConvertMsgpack(Stream stream, CancellationToken cancellationToken)
    {
        var reader = new MessagePackStreamReader(stream);
        var seq = await reader.ReadAsync(cancellationToken);

        return MessagePackSerializer.ConvertToJson((ReadOnlySequence<byte>)seq!, cancellationToken: cancellationToken);
    }
}
