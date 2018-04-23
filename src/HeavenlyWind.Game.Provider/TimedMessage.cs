using System;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public class TimedMessage<T> : ITimedMessage<T>
    {
        public DateTimeOffset TimeStamp { get; }
        public T Message { get; }

        public TimedMessage(DateTimeOffset timeStamp, T message)
        {
            TimeStamp = timeStamp;
            Message = message;
        }
    }
}
