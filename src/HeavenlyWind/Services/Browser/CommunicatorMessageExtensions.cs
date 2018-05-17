using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.Services.Browser
{
    static class CommunicatorMessageExtensions
    {
        public static IObservable<KeyValuePair<string, string>> GetMessageObservable(this MemoryMappedFileCommunicator rpCommunicator)
        {
            return rpCommunicator.DataReceived.Select(r =>
            {
                var rMessage = Encoding.UTF8.GetString(r);

                var rCommand = rMessage;
                var rParamater = string.Empty;

                var rPosition = rMessage.IndexOf(':');
                if (rPosition != -1)
                {
                    rCommand = rMessage.Remove(rPosition);
                    rParamater = rMessage.Substring(rPosition + 1);
                }

                return new KeyValuePair<string, string>(rCommand, rParamater);
            });
        }

        public static IDisposable Subscribe(this IObservable<KeyValuePair<string, string>> rpObserver, string rpCommand, Action<string> rpAction) =>
            rpObserver.Where(r => r.Key == rpCommand).Subscribe(r => rpAction(r.Value));
        public static IDisposable SubscribeOnDispatcher(this IObservable<KeyValuePair<string, string>> rpObserver, string rpCommand, Action<string> rpAction) =>
            rpObserver.Where(r => r.Key == rpCommand).ObserveOnDispatcher().Subscribe(r => rpAction(r.Value));
    }
}
