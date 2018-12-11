using System.Collections.Generic;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models
{
    public interface IRawRewards
    {
        IReadOnlyCollection<UseItemRecord> UseItem { get; }
        IReadOnlyCollection<EquipmentRecord> Equipment { get; }
        IReadOnlyCollection<FurnitureId> Furniture { get; }
        IReadOnlyCollection<ShipInfoId> Ship { get; }
    }
}
