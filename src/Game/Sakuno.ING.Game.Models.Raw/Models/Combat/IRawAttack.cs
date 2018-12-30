using System.Collections.Generic;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public interface IRawAttack
    {
        int? SourceIndex { get; }
        bool IsEnemy { get; }
        int Type { get; }
        IReadOnlyList<EquipmentInfoId> EquipmentUsed { get; }
        IReadOnlyList<IRawHit> Hits { get; }
    }
}
