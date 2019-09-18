using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Models.Quests.Json
{
    internal class MapDescriptorConverter : JsonConverter<MapDescriptor>
    {
        public override MapDescriptor Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new FormatException();
            return new MapDescriptor(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, MapDescriptor value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString());
    }
}
