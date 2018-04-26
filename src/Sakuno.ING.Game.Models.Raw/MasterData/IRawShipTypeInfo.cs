using System.Collections.Generic;

namespace Sakuno.ING.Game.Models.MasterData
{
    public interface IRawShipTypeInfo : IIdentifiable
    {
        int SortNo { get; }
        string Name { get; }

        int RepairTimeRatio { get; }
        int BuildOutlineId { get; }
        IReadOnlyCollection<int> AvailableEquipmentTypes { get; }
    }
}
