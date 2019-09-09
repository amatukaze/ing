using System;
using Newtonsoft.Json;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json.Converters
{
    internal class MaterialsConverter : JsonConverter<Materials>
    {
        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, Materials value, JsonSerializer serializer) => throw new NotSupportedException();
        public override Materials ReadJson(JsonReader reader, Type objectType, Materials existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            Materials result = default;

            if (reader.TokenType == JsonToken.StartArray)
            {
                for (var i = 0; reader.ReadAsInt32() is int current; i++)
                    switch (i)
                    {
                        case 0:
                            result.Fuel = current;
                            break;
                        case 1:
                            result.Bullet = current;
                            break;
                        case 2:
                            result.Steel = current;
                            break;
                        case 3:
                            result.Bauxite = current;
                            break;
                        case 4:
                            result.InstantBuild = current;
                            break;
                        case 5:
                            result.InstantRepair = current;
                            break;
                        case 6:
                            result.Development = current;
                            break;
                        case 7:
                            result.Improvement = current;
                            break;
                    }
            }

            return result;
        }
    }
}
