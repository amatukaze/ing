using System;
using System.Collections.Generic;

namespace Sakuno.ING.Game.Models
{
    public static class LinqHelper
    {
        public static Materials Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Materials> selector)
        {
            Materials result = default;
            foreach (var item in source)
                result += selector(item);
            return result;
        }

        public static AirFightPower Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, AirFightPower> selector)
        {
            AirFightPower result = default;
            foreach (var item in source)
                result += selector(item);
            return result;
        }

        public static LineOfSight Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, LineOfSight> selector)
        {
            LineOfSight result = default;
            foreach (var item in source)
                result += selector(item);
            return result;
        }
    }
}
