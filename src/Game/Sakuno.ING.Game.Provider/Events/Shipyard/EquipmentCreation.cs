using System.Collections.Generic;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events.Shipyard
{
    public sealed class EquipmentCreation
    {
        public EquipmentCreation(bool isSuccess, IReadOnlyCollection<RawEquipment> equipment, Materials consumption)
        {
            IsSuccess = isSuccess;
            Equipment = equipment;
            Consumption = consumption;
        }

        public bool IsSuccess { get; }
        public IReadOnlyCollection<RawEquipment> Equipment { get; }
        public Materials Consumption { get; }
    }
}
