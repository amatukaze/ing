using System;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public interface ITimedMessage<out T>
    {
        DateTimeOffset TimeStamp { get; }
        T Message { get; }
    }
}
