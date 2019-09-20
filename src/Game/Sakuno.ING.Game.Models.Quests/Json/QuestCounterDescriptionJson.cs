using System.Collections.Immutable;
using Sakuno.ING.Game.Models.Combat;

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
}
