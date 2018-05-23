using System;
using Newtonsoft.Json;

namespace Sakuno.ING.Game.Json.Converters
{
    internal class DateTimeMillisecondConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(DateTimeOffset);
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            => DateTimeOffset.FromUnixTimeMilliseconds((long)reader.Value);
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotSupportedException();
    }
}
