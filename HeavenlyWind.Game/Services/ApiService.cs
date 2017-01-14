using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Services
{
    public static class ApiService
    {
        static ConcurrentDictionary<string, IObservable<ApiInfo>> r_ApiObservables = new ConcurrentDictionary<string, IObservable<ApiInfo>>();

        public static IDisposable Subscribe(string api, Action<ApiInfo> action) => new SingleApiAfterProcessCompletedSubscription(api, action);
        public static IDisposable Subscribe(string api, Action<ApiInfo> onStarted, Action<ApiInfo> onCompleted)
        {
            var rBpsSubscription = new SingleApiBeforeProcessStartedSubscription(api, onStarted);
            var rApcSubscription = new SingleApiAfterProcessCompletedSubscription(api, onCompleted);

            return Disposable.Create(() =>
            {
                rBpsSubscription.Dispose();
                rApcSubscription.Dispose();
            });
        }
        public static IDisposable SubscribeOnlyOnBeforeProcessStarted(string api, Action<ApiInfo> action) => new SingleApiBeforeProcessStartedSubscription(api, action);

        public static IDisposable Subscribe(string[] apis, Action<ApiInfo> action) => new MultipleApiAfterProcessCompletedSubscription(apis, action);
        public static IDisposable Subscribe(string[] apis, Action<ApiInfo> onStarted, Action<ApiInfo> onCompleted)
        {
            var rBpsSubscription = new MultipleApiBeforeProcessStartedSubscription(apis, onStarted);
            var rApcSubscription = new MultipleApiAfterProcessCompletedSubscription(apis, onCompleted);

            return Disposable.Create(() =>
            {
                rBpsSubscription.Dispose();
                rApcSubscription.Dispose();
            });
        }
        public static IDisposable SubscribeOnlyOnBeforeProcessStarted(string[] apis, Action<ApiInfo> action) => new MultipleApiBeforeProcessStartedSubscription(apis, action);

        public static void SubscribeOnce(string api, Action<ApiInfo> action)
        {
            IDisposable rSubscription = null;
            rSubscription = new SingleApiAfterProcessCompletedSubscription(api, r =>
            {
                action(r);
                rSubscription.Dispose();
            });
        }
        public static void SubscribeOnceOnlyOnBeforeProcessStarted(string api, Action<ApiInfo> action)
        {
            IDisposable rSubscription = null;
            rSubscription = new SingleApiBeforeProcessStartedSubscription(api, r =>
            {
                action(r);
                rSubscription.Dispose();
            });
        }

        internal static IObservable<ApiInfo> GetObservable(string api) =>
            r_ApiObservables.GetOrAdd(api, rpApi =>
            {
                var rResult = Observable.FromEvent<ApiInfo>(
                    rpHandler => ApiParserManager.GetParser(rpApi).AfterProcessCompleted += rpHandler,
                    rpHandler => ApiParserManager.GetParser(rpApi).AfterProcessCompleted -= rpHandler).Publish();
                rResult.Connect();

                return rResult;
            });
    }
}
