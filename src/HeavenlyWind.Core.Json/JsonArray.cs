using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Data.Json;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Services.Json
{
    class JsonArray : JsonToken
    {
        JArray _array;
        IBindableCollection<IJsonNode> _children;

        public JsonArray(JArray array) : base(array)
        {
            _array = array;
        }

        public IBindableCollection<IJsonNode> Children
        {
            get
            {
                if (_children == null)
                    _children = new BindableCollection<IJsonNode>(_array.Select(Create).ToArray());

                return _children;
            }
        }
    }
}
