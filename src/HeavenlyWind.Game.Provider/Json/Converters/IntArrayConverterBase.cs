using System;
using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Json.Converters
{
    internal abstract class IntArrayConverterBase<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType.IsAssignableFrom(typeof(T));
        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotSupportedException();
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartArray)
                return default(T);

            var array = new int[RequiredCount];
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
        protected abstract T ConvertValue(int[] array);
    }
}
