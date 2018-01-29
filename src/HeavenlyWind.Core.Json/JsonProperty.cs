using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Data.Json;

namespace Sakuno.KanColle.Amatsukaze.Services.Json
{
    class JsonProperty : IJsonProperty
    {
        JProperty _property;

        public string Name => _property.Name;

        public IJsonNode Value { get; }

        public JsonProperty(JProperty property)
        {
            _property = property;
            Value = JsonToken.Create(_property.Value);
        }
    }
}
