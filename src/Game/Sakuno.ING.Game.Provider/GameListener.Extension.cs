using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Http;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game
{
    public partial class GameProvider
    {
        public T Deserialize<T>(string json)
            => jSerializer.Deserialize<T>(new JsonTextReader(new StringReader(json)));

        private T Convert<T>(ReadOnlyMemory<char> response)
            => jSerializer.Deserialize<T>(new JsonTextReader(new MemoryReader(response)));

        private static NameValueCollection ParseRequest(ReadOnlyMemory<char> request)
            => HttpUtility.ParseQueryString(request.ToString());

        private T Response<T>(HttpMessage message)
            => Convert<SvData<T>>(message.Response).api_data;

        private static NameValueCollection Request(HttpMessage message)
            => ParseRequest(message.Request);
    }

    internal static class TimedMessageHandlerExtensions
    {
        public static void InvokeEach<T>(this TimedMessageHandler<T> handler, DateTimeOffset timeStamp, T param, TimedMessageHandler<Exception> onException)
        {
            foreach (TimedMessageHandler<T> h in handler.GetInvocationList())
            {
                try
                {
                    h(timeStamp, param);
                }
                catch (Exception ex)
                {
                    onException?.Invoke(timeStamp, ex);
                }
            }
        }
    }
}
