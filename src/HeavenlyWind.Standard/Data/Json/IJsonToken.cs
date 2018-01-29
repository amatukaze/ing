using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Data.Json
{
    public interface IJsonToken
    {
        JsonTokenType Type { get; }

        T As<T>();

        IJsonToken SelectToken(string path);
        IEnumerable<IJsonToken> SelectTokens(string path);

        T SelectToken<T>(string path) where T : IJsonToken;
        IEnumerable<T> SelectTokens<T>(string path) where T : IJsonToken;

        T SelectValue<T>(string path);
        IEnumerable<T> SelectValues<T>(string path);
    }
}
