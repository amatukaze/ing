using System;

namespace Sakuno.KanColle.Amatsukaze.Messaging
{
    public interface IProducer<out T>
    {
        event Action<T> Received;
    }

    public interface IReceiver<in T>
    {
        void Send(T arg);
    }

    public class Chainer<TInput, TOutput>
        : IReceiver<TInput>, IProducer<TOutput>
    {
        private Func<TInput, TOutput> converter;

        public Chainer(Func<TInput, TOutput> converter)
            => this.converter = converter
            ?? throw new ArgumentNullException(nameof(converter));

        public event Action<TOutput> Received;

        public void Send(TInput arg) => Received?.Invoke(converter(arg));
    }

    public class Conditioner<T>
        : IReceiver<T>, IProducer<T>
    {
        private Predicate<T> predicate;

        public Conditioner(Predicate<T> predicate)
            => this.predicate = predicate
            ?? throw new ArgumentNullException(nameof(predicate));

        public event Action<T> Received;

        public void Send(T arg)
        {
            var temp = Received;
            if (temp != null && predicate(arg))
                temp(arg);
        }
    }
}
