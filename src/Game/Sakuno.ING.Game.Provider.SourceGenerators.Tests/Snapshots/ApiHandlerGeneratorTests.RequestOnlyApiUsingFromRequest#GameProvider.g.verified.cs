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
            {
                ShipId id = default;
                string name = string.Empty;
                bool flag = default;
                foreach (var item in message.EnumerateRequestQueryString())
                {
                    if (item.Name.SequenceEqual("api_id"u8))
                        id = item.DecodeValueAsIdentifier<ShipId>();
                    else if (item.Name.SequenceEqual("api_name"u8))
                        name = item.DecodeValueAsString();
                    else if (item.Name.SequenceEqual("api_flag"u8))
                        flag = item.DecodeValueAsBool();
                }

                var reader = new Utf8JsonReader(message.Response);
                var response = JsonSerializer.Deserialize(ref reader, JsonModelContext.Default.SvData)!;
                CheckResultCode(response.api_result);
                HandleApi1(id, name, flag);
                return true;
            }

            case "api2":
            {
                int number = default;
                int[] items = [];
                SlotItemId[] ids = [];
                foreach (var item in message.EnumerateRequestQueryString())
                {
                    if (item.Name.SequenceEqual("api_abc"u8))
                        number = item.DecodeValueAsInt();
                    else if (item.Name.SequenceEqual("api_items"u8))
                        items = item.DecodeValueAsIntArray();
                    else if (item.Name.SequenceEqual("api_ids"u8))
                        ids = item.DecodeValueAsIdentifierArray<SlotItemId>();
                }

                var reader = new Utf8JsonReader(message.Response);
                var response = JsonSerializer.Deserialize(ref reader, JsonModelContext.Default.SvData)!;
                CheckResultCode(response.api_result);
                HandleApi2(number, items, ids);
                return true;
            }
        }

        return false;
    }
}
