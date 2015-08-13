using System;
using System.Runtime.CompilerServices;

namespace Sakuno
{
    public static class ComparableExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGreaterThan<T>(this T rpLeft, T rpRight) where T : IComparable<T> => rpLeft.CompareTo(rpRight) > 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGreaterThanOrEqualTo<T>(this T rpLeft, T rpRight) where T : IComparable<T> => rpLeft.CompareTo(rpRight) >= 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLessThan<T>(this T rpLeft, T rpRight) where T : IComparable<T> => rpLeft.CompareTo(rpRight) < 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLessThanOrEqualTo<T>(this T rpLeft, T rpRight) where T : IComparable<T> => rpLeft.CompareTo(rpRight) <= 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEqualTo<T>(this T rpLeft, T rpRight) where T : IComparable<T> => rpLeft.CompareTo(rpRight) == 0;

        public static T Clamp<T>(this T rpValue, T rpMin, T rpMax) where T : IComparable<T>
        {
            var rResult = rpValue;
            if (rpMin.IsGreaterThan(rpMax))
                throw new ArgumentOutOfRangeException("Min must be less than or equal to max.");

            if (rpValue.IsGreaterThan(rpMax))
                rResult = rpMax;
            if (rpValue.IsLessThan(rpMin))
                rResult = rpMin;
            return rResult;
        }
    }
}
