using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public interface IRawEquipmentInBattle
    {
        EquipmentInfoId Id { get; }
        int ImprovementLevel { get; }
        int AirProficiency { get; }
    }
}
