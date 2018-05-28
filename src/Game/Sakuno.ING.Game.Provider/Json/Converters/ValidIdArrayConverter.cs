using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sakuno.ING.Game.Json.Converters
{
    internal class ValidIdArrayConverter<TId, TUnderlying> : JsonConverter
        where TId : struct, IComparable<TId>
    {
        private readonly Func<TUnderlying, TId> cast;
        private readonly Type nullableType = typeof(TId?);
        public ValidIdArrayConverter()
        {
            var op = typeof(TId).GetMethod("op_Explicit", new Type[] { typeof(TUnderlying) });
            cast = (Func<TUnderlying, TId>)op.CreateDelegate(typeof(Func<TUnderlying, TId>));
        }

        public override bool CanConvert(Type objectType) => true;
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartArray)
                return null;

            var list = new List<TId>();
            for (reader.Read(); reader.TokenType != JsonToken.EndArray; reader.Read())
            {
                var value = cast((TUnderlying)Convert.ChangeType(reader.Value, typeof(TUnderlying)));
                if (value.CompareTo(default) > 0)
                    list.Add(value);
            }
            return list;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotSupportedException();
    }
}
