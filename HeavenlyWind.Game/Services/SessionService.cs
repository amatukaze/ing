using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
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

        public Subject<ApiData> GetObservable(string rpApi) => GetParser(rpApi).Observable;
        public IObservable<ApiData> GetObservable(string[] rpApis) => rpApis.Select(GetObservable).Aggregate<IObservable<ApiData>>((x, y) => x.Merge(y));

        public IDisposable Subscribe(string rpApi, Action<ApiData> rpAction) => GetObservable(rpApi).SubscribeCore(rpAction);
        public IDisposable Subscribe(string[] rpApis, Action<ApiData> rpAction) => GetObservable(rpApis).SubscribeCore(rpAction);

        public void SubscribeOnce(string rpApi, Action<ApiData> rpAction)
        {
            IDisposable rSubscription = null;
            rSubscription = GetObservable(rpApi).SubscribeCore(r =>
            {
                rpAction(r);
                rSubscription.Dispose();
            });
        }
    }
}
