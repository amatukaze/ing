using System;
using System.Collections.Specialized;
using Sakuno.ING.Game.Json;

namespace Sakuno.ING.Game
{
    public sealed class ParsedMessage<T> : ITimedMessage<T>
    {
        public ParsedMessage(string name, DateTimeOffset timeStamp, NameValueCollection request, SvData<T> response)
        {
            Name = name;
            TimeStamp = timeStamp;
            Request = request;
            IsSuccess = response.api_result == 1;
            Response = response.api_data;
        }

        public string Name { get; }
        public DateTimeOffset TimeStamp { get; }
        public NameValueCollection Request { get; }
        public bool IsSuccess { get; }
        public T Response { get; }

        T ITimedMessage<T>.Message => Response;

        public ITimedMessage<TNew> SelectResponse<TNew>(Func<T, TNew> func)
            => new TimedMessage<TNew>(TimeStamp, func(Response));

        public ITimedMessage<TNew> SelectRequestAndResponse<TNew>(Func<NameValueCollection, T, TNew> func)
            => new TimedMessage<TNew>(TimeStamp, func(Request, Response));
    }
}
