using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Sakuno.ING.Game.Json.Converters
{
    internal class IdConverter<TId> : JsonConverter
        where TId : struct
    {
        private static readonly Type nullableType = typeof(TId?);

        public override bool CanConvert(Type objectType) => true;
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            int int32Value = Convert.ToInt32(reader.Value);
            if (int32Value <= 0)
                if (objectType == nullableType)
                    return null;
                else
                    throw new ArgumentOutOfRangeException("The id must be valid.");
            return Unsafe.As<int, TId>(ref int32Value);
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotSupportedException();
    }
}
