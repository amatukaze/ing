using System;
using System.IO;
using Newtonsoft.Json;
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

        public GameListener(ITextStreamProvider provider) => this.provider = provider;

        private T Convert<T>(TextReader reader) => jSerializer.Deserialize<T>(new JsonTextReader(reader));

        public IProducer<T> RegisterRaw<T>(string api) => provider
            .Where(arg => arg.Key.Equals(api, StringComparison.Ordinal))
            .Select(arg => Convert<T>(arg.Value));
    }
}
