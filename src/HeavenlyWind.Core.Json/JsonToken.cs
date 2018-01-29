using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Data.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Services.Json
{
    abstract class JsonToken : IJsonToken, IJsonNode
    {
        static JsonSerializer _serializer;

        JToken _token;

        public JsonTokenType Type
        {
            get
            {
                switch (_token.Type)
                {
                    case JTokenType.Object:
                        return JsonTokenType.Object;

                    case JTokenType.Array:
                        return JsonTokenType.Array;

                    case JTokenType.Integer:
                        return JsonTokenType.Integer;

                    case JTokenType.Float:
                        return JsonTokenType.Float;

                    case JTokenType.String:
                        return JsonTokenType.String;

                    case JTokenType.Boolean:
                        return JsonTokenType.Boolean;

                    case JTokenType.Null:
                        return JsonTokenType.Null;

                    default: throw new InvalidOperationException();
                }
            }
        }

        static JsonToken()
        {
            _serializer = new JsonSerializer() { ContractResolver = new ContractResolver() };
        }

        protected JsonToken(JToken token)
        {
            _token = token;
        }

        public static JsonToken Create(JToken token)
        {
            if (token is JObject @object)
                return new JsonObject(@object);

            if (token is JArray array)
                return new JsonArray(array);

            if (token is JValue value)
                return new JsonAtomValue(value);

            throw new ArgumentException("Unexpected token type.");
        }

        public IJsonToken SelectToken(string path)
        {
            var result = _token.SelectToken(path);

            if (result == null)
                return null;

            return Create(result);
        }
        public IEnumerable<IJsonToken> SelectTokens(string path)
        {
            var tokens = _token.SelectTokens(path);

            if (tokens is JToken[] array)
            {
                var result = new JsonToken[array.Length];

                for (var i = 0; i < array.Length; i++)
                    result[i] = Create(array[i]);

                return result;
            }

            return MapTokenEnumerable(tokens);
        }
        IEnumerable<IJsonToken> MapTokenEnumerable(IEnumerable<JToken> tokens)
        {
            foreach (var token in tokens)
                yield return Create(token);
        }

        public T SelectToken<T>(string path) where T : IJsonToken => (T)SelectToken(path);
        public IEnumerable<T> SelectTokens<T>(string path) where T : IJsonToken => SelectTokens(path).Cast<T>();

        public T As<T>() => _token.ToObject<T>(_serializer);

        public T SelectValue<T>(string path)
        {
            var token = SelectToken<JsonToken>(path)._token;

            if (token is JValue value)
                return token.Value<T>();

            return token.ToObject<T>();
        }
        public IEnumerable<T> SelectValues<T>(string path)
        {
            var token = SelectToken<JsonToken>(path)._token;

            return token.Values<T>();
        }
    }
}
