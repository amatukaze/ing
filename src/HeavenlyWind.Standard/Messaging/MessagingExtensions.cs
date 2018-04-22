using System;

namespace Sakuno.KanColle.Amatsukaze.Messaging
{
    public static class MessagingExtensions
    {
        public static IProducer<T> Where<T>(this IProducer<T> producer, Predicate<T> predicate)
            => new Conditioner<T>(producer, predicate);

        public static IProducer<TOutput> Select<TInput, TOutput>(this IProducer<TInput> producer, Func<TInput, TOutput> converter)
            => new Transformer<TInput, TOutput>(producer, converter);

        public static IProducer<T> CombineWith<T>(this IProducer<T> first, IProducer<T> second)
            => new Combiner<T>(first, second);
    }
}
