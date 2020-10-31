using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Json.Converters
{
    internal sealed class IntToBooleanConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            reader.GetInt32() != 0;

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) =>
            throw new NotSupportedException();
    }
}
