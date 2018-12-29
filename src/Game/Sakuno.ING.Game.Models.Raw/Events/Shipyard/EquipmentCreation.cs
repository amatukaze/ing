using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events.Shipyard
{
    public sealed class EquipmentCreation
    {
        public EquipmentCreation(bool isSuccess, IRawEquipment equipment, EquipmentInfoId selectedEquipentInfoId, Materials consumption)
        {
            IsSuccess = isSuccess;
            Equipment = equipment;
            SelectedEquipentInfoId = selectedEquipentInfoId;
            Consumption = consumption;
        }

        public bool IsSuccess { get; }
        public IRawEquipment Equipment { get; }
        public EquipmentInfoId SelectedEquipentInfoId { get; }
        public Materials Consumption { get; }
    }
}
