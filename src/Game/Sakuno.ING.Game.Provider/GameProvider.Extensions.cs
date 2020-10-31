using Sakuno.ING.Game.Json;
using System;
using System.Reactive.Linq;

namespace Sakuno.ING.Game
{
    internal static class SvDataObservableExtensions
    {
        public static IObservable<TEvent> Parse<TRaw, TEvent>(this IObservable<SvData?> source, Func<TRaw, TEvent> eventSelector)
        {
            var events = source.OfType<SvData<TRaw>>().Where(svdata => svdata.api_result == 1).Select(svdata => eventSelector(svdata.api_data)).Publish();
            events.Connect();

            return events.AsObservable();
        }
        public static IObservable<T> OfData<T>(this IObservable<SvData?> source)
        {
            var events = source.OfType<SvData<T>>().Where(svdata => svdata.api_result == 1).Select(svdata => svdata.api_data).Publish();
            events.Connect();

            return events.AsObservable();
        }
    }
}
