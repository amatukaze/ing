using System;
using System.Collections.Immutable;
using System.Linq;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Quests.Json
{
    internal class FleetRequirementDescription
    {
        public int RequiredCount { get; set; }
        public int? NoMoreThan { get; set; }
        public bool RequireFlagship { get; set; }
        public bool RequireAll { get; set; }
        public bool NoUpgrade { get; set; }
        public ImmutableArray<int?> ShipType { get; set; }
        public ImmutableArray<int> Ship { get; set; }

        public bool Satisfy(Fleet fleet)
        {
            bool SatisfyShip(Ship ship)
            {
                var info = ship?.Info;
                if (info is null) return false;
                foreach (var id in Ship)
                    if (NoUpgrade ?
                        info.Id == id :
                        info.CanUpgradeFrom((ShipInfoId)id))
                        return true;
                return false;
            }

            bool SatisfyType(Ship ship)
            {
                var type = ship?.Info?.Type;
                foreach (var id in ShipType)
                    if (type?.Id == id)
                        return true;
                return false;
            }

            if (fleet is null) return false;
            var satisfy = Ship.IsDefault ? (Func<Ship, bool>)SatisfyType : SatisfyShip;

            if (RequireAll)
                return fleet.Ships.All(satisfy);
            if (RequireFlagship && !satisfy(fleet.Ships.FirstOrDefault()))
                return false;
            int count = fleet.Ships.Count(satisfy);
            if (count > NoMoreThan)
                return false;
            return count >= RequiredCount;
        }
    }
}
