//HintName: GameProvider.g.cs
#nullable enable
using System;
using System.Text.Json;
using Sakuno.ING.Game;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Provider;
partial class GameProvider
{
    private partial bool HandleApiMessageCore(ApiMessage message)
    {
        switch (message.Api)
        {
            case "api1":
            case "api2":
            case "api3":
            {
                var reader = new Utf8JsonReader(message.Response);
                var response = JsonSerializer.Deserialize(ref reader, JsonModelContext.Default.SvData)!;
                CheckResultCode(response.api_result);
                HandleApis(message.Api);
                return true;
            }
        }

        return false;
    }
}
