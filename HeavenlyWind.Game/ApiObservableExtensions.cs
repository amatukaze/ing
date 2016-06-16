using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public static class ApiObservableExtensions
    {
        public static IDisposable SubscribeCore(this IObservable<ApiData> rpObservable, Action<ApiData> rpAction)
        {
            return rpObservable.SubscribeCore(rpAction, (rpApiData, r) => ApiParserManager.Instance.HandleException(rpApiData.Session, r));
        }
        public static IDisposable SubscribeCore(this IObservable<ApiData> rpObservable, Action<ApiData> rpAction, Action<ApiData, Exception> rpExceptionHandler)
        {
            return rpObservable.Subscribe(r =>
            {
                try
                {
                    rpAction(r);
                }
                catch (SQLiteException e) when (e.ResultCode == SQLiteErrorCode.Error && RecordService.Instance.HistoryCommandTexts.Count > 0)
                {
                    Logger.Write(LoggingLevel.Error, string.Format(StringResources.Instance.Main.Log_Exception_API_ParseException, e.Message));
                    RecordService.Instance.HandleException(r.Session, e);
                }
                catch (Exception e)
                {
                    Logger.Write(LoggingLevel.Error, string.Format(StringResources.Instance.Main.Log_Exception_API_ParseException, e.Message));
                    rpExceptionHandler(r, e);
                }
            });
        }
    }
}
