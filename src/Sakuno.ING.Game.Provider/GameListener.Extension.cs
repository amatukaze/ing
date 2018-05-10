using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game
{
    partial class GameListener
    {
        private T Convert<T>(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            return jSerializer.Deserialize<T>(new JsonTextReader(new StreamReader(stream)));
        }

        public ITimedMessageProvider<T> RegisterResponse<T>(string api)
            => RegisterRaw<T>(api).Select(x => x.Response);

        public ITimedMessageProvider<NameValueCollection> RegisterRequest(string api)
            => RegisterRaw(api).Select(x => x.Request);

        public ITimedMessageProvider<ParsedMessage<T>> RegisterRaw<T>(string api) => provider
            .Where(arg => arg.Key.Equals(api, StringComparison.Ordinal))
            .Select(arg => new ParsedMessage<T>
            (
                arg.Key,
                HttpUtility.ParseQueryString(arg.Request),
                Convert<SvData<T>>(arg.Stream)
            ))
            .Where(m => m.IsSuccess);

        public ITimedMessageProvider<ParsedMessage> RegisterRaw(string api) => provider
            .Where(arg => arg.Key.Equals(api, StringComparison.Ordinal))
            .Select(arg => new ParsedMessage
            (
                arg.Key,
                HttpUtility.ParseQueryString(arg.Request),
                Convert<SvData>(arg.Stream)
            ))
            .Where(m => m.IsSuccess);

        public ITimedMessageProvider<SvData> RegisterFail() => provider
            .Select(arg => Convert<SvData>(arg.Stream))
            .Where(v => v.api_result != 1);

        public ITimedMessageProvider<ParsedMessage<JToken>> RegisterAny() => provider
            .Select(arg => new ParsedMessage<JToken>
            (
                arg.Key,
                HttpUtility.ParseQueryString(arg.Request),
                Convert<SvData<JToken>>(arg.Stream)
            ));
    }
}
