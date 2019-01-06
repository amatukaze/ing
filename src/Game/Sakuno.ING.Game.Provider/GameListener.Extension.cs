using System;
using System.Collections.Specialized;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game
{
    public partial class GameProvider
    {
        private T Convert<T>(ReadOnlyMemory<char> response)
            => jSerializer.Deserialize<T>(new JsonTextReader(new MemoryReader(response)));

        private static NameValueCollection ParseRequest(ReadOnlyMemory<char> request)
            => HttpUtility.ParseQueryString(request.ToString());

        public ITimedMessageProvider<T> RegisterResponse<T>(string api)
            => RegisterRaw<T>(api).Select(x => x.Response);

        public ITimedMessageProvider<NameValueCollection> RegisterRequest(string api)
            => RegisterRaw(api).Select(x => x.Request);

        public ITimedMessageProvider<ParsedMessage<T>> RegisterRaw<T>(string api) => provider
            .Where(arg => arg.Key.Equals(api, StringComparison.Ordinal))
            .Select(arg => new ParsedMessage<T>
            (
                arg.Key,
                ParseRequest(arg.Request),
                Convert<SvData<T>>(arg.Response)
            ))
            .Where(m => m.IsSuccess);

        public ITimedMessageProvider<ParsedMessage> RegisterRaw(string api) => provider
            .Where(arg => arg.Key.Equals(api, StringComparison.Ordinal))
            .Select(arg => new ParsedMessage
            (
                arg.Key,
                ParseRequest(arg.Request),
                Convert<SvData>(arg.Response)
            ))
            .Where(m => m.IsSuccess);

        public ITimedMessageProvider<ParsedMessage> RegisterFail() => provider
            .Select(arg => new ParsedMessage
            (
                arg.Key,
                ParseRequest(arg.Request),
                Convert<SvData>(arg.Response)
            ))
            .Where(m => !m.IsSuccess);

        public ITimedMessageProvider<ParsedMessage<JToken>> RegisterAny() => provider
            .Select(arg => new ParsedMessage<JToken>
            (
                arg.Key,
                ParseRequest(arg.Request),
                Convert<SvData<JToken>>(arg.Response)
            ));
    }
}
