using System.Collections.Generic;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public struct RawSide
    {
        public Formation Formation { get; set; }
        public IReadOnlyList<RawShipInBattle> Fleet { get; set; }
        public IReadOnlyList<RawShipInBattle> Fleet2 { get; set; }
        public Detection? Detection { get; set; }
        public EquipmentInfoId? NightTouchingId { get; set; }
        public int? FlareIndex { get; set; }
        public int? ActiveFleetId { get; set; }
    }
}
