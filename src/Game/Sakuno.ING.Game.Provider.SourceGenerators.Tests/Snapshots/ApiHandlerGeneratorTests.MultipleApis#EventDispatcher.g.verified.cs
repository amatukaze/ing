//HintName: EventDispatcher.g.cs
#nullable enable
using System;
using System.Text.Json;
using Sakuno.ING.Game;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Provider;
partial class EventDispatcher
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
                if (!ValidateResultCode(response.api_result))
                    return true;
                HandleApis(message.Api);
                return true;
            }
        }

        return false;
    }
}