using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Models.Quests;

namespace Sakuno.ING.Game.Json.Converters
{
    internal class Minus1Eater : JsonConverter<List<RawQuest>>
    {
        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, List<RawQuest> value, JsonSerializer serializer) => throw new NotSupportedException();
        public override List<RawQuest> ReadJson(JsonReader reader, Type objectType, List<RawQuest> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var result = new List<RawQuest>();
            if (reader.TokenType == JsonToken.StartArray)
            {
                reader.Read();
                while (reader.TokenType != JsonToken.EndArray)
                {
                    if (reader.TokenType == JsonToken.StartObject ||
                        reader.TokenType == JsonToken.StartArray)
                        result.Add(serializer.Deserialize<RawQuest>(reader));
                    else reader.Read();
                }
            }
            return result;
        }
    }
}
