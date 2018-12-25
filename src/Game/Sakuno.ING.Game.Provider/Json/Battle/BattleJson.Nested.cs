using System.Collections.Generic;
using Sakuno.ING.Game.Models.Battle;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json.Battle
{
    partial class BattleJson
    {
        public class ComboAttack : IRawAttack
        {
            public int? SourceIndex { get; set; }
            public bool IsEnemy { get; set; }
            public int Type { get; set; }
            public IReadOnlyList<EquipmentInfoId> EquipmentUsed { get; set; }
            public readonly List<Hit> hits = new List<Hit>();
            public IReadOnlyList<IRawHit> Hits => hits;
        }
        public class Hit : IRawHit
        {
            public int TargetIndex { get; set; }
            public double damage;
            public int Damage => (int)damage;
            public bool IsCritical { get; set; }
            public bool IsProtection => damage > (int)damage;
        }
        public class SingleAttack : IRawAttack
        {
            public int? SourceIndex { get; set; }
            public bool IsEnemy { get; set; }
            public int Type { get; set; }
            public IReadOnlyList<EquipmentInfoId> EquipmentUsed { get; set; }
            public Hit Hit { get; set; }
            public IReadOnlyList<IRawHit> Hits => new[] { Hit };
        }
        public class PartialHit
        {
            public double damage;
            public bool critical, torpedo, diving;
        }
    }
}
