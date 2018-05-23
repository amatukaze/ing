using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sakuno.ING.Game.Json.Converters
{
    internal class Minus1Eater<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType.IsAssignableFrom(typeof(List<QuestJson>));
        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotSupportedException();
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = new List<T>();
            if (reader.TokenType == JsonToken.StartArray)
            {
                reader.Read();
                while (reader.TokenType != JsonToken.EndArray)
                {
                    if (reader.TokenType == JsonToken.StartObject ||
                        reader.TokenType == JsonToken.StartArray)
                        result.Add(serializer.Deserialize<T>(reader));
                    else reader.Read();
                }
            }
            return result;
        }
    }
}
