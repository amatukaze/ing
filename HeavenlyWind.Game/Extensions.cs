using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public static class Extensions
    {
        public static IDTable<T> ToIDTable<T>(this IEnumerable<T> rpSource) where T : IID
        {
            if (rpSource == null)
                throw new ArgumentNullException(nameof(rpSource));

            var rResult = new IDTable<T>();
            foreach (var rItem in rpSource)
                rResult.Add(rItem);

            return rResult;
        }
    }
}
