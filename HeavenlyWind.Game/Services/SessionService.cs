using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Services
{
    public class SessionService
    {
        public static SessionService Instance { get; } = new SessionService();

        Dictionary<string, ApiParserBase> r_Parsers = ApiParserManager.Instance.Parsers;

        SessionService() { }

        public IDisposable Subscribe(string rpApi, Action<ApiData> rpAction)
        {
            ApiParserBase rParser;
            if (!r_Parsers.TryGetValue(rpApi, out rParser))
                r_Parsers.Add(rpApi, rParser = new DefaultApiParser() { Api = rpApi });

            return rParser.ProcessSucceeded.Subscribe(rpAction);
        }
    }
}
