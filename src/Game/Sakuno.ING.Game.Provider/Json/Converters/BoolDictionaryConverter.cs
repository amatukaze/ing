using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json.Converters
{
    internal class BoolDictionaryConverter : JsonConverter<List<EquipmentTypeId>>
    {
        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, List<EquipmentTypeId> value, JsonSerializer serializer) => throw new NotSupportedException();
        public override List<EquipmentTypeId> ReadJson(JsonReader reader, Type objectType, List<EquipmentTypeId> existingValue, bool hasExistingValue, JsonSerializer serializer)
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
    }
}
