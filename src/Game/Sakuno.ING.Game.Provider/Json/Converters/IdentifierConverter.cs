using System;
using System.Buffers;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Json.Converters
{
    internal sealed class IdentifierConverter<T> : JsonConverter<T> where T : struct
    {
        public IdentifierConverter()
        {
            if (Unsafe.SizeOf<T>() != sizeof(int))
                throw new NotSupportedException();
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.TokenType switch
            {
                JsonTokenType.Number => reader.GetInt32(),
                JsonTokenType.String => ReadAsInt32(ref reader),

                _ => throw new InvalidOperationException(),
            };

            return Unsafe.As<int, T>(ref value);
        }

        private int ReadAsInt32(ref Utf8JsonReader reader)
        {
            var span = !reader.HasValueSequence ? reader.ValueSpan : reader.ValueSequence.ToArray();

            if (!Utf8Parser.TryParse(span, out int value, out int bytesConsumed) || span.Length != bytesConsumed)
                throw new InvalidOperationException();

            return value;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) =>
            throw new NotSupportedException();
    }
}
