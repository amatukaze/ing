using System;
using System.Collections.Generic;

namespace Sakuno.ING.Game.Models
{
    public interface IStatePersist
    {
        void SaveChanges();
        void Initialize(int admiralId);

        DateTimeOffset LastHomeportUpdate { get; set; }
        DateTimeOffset LastHomeportRepair { get; set; }
        DateTimeOffset? LastSortieTime { get; set; }
        IReadOnlyList<FleetId> LastSortieFleets { get; set; }
        Materials ConsumptionBeforeSortie { get; set; }

        void SetQuestProgress(QuestId questId, int counterId, int value);
        int? GetQuestProgress(QuestId questId, int counterId);
        void ClearQuestProgress(QuestId questId);

        void SetQuestActive(QuestId questId, bool isActive);
        bool GetQuestActive(QuestId questId);

        void SetQuestTime(QuestId questId, int counterId, DateTimeOffset time);
        DateTimeOffset? GetQuestTime(QuestId questId, int counterId);

        void SetLastSortie(ShipId id, DateTimeOffset timeStamp);
        DateTimeOffset? GetLastSortie(ShipId id);
        void ClearLastSortie(ShipId id);
    }
}
