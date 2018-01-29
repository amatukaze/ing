using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Data.Json;

namespace Sakuno.KanColle.Amatsukaze.Services.Json
{
    class JsonAtomValue : JsonToken, IJsonAtomValueNode
    {
        JValue _value;
        public object Value => _value.Value;

        public JsonAtomValue(JValue value) : base(value)
        {
            _value = value;
        }
    }
}
