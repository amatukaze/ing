using System;

namespace Sakuno.KanColle.Amatsukaze.Messaging
{
    public static class MessagingExtensions
    {
        public static void Chain<T>(this IProducer<T> producer, IReceiver<T> receiver)
            => producer.Received += receiver.Send;

        public static void Unchain<T>(this IProducer<T> producer, IReceiver<T> receiver)
            => producer.Received -= receiver.Send;

        public static IProducer<T> Where<T>(this IProducer<T> producer, Predicate<T> predicate)
        {
            var conditioner = new Conditioner<T>(predicate);
            producer.Chain(conditioner);
            return conditioner;
        }

        public static IProducer<TOutput> Select<TInput,TOutput>(this IProducer<TInput> producer, Func<TInput,TOutput> converter)
        {
            var chainer = new Chainer<TInput, TOutput>(converter);
            producer.Chain(chainer);
            return chainer;
        }
    }
}
