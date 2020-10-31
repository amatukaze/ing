using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Json.Converters
{
    internal sealed class TimestampInMillisecondConverter : JsonConverter<DateTimeOffset?>
    {
        public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var timestamp = reader.GetInt64();
            if (timestamp == 0)
                return null;

            return DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options) =>
            throw new NotSupportedException();
    }
}
