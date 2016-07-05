using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Models.Preferences;
using System;

namespace Sakuno.KanColle.Amatsukaze.Internal
{
    class PreferencePropertyJsonConverter : JsonConverter
    {
        public static PreferencePropertyJsonConverter Instance { get; } = new PreferencePropertyJsonConverter();

        public override bool CanConvert(Type rpObjectType) => true;

        public override object ReadJson(JsonReader rpReader, Type rpObjectType, object rpExistingValue, JsonSerializer rpSerializer)
        {
            Property rProperty;
            if (Property.Cache.TryGetValue(rpObjectType, out rProperty))
            {
                var rToken = JToken.Load(rpReader);
                rProperty.Setter.DynamicInvoke(rpExistingValue, rToken);

                return rpExistingValue;
            }

            throw new InvalidOperationException();
        }

        public override void WriteJson(JsonWriter rpWriter, object rpValue, JsonSerializer rpSerializer)
        {
            var rType = GetBaseType(rpValue.GetType());
            if (rType == null)
                throw new InvalidOperationException();

            Property rProperty;
            if (!Property.Cache.TryGetValue(rType, out rProperty))
                return;

            rpWriter.WriteValue(rProperty.Getter.DynamicInvoke(rpValue));
        }
        static Type GetBaseType(Type rpType)
        {
            while (rpType != null && rpType != typeof(object))
            {
                var rType = rpType.IsGenericType ? rpType.GetGenericTypeDefinition() : rpType;
                if (rType == typeof(Property<>))
                    return rpType;

                rpType = rpType.BaseType;
            }
            return null;
        }
    }
}
