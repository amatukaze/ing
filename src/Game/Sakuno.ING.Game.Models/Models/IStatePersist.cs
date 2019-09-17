using System;
using System.Collections.Generic;
using Sakuno.ING.Game.Models.Quests;

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

        void SetQuestActive(QuestId questId, bool isActive, DateTimeOffset timeStamp);
        bool GetQuestActive(QuestId questId);
        DateTimeOffset? GetQuestTime(QuestId questId);

        void SetLastSortie(ShipId id, DateTimeOffset timeStamp);
        DateTimeOffset? GetLastSortie(ShipId id);
        void ClearLastSortie(ShipId id);
    }
}
