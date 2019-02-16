using System.Collections.Generic;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public abstract class RawAttack
    {
        protected RawAttack(int? sourceIndex, bool enemyAttacks, int type, IReadOnlyList<EquipmentInfoId> equipmentUsed)
        {
            SourceIndex = sourceIndex;
            EnemyAttacks = enemyAttacks;
            Type = type;
            EquipmentUsed = equipmentUsed;
        }

        public int? SourceIndex { get; }
        public bool EnemyAttacks { get; }
        public int Type { get; }
        public IReadOnlyList<EquipmentInfoId> EquipmentUsed { get; }
        public abstract IReadOnlyList<RawHit> Hits { get; }
    }

    public readonly struct RawHit
    {
        public int TargetIndex { get; }
        public int Damage { get; }
        public bool IsCritical { get; }
        public bool IsProtection { get; }

        public RawHit(int targetIndex, decimal damageNumber, int criticalNumber)
        {
            TargetIndex = targetIndex;
            Damage = (int)damageNumber;
            IsProtection = damageNumber > (int)damageNumber;
            IsCritical = criticalNumber == 2;
        }
    }

    internal class ComboAttack : RawAttack
    {
        public ComboAttack(int? sourceIndex, bool enemyAttacks, int type, IReadOnlyList<EquipmentInfoId> equipmentUsed, IReadOnlyList<RawHit> hits)
            : base(sourceIndex, enemyAttacks, type, equipmentUsed)
            => Hits = hits;

        public override IReadOnlyList<RawHit> Hits { get; }
    }

    internal class SingleAttack : RawAttack
    {
        public SingleAttack(int? sourceIndex, bool enemyAttacks, int type, IReadOnlyList<EquipmentInfoId> equipmentUsed, RawHit hit)
            : base(sourceIndex, enemyAttacks, type, equipmentUsed)
            => Hit = hit;

        public RawHit Hit { get; }
        public override IReadOnlyList<RawHit> Hits => new[] { Hit };
    }
}
