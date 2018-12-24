using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Sakuno.ING.Game.Models;

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

        public static T? ElementAtOrNull<T>(this T[] source, int index)
            where T : struct, IComparable<T>
        {
            if (source == null) return null;
            if (source.Length <= index) return null;
            T result = source[index];
            if (result.CompareTo(default) <= 0) return null;
            else return result;
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

        public static ShipId[] GetShipIds(this NameValueCollection source, string name)
            => source[name]?.Split(',').Select(x => (ShipId)int.Parse(x)).ToArray() ?? Array.Empty<ShipId>();

        public static EquipmentId[] GetEquipmentIds(this NameValueCollection source, string name)
            => source[name]?.Split(',').Select(x => (EquipmentId)int.Parse(x)).ToArray() ?? Array.Empty<EquipmentId>();
    }
}
