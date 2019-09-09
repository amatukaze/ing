using System;
using Newtonsoft.Json;

namespace Sakuno.ING.Game.Json.Converters
{
    internal class HtmlNewLineEater : JsonConverter<string>
    {
        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer) => throw new NotSupportedException();
        public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue, JsonSerializer serializer)
            => reader.Value.ToString().Replace("<br>", string.Empty);
    }
}
