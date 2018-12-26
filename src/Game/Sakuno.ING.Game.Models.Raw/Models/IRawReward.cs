using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models
{
    public interface IRawReward
    {
        bool TryGetUseItem(out UseItemRecord useItem);
        bool TryGetEquipment(out EquipmentRecord equipment);
        bool TryGetFurniture(out FurnitureId furniture);
        bool TryGetShip(out ShipInfoId ship);
    }
}
