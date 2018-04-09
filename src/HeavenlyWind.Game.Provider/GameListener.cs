using System;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Messaging;
using Sakuno.KanColle.Amatsukaze.Services;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public sealed partial class GameListener
    {
        private ITextStreamProvider provider;
        private JsonSerializer jSerializer = new JsonSerializer
        {
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        public GameListener(ITextStreamProvider provider)
        {
            this.provider = provider;
            jSerializer.Error += JsonError;
        }

        private void JsonError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            //Debug usage
        }

        private T Convert<T>(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            return jSerializer.Deserialize<T>(new JsonTextReader(new StreamReader(stream)));
        }

        public IProducer<ParsedMessage<T>> RegisterRaw<T>(string api) => provider
            .Where(arg => arg.Key.Equals(api, StringComparison.Ordinal))
            .Select(arg => new ParsedMessage<T>
            (
                arg.Key,
                arg.TimeStamp,
                HttpUtility.ParseQueryString(arg.Request),
                Convert<T>(arg.Stream)
            ));

        public IProducer<ParsedMessage<JToken>> RegisterAny() => provider
            .Select(arg => new ParsedMessage<JToken>
            (
                arg.Key,
                arg.TimeStamp,
                HttpUtility.ParseQueryString(arg.Request),
                Convert<JToken>(arg.Stream)
            ));
    }
}
