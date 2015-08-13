using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Sakuno
{
    public static class ArrayExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlyCollection<T> AsReadOnly<T>(this T[] rpArray) => Array.AsReadOnly(rpArray);
    }
}
