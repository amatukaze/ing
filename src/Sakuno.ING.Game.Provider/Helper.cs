using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Sakuno.ING.Game
{
    internal static class Helper
    {
        public static T ElementAtOrDefault<T>(this IReadOnlyList<T> source, int index)
        {
            if (source == null) return default;
            if (source.Count <= index) return default;
            return source[index];
        }

        public static int GetInt(this NameValueCollection source, string name)
        {
            int.TryParse(source[name], out int r);
            return r;
        }

        public static bool GetBool(this NameValueCollection source, string name)
            => source.GetInt(name) != 0;

        public static int[] GetInts(this NameValueCollection source, string name)
            => source[name]?.Split(',').Select(int.Parse).ToArray() ?? Array.Empty<int>();
    }
}
