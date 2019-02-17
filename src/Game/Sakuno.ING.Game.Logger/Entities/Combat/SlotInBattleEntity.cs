using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Logger.Entities.Combat
{
    public struct SlotInBattleEntity
    {
        public EquipmentInfoId Id { get; set; }
        public ClampedValue Count { get; set; }
        public int ImprovementLevel { get; set; }
        public int AirProficiency { get; set; }
    }
}
