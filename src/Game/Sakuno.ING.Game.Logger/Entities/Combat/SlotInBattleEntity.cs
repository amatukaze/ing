using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Logger.Entities.Combat
{
    public struct SlotInBattleEntity
    {
        public EquipmentInfoId Id { readonly get; set; }
        public ClampedValue Count { readonly get; set; }
        public int ImprovementLevel { readonly get; set; }
        public int AirProficiency { readonly get; set; }
    }
}
