using System.Collections.Generic;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Battle
{
    public readonly struct RawAttack
    {
        public RawAttack(int? sourceIndex, bool isEnemy, int type, IReadOnlyList<EquipmentInfoId> equipments, IReadOnlyList<RawHit> hits)
        {
            SourceIndex = sourceIndex;
            IsEnemy = isEnemy;
            Type = type;
            Equipments = equipments;
            Hits = hits;
        }

        public int? SourceIndex { get; }
        public bool IsEnemy { get; }
        public int Type { get; }
        public IReadOnlyList<EquipmentInfoId> Equipments { get; }
        public IReadOnlyList<RawHit> Hits { get; }
    }
}
