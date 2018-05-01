using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events.Shipyard
{
    public class EquipmentCreation
    {
        public bool IsSuccess { get; internal set; }
        public IRawEquipment Equipment { get; internal set; }
        public int SelectedEquipentInfoId { get; internal set; }
        public Materials Consumption { get; internal set; }
    }
}
