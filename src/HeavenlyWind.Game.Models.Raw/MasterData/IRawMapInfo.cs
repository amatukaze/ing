using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    public interface IRawMapInfo : IIdentifiable
    {
        int MapAreaId { get; }
        int CategoryNo { get; }

        string Name { get; }
        int StarDifficulty { get; }
        string OperationName { get; }
        string Description { get; }

        IReadOnlyCollection<int> ItemAcquirements { get; }
        int? RequiredDefeatCount { get; }

        IReadOnlyCollection<FleetType> AvailableFleetTypes { get; }

        IRawMapBgmInfo BgmInfo { get; }
    }
}
