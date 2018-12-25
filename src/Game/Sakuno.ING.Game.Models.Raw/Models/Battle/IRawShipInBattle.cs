using System.Collections.Generic;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Battle
{
    public interface IRawShipInBattle
    {
        ShipInfoId Id { get; }
        int Level { get; }
        ClampedValue HP { get; }
        IReadOnlyList<EquipmentInfoId> Equipment { get; }
        int Firepower { get; }
        int Torpedo { get; }
        int AntiAir { get; }
        int Armor { get; }
    }
}
