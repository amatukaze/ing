using Sakuno.ING.Game.Json;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;

namespace Sakuno.ING.Game
{
    internal static class SvDataObservableExtensions
    {
        public static IObservable<T> OfData<T>(this IObservable<SvData?> source) =>
            source.OfType<SvData<T>>().Where(svdata => svdata.api_result == 1).Select(svdata => svdata.api_data);

        public static IObservable<TEvent> Parse<TRaw, TEvent>(this IObservable<SvData?> source, Func<TRaw, TEvent> eventSelector)
        {
            var events = source.OfType<SvData<TRaw>>().Where(svdata => svdata.api_result == 1).Select(svdata => eventSelector(svdata.api_data)).Publish();
            events.Connect();

            return events.AsObservable();
        }

        public static int GetInt(this NameValueCollection source, string name) => int.Parse(source[name]);
        public static int[] GetInts(this NameValueCollection source, string name) =>
            source[name]?.Split(',').Select(int.Parse).ToArray() ?? Array.Empty<int>();
        public static bool GetBool(this NameValueCollection source, string name) => source.GetInt(name) != 0;
    }
}
