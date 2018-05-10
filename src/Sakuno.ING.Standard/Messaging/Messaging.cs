using System;
using System.Runtime.ExceptionServices;

namespace Sakuno.ING.Messaging
{
    public delegate void TimedMessageHandler<in T>(DateTimeOffset timeStamp, T message);

    public interface ITimedMessageProvider<out T>
    {
        event TimedMessageHandler<T> Received;
    }

    internal abstract class Sender<T>
    {
        protected TimedMessageHandler<T> Downstreams;
        protected void SendToDownstream(DateTimeOffset timeStamp, T value)
        {
            var temp = Downstreams;
            if (temp == null) return;
            var list = temp.GetInvocationList();
            if (list.Length == 1)
                temp(timeStamp, value);
            else
            {
                Exception exception = null;
                foreach (TimedMessageHandler<T> invo in list)
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
        }
    }

    internal abstract class Chainer<TInput, TOutput>
        : Sender<TOutput>, ITimedMessageProvider<TOutput>
    {
        private ITimedMessageProvider<TInput> upstream;

        public Chainer(ITimedMessageProvider<TInput> upstream)
            => this.upstream = upstream
            ?? throw new ArgumentNullException(nameof(upstream));

        public event TimedMessageHandler<TOutput> Received
        {
            add
            {
                if ((Downstreams += value) != null)
                    upstream.Received += Send;
            }
            remove
            {
                if ((Downstreams -= value) == null)
                    upstream.Received -= Send;
            }
        }

        public abstract void Send(DateTimeOffset timeStamp, TInput arg);
    }

    internal class Transformer<TInput, TOutput>
        : Chainer<TInput, TOutput>
    {
        private readonly Func<TInput, TOutput> converter;

        public Transformer(ITimedMessageProvider<TInput> upstream, Func<TInput, TOutput> converter)
            : base(upstream)
            => this.converter = converter
            ?? throw new ArgumentNullException(nameof(converter));

        public override void Send(DateTimeOffset timeStamp, TInput arg) => SendToDownstream(timeStamp, converter(arg));
    }

    internal class Conditioner<T>
        : Chainer<T, T>
    {
        private readonly Predicate<T> predicate;

        public Conditioner(ITimedMessageProvider<T> upstream, Predicate<T> predicate)
            : base(upstream)
            => this.predicate = predicate
            ?? throw new ArgumentNullException(nameof(predicate));

        public override void Send(DateTimeOffset timeStamp, T arg)
        {
            if (predicate(arg))
                SendToDownstream(timeStamp, arg);
        }
    }

    internal class Combiner<T> : Sender<T>, ITimedMessageProvider<T>
    {
        private readonly ITimedMessageProvider<T>[] upstreams;

        public Combiner(params ITimedMessageProvider<T>[] upstreams)
            => this.upstreams = upstreams
            ?? throw new ArgumentNullException(nameof(upstreams));

        public event TimedMessageHandler<T> Received
        {
            add
            {
                if ((Downstreams += value) != null)
                    foreach (var u in upstreams)
                        u.Received += SendToDownstream;
            }
            remove
            {
                if ((Downstreams -= value) == null)
                    foreach (var u in upstreams)
                        u.Received -= SendToDownstream;
            }
        }
    }
}
