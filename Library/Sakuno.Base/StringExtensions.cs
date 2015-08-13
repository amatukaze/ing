using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sakuno
{
    public static class StringExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this string rpString) => string.IsNullOrEmpty(rpString);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Join(this IEnumerable<string> rpStrings, string rpSeparator) => string.Join(rpSeparator, rpStrings);
    }
}
