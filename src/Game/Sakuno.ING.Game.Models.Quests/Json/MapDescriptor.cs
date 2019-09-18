using System;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Quests.Json
{
    public readonly struct MapDescriptor
    {
        private readonly int area;
        private readonly int? no;
        private readonly bool plus;

        public MapDescriptor(int area, int? no, bool plus)
        {
            this.area = area;
            this.no = no;
            this.plus = plus;
        }

        public MapDescriptor(string str)
        {
            var parts = str.Split('-');
            if (parts.Length != 2) throw new FormatException();
            area = int.Parse(parts[0]);
            if (parts[1] == "*")
            {
                no = null;
                plus = false;
            }
            else if (parts[1][parts[1].Length - 1] == '+')
            {
                plus = true;
                no = int.Parse(parts[1].Substring(0, parts[1].Length - 1));
            }
            else
            {
                plus = false;
                no = int.Parse(parts[1]);
            }
        }

        public bool Satisfy(MapId mapId)
        {
            if (mapId.AreaId != area) return false;
            return (no, plus) switch
            {
                (null, _) => true,
                (int n, false) => mapId.CategoryNo == n,
                (int n, true) => mapId.CategoryNo >= n
            };
        }

        public override string ToString()
            => (no, plus) switch
            {
                (null, _) => $"{area}-*",
                (int n, false) => $"{area}-{n}",
                (int n, true) => $"{area}-{n}+"
            };
    }
}
