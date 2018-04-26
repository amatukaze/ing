using System.Collections.Generic;

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
    }
}
