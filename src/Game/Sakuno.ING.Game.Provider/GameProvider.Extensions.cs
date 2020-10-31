using Sakuno.ING.Game.Json;
using System;
using System.Reactive.Linq;

namespace Sakuno.ING.Game
{
    internal static class SvDataObservableExtensions
    {
        public static IObservable<TEvent> Parse<TRaw, TEvent>(this IObservable<SvData?> source, Func<TRaw, TEvent> eventSelector)
        {
            var events = source.OfType<SvData<TRaw>>().Select(svdata => eventSelector(svdata.api_data)).Publish();
            events.Connect();

            return events.AsObservable();
        }
    }
}
