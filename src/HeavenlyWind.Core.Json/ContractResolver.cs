using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using NJJsonProperty = Newtonsoft.Json.Serialization.JsonProperty;

namespace Sakuno.KanColle.Amatsukaze.Data.Json
{
    class ContractResolver : DefaultContractResolver
    {
        protected override NJJsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var result = base.CreateProperty(member, memberSerialization);
            var attribute = member.GetCustomAttribute<JsonPropertyAttribute>();

            if (attribute != null)
                result.PropertyName = attribute.PropertyName;

            return result;
        }
    }
}
