using System.Collections.Generic;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public class RawShipInBattle
    {
        public ShipInfoId Id { get; internal set; }
        public int Level { get; internal set; }
        public ShipHP HP { get; internal set; }
        public IReadOnlyList<EquipmentInfoId> Equipment { get; internal set; }
        public int Firepower { get; internal set; }
        public int Torpedo { get; internal set; }
        public int AntiAir { get; internal set; }
        public int Armor { get; internal set; }
        public bool IsEscaped { get; internal set; }
    }
}
