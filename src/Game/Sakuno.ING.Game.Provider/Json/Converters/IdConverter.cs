using System;
using Newtonsoft.Json;

namespace Sakuno.ING.Game.Json.Converters
{
    internal class IdConverter<TId, TUnderlying> : JsonConverter
    {
        private readonly Func<TUnderlying, TId> cast;
        public IdConverter()
        {
            var op = typeof(TId).GetMethod("op_Explicit", new Type[] { typeof(TUnderlying) });
            cast = (Func<TUnderlying, TId>)op.CreateDelegate(typeof(Func<TUnderlying, TId>));
        }

        public override bool CanConvert(Type objectType) => true;
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return cast((TUnderlying)Convert.ChangeType(reader.Value, typeof(TUnderlying)));
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotSupportedException();
    }
}
