using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Logger.Entities.Combat
{
    public struct EquipmentInBattleEntity
    {
        public EquipmentInfoId Id { get; set; }
        public int Count { get; set; }
        public int MaxCount { get; set; }
        public int ImprovementLevel { get; set; }
        public int AirProficiency { get; set; }
    }
}
