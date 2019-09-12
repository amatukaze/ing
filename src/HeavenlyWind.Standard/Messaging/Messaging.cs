using System;

namespace Sakuno.ING.Messaging
{
    public delegate void TimedMessageHandler<in T>(DateTimeOffset timeStamp, T message);

    public interface ITimedMessageProvider<out T>
    {
        event TimedMessageHandler<T> Received;
    }
}
