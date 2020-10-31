using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Json.Converters
{
    internal sealed class FleetExpeditionStatusConverter : JsonConverter<RawFleetExpeditionStatus>
    {
        public override RawFleetExpeditionStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new NotSupportedException();

            reader.Read();
            var state = (FleetExpeditionState)reader.GetInt32();
            reader.Read();
            var expeditionId = (ExpeditionId)reader.GetInt32();
            reader.Read();
            var returnTime = DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64());
            reader.Read();
            reader.Read();

            if (reader.TokenType != JsonTokenType.EndArray)
                throw new InvalidOperationException();

            return new RawFleetExpeditionStatus(state, expeditionId, returnTime);
        }

        public override void Write(Utf8JsonWriter writer, RawFleetExpeditionStatus value, JsonSerializerOptions options) =>
            throw new NotSupportedException();
    }
}
