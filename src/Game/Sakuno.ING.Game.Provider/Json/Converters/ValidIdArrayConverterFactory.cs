using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Json.Converters
{
    internal sealed class ValidIdArrayConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) =>
            typeToConvert.IsArray && typeToConvert.GetElementType().GetCustomAttribute<IdentifierAttribute>() != null;

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var converterType = typeof(ValidIdArrayConverter<>).MakeGenericType(typeToConvert.GetElementType());

            return (JsonConverter)Activator.CreateInstance(converterType);
        }
    }
}
