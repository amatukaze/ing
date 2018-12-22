using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json.Converters
{
    internal class BoolDictionaryConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(List<int>);
        public override bool CanWrite => false;
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
                return null;
            var result = new List<EquipmentTypeId>();
            while (true)
            {
                reader.Read();
                if (reader.TokenType == JsonToken.EndObject) break;
                string name = reader.Value.ToString();
                var value = reader.ReadAsInt32();
                if (value == 1)
                    result.Add((EquipmentTypeId)int.Parse(name));
            }
            return result;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotSupportedException();
    }
}
