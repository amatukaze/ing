using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events.Shipyard
{
    public class EquipmentCreation
    {
        public EquipmentCreation(bool isSuccess, IRawEquipment equipment, int selectedEquipentInfoId, Materials consumption)
        {
            IsSuccess = isSuccess;
            Equipment = equipment;
            SelectedEquipentInfoId = selectedEquipentInfoId;
            Consumption = consumption;
        }

        public bool IsSuccess { get; }
        public IRawEquipment Equipment { get; }
        public int SelectedEquipentInfoId { get; }
        public Materials Consumption { get; }
    }
}
