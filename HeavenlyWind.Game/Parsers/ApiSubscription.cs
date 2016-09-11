using Sakuno.KanColle.Amatsukaze.Game.Services;
using System;
using System.Data.SQLite;
using System.Threading;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers
{
    abstract class ApiSubscription : IDisposable
    {
        Action<ApiInfo> r_Action;

        protected ApiSubscription(Action<ApiInfo> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            r_Action = action;
        }

        public void Subscription(ApiInfo rpInfo)
        {
            try
            {
                r_Action(rpInfo);
            }
            catch (SQLiteException e) when (e.ResultCode == SQLiteErrorCode.Error && RecordService.Instance.HistoryCommandTexts.Count > 0)
            {
                Logger.Write(LoggingLevel.Error, string.Format(StringResources.Instance.Main.Log_Exception_API_ParseException, e.Message));
                RecordService.Instance.HandleException(rpInfo.Session, e);
            }
            catch (Exception e)
            {
                Logger.Write(LoggingLevel.Error, string.Format(StringResources.Instance.Main.Log_Exception_API_ParseException, e.Message));
                ApiParserManager.HandleException(rpInfo.Session, e);
            }
        }

        public void Dispose()
        {
            var rAction = Interlocked.Exchange(ref r_Action, null);
            if (rAction != null)
                Unsubscribe();
        }

        protected abstract void Subscribe();
        protected abstract void Unsubscribe();
    }

    abstract class SingleApiSubscription : ApiSubscription
    {
        protected string r_Api;

        public SingleApiSubscription(string api, Action<ApiInfo> action) : base(action)
        {
            if (api.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(api));

            r_Api = api;

            Subscribe();
        }
    }
    class SingleApiBeforeProcessStartedSubscription : SingleApiSubscription
    {
        public SingleApiBeforeProcessStartedSubscription(string api, Action<ApiInfo> action) : base(api, action) { }

        protected override void Subscribe() =>
            ApiParserManager.GetParser(r_Api).BeforeProcessStarted += Subscription;
        protected override void Unsubscribe() =>
            ApiParserManager.GetParser(r_Api).BeforeProcessStarted -= Subscription;
    }
    class SingleApiAfterProcessCompletedSubscription : SingleApiSubscription
    {
        public SingleApiAfterProcessCompletedSubscription(string api, Action<ApiInfo> action) : base(api, action) { }

        protected override void Subscribe() =>
            ApiParserManager.GetParser(r_Api).AfterProcessCompleted += Subscription;
        protected override void Unsubscribe() =>
            ApiParserManager.GetParser(r_Api).AfterProcessCompleted -= Subscription;
    }
    abstract class MultipleApiSubscription : ApiSubscription
    {
        protected string[] r_Apis;

        protected MultipleApiSubscription(string[] apis, Action<ApiInfo> action) : base(action)
        {
            if (apis == null || apis.Length == 0)
                throw new ArgumentException(nameof(apis));

            r_Apis = apis;

            Subscribe();
        }

    }
    class MultipleApiBeforeProcessStartedSubscription : MultipleApiSubscription
    {
        public MultipleApiBeforeProcessStartedSubscription(string[] apis, Action<ApiInfo> action) : base(apis, action) { }

        protected override void Subscribe()
        {
            foreach (var rApi in r_Apis)
                ApiParserManager.GetParser(rApi).BeforeProcessStarted += Subscription;
        }
        protected override void Unsubscribe()
        {
            foreach (var rApi in r_Apis)
                ApiParserManager.GetParser(rApi).BeforeProcessStarted -= Subscription;
        }
    }
    class MultipleApiAfterProcessCompletedSubscription : MultipleApiSubscription
    {
        public MultipleApiAfterProcessCompletedSubscription(string[] apis, Action<ApiInfo> action) : base(apis, action) { }

        protected override void Subscribe()
        {
            foreach (var rApi in r_Apis)
                ApiParserManager.GetParser(rApi).AfterProcessCompleted += Subscription;
        }
        protected override void Unsubscribe()
        {
            foreach (var rApi in r_Apis)
                ApiParserManager.GetParser(rApi).AfterProcessCompleted -= Subscription;
        }
    }
}
