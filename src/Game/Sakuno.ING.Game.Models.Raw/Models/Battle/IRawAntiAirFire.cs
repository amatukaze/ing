using System.Collections.Generic;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Battle
{
    public interface IRawAntiAirFire
    {
        int Index { get; }
        int Type { get; }
        IReadOnlyList<EquipmentInfoId> EquipmentUsed { get; }
    }
}
