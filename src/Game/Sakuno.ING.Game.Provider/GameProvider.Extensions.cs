using Sakuno.ING.Game.Models;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;

namespace Sakuno.ING.Game
{
    internal static class SvDataObservableExtensions
    {
        public static int GetInt(this NameValueCollection source, string name) => int.Parse(source[name]);
        public static int[] GetInts(this NameValueCollection source, string name) =>
            source[name]?.Split(',').Select(int.Parse).ToArray() ?? Array.Empty<int>();
        public static bool GetBool(this NameValueCollection source, string name) => source.GetInt(name) != 0;

        public static ShipId[] GetShipIds(this NameValueCollection source, string name) =>
            source[name]?.Split(',').Select(id => (ShipId)int.Parse(id)).ToArray() ?? Array.Empty<ShipId>();
        public static SlotItemId[] GetSlotItemIds(this NameValueCollection source, string name) =>
            source[name]?.Split(',').Select(x => (SlotItemId)int.Parse(x)).ToArray() ?? Array.Empty<SlotItemId>();
    }
}
