using System;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public sealed class ParsedMessage<T>
    {
        public ParsedMessage(string name, DateTimeOffset timeStamp, T response)
        {
            Name = name;
            TimeStamp = timeStamp;
            Response = response;
        }

        public string Name { get; }
        public DateTimeOffset TimeStamp { get; }
        public T Response { get; }
    }
}
