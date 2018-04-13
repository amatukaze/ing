using System;
using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Json.Converters
{
    internal class HtmlNewLineEater : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(string);
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            => reader.Value.ToString().Replace("<br>", string.Empty);
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotSupportedException();
        public override bool CanWrite => false;
    }
}
