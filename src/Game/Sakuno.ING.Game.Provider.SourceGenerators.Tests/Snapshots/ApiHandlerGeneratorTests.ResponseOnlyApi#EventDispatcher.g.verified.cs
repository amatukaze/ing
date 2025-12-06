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
            {
                var reader = new Utf8JsonReader(message.Response);
                var response = JsonSerializer.Deserialize(ref reader, JsonModelContext.Default.SvDataApi1Json)!;
                CheckResultCode(response.api_result);
                HandleApi1(response.api_data);
                return true;
            }
        }

        return false;
    }
}
