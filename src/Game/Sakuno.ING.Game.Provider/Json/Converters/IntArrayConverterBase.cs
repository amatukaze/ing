using System;
using Newtonsoft.Json;

namespace Sakuno.ING.Game.Json.Converters
{
    internal abstract class IntArrayConverterBase<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType) => typeof(T).IsAssignableFrom(objectType);
        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotSupportedException();
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartArray)
                return default(T);

            Span<int> array = stackalloc int[RequiredCount];
            int i;
            for (i = 0; reader.ReadAsInt32() is int current; i++)
                if (i < array.Length)
                    array[i] = current;

            if (i >= array.Length)
                return ConvertValue(array);
            else
                return default(T);
        }

        protected abstract int RequiredCount { get; }
        protected abstract T ConvertValue(ReadOnlySpan<int> array);
    }
}
