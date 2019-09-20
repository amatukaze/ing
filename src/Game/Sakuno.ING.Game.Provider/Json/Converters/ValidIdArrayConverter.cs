using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Sakuno.ING.Game.Json.Converters
{
    internal class ValidIdArrayConverter<TId> : JsonConverter
        where TId : struct
    {
        public override bool CanConvert(Type objectType) => true;
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartArray)
                return null;

            var list = new List<TId>();
            for (reader.Read(); reader.TokenType != JsonToken.EndArray; reader.Read())
            {
                int int32Value = Convert.ToInt32(reader.Value);
                if (int32Value > 0)
                    list.Add(Unsafe.As<int, TId>(ref int32Value));
            }
            return list;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotSupportedException();
    }
}
