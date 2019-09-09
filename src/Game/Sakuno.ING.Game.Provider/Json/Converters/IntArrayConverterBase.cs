using System;
using Newtonsoft.Json;

namespace Sakuno.ING.Game.Json.Converters
{
    internal abstract class IntArrayConverterBase<T> : JsonConverter<T>
    {
        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer) => throw new NotSupportedException();
        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartArray)
                return default;

            Span<int> array = stackalloc int[RequiredCount];
            int i;
            for (i = 0; reader.ReadAsInt32() is int current; i++)
                if (i < array.Length)
                    array[i] = current;

            if (i >= array.Length)
                return ConvertValue(array);
            else
                return default;
        }

        protected abstract int RequiredCount { get; }
        protected abstract T ConvertValue(ReadOnlySpan<int> array);
    }
}
