using System.Collections.Generic;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Battle
{
    public interface IRawShipInBattle
    {
        ClampedValue HP { get; }
        IReadOnlyList<EquipmentInfoId> Equipments { get; }
        int Firepower { get; }
        int Torpedo { get; }
        int AntiAir { get; }
        int Armor { get; }
    }
}
