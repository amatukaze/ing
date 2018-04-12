using System;
using System.Collections.Specialized;
using Sakuno.KanColle.Amatsukaze.Game.Json;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public sealed class ParsedMessage<T>
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
    }
}
