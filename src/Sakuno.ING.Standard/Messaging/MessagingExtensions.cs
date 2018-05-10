using System;
using System.Linq;

namespace Sakuno.ING.Messaging
{
    public static class MessagingExtensions
    {
        public static ITimedMessageProvider<T> Where<T>(this ITimedMessageProvider<T> producer, Predicate<T> predicate)
            => new Conditioner<T>(producer, predicate);

        public static ITimedMessageProvider<TOutput> Select<TInput, TOutput>(this ITimedMessageProvider<TInput> producer, Func<TInput, TOutput> converter)
            => new Transformer<TInput, TOutput>(producer, converter);

        public static ITimedMessageProvider<T> CombineWith<T>(this ITimedMessageProvider<T> first, ITimedMessageProvider<T> second)
            => new Combiner<T>(first, second);

        public static ITimedMessageProvider<T> CombineWith<T>(this ITimedMessageProvider<T> first, params ITimedMessageProvider<T>[] others)
            => new Combiner<T>(others.Prepend(first).ToArray());
    }
}
