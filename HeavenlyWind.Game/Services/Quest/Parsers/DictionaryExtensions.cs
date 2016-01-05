using System;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Parsers
{
    static class DictionaryExtensions
    {
        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> rpCache, TKey rpKey, Func<TKey, TValue> rpFactory)
        {
            TValue rResult;
            if (!rpCache.TryGetValue(rpKey, out rResult))
                rpCache.Add(rpKey, rResult = rpFactory(rpKey));

            return rResult;
        }
    }
}
