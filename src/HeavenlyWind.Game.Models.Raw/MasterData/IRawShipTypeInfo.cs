using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    public interface IRawShipTypeInfo : IIdentifiable
    {
        int SortNo { get; }
        string Name { get; }

        int RepairTimeRatio { get; }
        int BuildOutlineId { get; }
        IReadOnlyList<int> AvailableEquipmentTypes { get; }
    }
}
