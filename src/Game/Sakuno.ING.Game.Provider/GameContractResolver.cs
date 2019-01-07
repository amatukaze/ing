using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sakuno.ING.Game.Json.Converters;

namespace Sakuno.ING.Game
{
    internal class GameContractResolver : DefaultContractResolver
    {
        public GameContractResolver() =>
#pragma warning disable CS0618 // Type or member is obsolete
            DefaultMembersSearchFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
#pragma warning restore CS0618 // Type or member is obsolete

        protected override JsonContract CreateContract(Type objectType)
        {
            var contract = base.CreateContract(objectType);
            var valueType = Nullable.GetUnderlyingType(objectType) ?? objectType;

            var id = valueType.GetCustomAttribute<IdentifierAttribute>();
            if (id != null)
            {
                var type = typeof(IdConverter<,>).MakeGenericType(valueType, id.UnderlyingType);
                contract.Converter = (JsonConverter)Activator.CreateInstance(type);
            }

            if (valueType == typeof(DateTimeOffset))
                contract.Converter = new DateTimeMillisecondConverter();

            return contract;
        }

        protected override JsonArrayContract CreateArrayContract(Type objectType)
        {
            var contract = base.CreateArrayContract(objectType);
            var valueType = contract.CollectionItemType;

            var id = valueType.GetCustomAttribute<IdentifierAttribute>();
            if (id != null)
            {
                var type = typeof(ValidIdArrayConverter<,>).MakeGenericType(valueType, id.UnderlyingType);
                contract.Converter = (JsonConverter)Activator.CreateInstance(type);
            }

            return contract;
        }
    }
}
