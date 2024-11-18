using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Provider.Json.Converters;

internal sealed class IdentifierConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.GetCustomAttribute<IdentifierAttribute>() is not null;

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = typeof(IdentifierConverter<>).MakeGenericType(typeToConvert);

        return (JsonConverter?)Activator.CreateInstance(converterType);
    }
}
