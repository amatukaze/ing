using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData.Raw
{
    public interface IRawShipTypeInfo
    {
        int Id { get; }
        int SortNo { get; }
        string Name { get; }

        int RepairTimeRatio { get; }
        int BuildOutlineId { get; }
        IReadOnlyList<int> AvailableEquipmentTypes { get; }
    }
}
