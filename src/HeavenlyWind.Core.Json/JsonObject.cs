using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Data.Json;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Services.Json
{
    class JsonObject : JsonToken, IJsonObjectNode
    {
        JObject _object;
        IBindableCollection<IJsonProperty> _properties;

        public JsonObject(JObject @object) : base(@object) =>
            _object = @object;

        public IBindableCollection<IJsonProperty> Properties
        {
            get
            {
                if (_properties == null)
                    _properties = new BindableCollection<IJsonProperty>(_object.Properties().Select(r => new JsonProperty(r)).ToArray());

                return _properties;
            }
        }
    }
}
