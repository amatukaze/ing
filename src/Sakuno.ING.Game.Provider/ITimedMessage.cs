using System;

namespace Sakuno.ING.Game
{
    public interface ITimedMessage<out T>
    {
        DateTimeOffset TimeStamp { get; }
        T Message { get; }
    }
}
