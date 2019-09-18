using System;
using System.Collections.Immutable;
using System.Linq;
using Sakuno.ING.Game.Models.Combat;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Quests.Json
{
    internal class QuestCounterDescriptionJson
    {
        public string Type { get; set; }
        public int Count { get; set; }
        public QuestPeriod? PeriodOverride { get; set; }
        public ImmutableArray<int> ShipType { get; set; }
        public MapDescriptor? Map { get; set; }
        public BattleRank? RankRequired { get; set; }
        public ImmutableArray<int> Expedition { get; set; }
        public ImmutableArray<int> EquipmentType { get; set; }
        public ImmutableArray<int> Equipment { get; set; }
        public ImmutableArray<FleetRequirementDescription> Fleet { get; set; }
    }

    internal class FleetRequirementDescription
    {
        public int RequiredCount { get; set; }
        public bool RequireFlagship { get; set; }
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

            if (RequireFlagship && !satisfy(fleet.Ships.FirstOrDefault()))
                return false;
            return fleet.Ships.Count(satisfy) >= RequiredCount;
        }
    }
}
