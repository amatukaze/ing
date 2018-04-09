using System;
using System.Collections.Specialized;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public sealed class ParsedMessage<T>
    {
        public ParsedMessage(string name, DateTimeOffset timeStamp, NameValueCollection request, T response)
        {
            Name = name;
            TimeStamp = timeStamp;
            Request = request;
            Response = response;
        }

        public string Name { get; }
        public DateTimeOffset TimeStamp { get; }
        public NameValueCollection Request { get; }
        public T Response { get; }
    }
}
