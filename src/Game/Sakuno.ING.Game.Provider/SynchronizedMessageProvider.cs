using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game
{
    internal sealed class SynchronizedMessageProvider<T> : ITimedMessageProvider<T>
    {
        private readonly object lockObj = new object();
        public SynchronizedMessageProvider(ITimedMessageProvider<T> original)
        {
            original.Received += SendToDownstream;
        }
        private List<TimedMessageHandler<T>> Downstreams { get; } = new List<TimedMessageHandler<T>>();
        private void SendToDownstream(DateTimeOffset timeStamp, T value)
        {
            Exception exception = null;
            lock (lockObj)
                foreach (var invo in Downstreams)
                {
                    try
                    {
                        invo(timeStamp, value);
                    }
                    catch (Exception ex)
                    {
                        if (exception == null)
                            exception = ex;
                        else
                            exception = new AggregateException(exception, ex);
                    }
                }
            if (exception is AggregateException agg)
                throw agg.Flatten();
            else if (exception != null)
            {
                var di = ExceptionDispatchInfo.Capture(exception);
                di.Throw();
            }
        }

        public event TimedMessageHandler<T> Received
        {
            add => Downstreams.Add(value);
            remove => Downstreams.Remove(value);
        }
    }
}
