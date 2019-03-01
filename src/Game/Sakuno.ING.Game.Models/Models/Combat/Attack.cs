using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public class Attack
    {
        public Attack(BattleParticipant source, AttackType type, IEnumerable<EquipmentInfo> equipmentUsed, IEnumerable<Hit> hits)
        {
            Source = source;
            Type = type;
            EquipmentUsed = equipmentUsed.ToArray();
            Hits = hits.ToArray();
        }
        public BattleParticipant Source { get; }
        public AttackType Type { get; }
        public IReadOnlyList<EquipmentInfo> EquipmentUsed { get; }
        public IReadOnlyList<Hit> Hits { get; }
    }

    public class Hit
    {
        public Hit(BattleParticipant target, RawHit raw)
        {
            Target = target;
            IsCritical = raw.IsCritical;
            IsProtection = raw.IsProtection;
            Damage = raw.Damage;
            FromHP = target.FromHP;
            (ToHP, Recover) = target.DoDamage(Damage);
        }

        public BattleParticipant Target { get; }
        public bool IsCritical { get; }
        public bool IsProtection { get; }
        public int Damage { get; }
        public int FromHP { get; }
        public int ToHP { get; }
        public bool Recover { get; }
    }
}
