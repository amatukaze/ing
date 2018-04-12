using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData.Raw
{
    public interface IRawMapInfo
    {
        int Id { get; }
        int MapAreaId { get; }
        int CategoryNo { get; }

        string Name { get; }
        int StarDifficulty { get; }
        string OperationName { get; }
        string Description { get; }

        IReadOnlyCollection<int> ItemAcquirements { get; }
        int? RequiredDefeatCount { get; }

        IReadOnlyCollection<FleetType> AvailableFleetTypes { get; }
    }
}
