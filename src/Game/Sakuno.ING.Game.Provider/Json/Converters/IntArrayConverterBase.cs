using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Json.Converters
{
    internal abstract class IntArrayConverterBase<T> : JsonConverter<T>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new NotSupportedException();

            var span = (stackalloc int[MaxLength]);

            var actualLength = 0;
            for (var i = 0; reader.Read() && reader.TokenType == JsonTokenType.Number; i++)
                if (i < span.Length)
                    span[actualLength++] = reader.GetInt32();

            if (reader.TokenType != JsonTokenType.EndArray)
                throw new InvalidOperationException();

            if (actualLength < MinLength)
                throw new InvalidOperationException();

            return Parse(span);
        }

        protected abstract int MaxLength { get; }
        protected virtual int MinLength => MaxLength;

        protected abstract T Parse(ReadOnlySpan<int> span);

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) =>
            throw new NotSupportedException();
    }
}
