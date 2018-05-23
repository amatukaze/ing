using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sakuno.ING.Game.Json.Converters;

namespace Sakuno.ING.Game
{
    internal class GameContractResolver : DefaultContractResolver
    {
        protected override JsonContract CreateContract(Type objectType)
        {
            var contract = base.CreateContract(objectType);

            var id = objectType.GetCustomAttribute<IdentifierAttribute>();
            if (id != null)
            {
                var type = typeof(IdConverter<,>).MakeGenericType(objectType, id.UnderlyingType);
                contract.Converter = (JsonConverter)Activator.CreateInstance(type);
            }

            if (objectType == typeof(DateTimeOffset))
                contract.Converter = new DateTimeMillisecondConverter();

            return contract;
        }
    }
}
