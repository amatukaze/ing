using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Sakuno.KanColle.Amatsukaze.Game.Services
{
    public class SessionService
    {
        public static SessionService Instance { get; } = new SessionService();

        Dictionary<string, ApiParserBase> r_Parsers = ApiParserManager.Instance.Parsers;

        SessionService() { }

        ApiParserBase GetParser(string rpApi)
        {
            ApiParserBase rParser;
            if (!r_Parsers.TryGetValue(rpApi, out rParser))
                r_Parsers.Add(rpApi, rParser = new DefaultApiParser() { Api = rpApi });

            return rParser;
        }

        public Subject<ApiData> GetProcessSucceededSubject(string rpApi) => GetParser(rpApi).ProcessSucceeded;
        public IDisposable Subscribe(string rpApi, Action<ApiData> rpAction) => GetProcessSucceededSubject(rpApi).Subscribe(rpAction);
    }
}
