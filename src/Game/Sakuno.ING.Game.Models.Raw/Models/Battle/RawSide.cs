using System.Collections.Generic;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Battle
{
    public readonly struct RawSide
    {
        public RawSide(Formation formation, IReadOnlyList<IRawShipInBattle> fleet, IReadOnlyList<IRawShipInBattle> fleet2, Detection? detection, EquipmentInfoId? nightTouchingId, int? flareIndex)
        {
            Formation = formation;
            Fleet = fleet;
            Fleet2 = fleet2;
            Detection = detection;
            NightTouchingId = nightTouchingId;
            FlareIndex = flareIndex;
        }

        public Formation Formation { get; }
        public IReadOnlyList<IRawShipInBattle> Fleet { get; }
        public IReadOnlyList<IRawShipInBattle> Fleet2 { get; }
        public Detection? Detection { get; }
        public EquipmentInfoId? NightTouchingId { get; }
        public int? FlareIndex { get; }
    }
}
