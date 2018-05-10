using System;
using Sakuno.ING.Game.Events;

namespace Sakuno.ING.Game.Models
{
    public class QuestManager
    {
        internal QuestManager(IGameProvider listener)
        {
            _allQuests = new IdTable<int, Quest, IRawQuest, QuestManager>(this);
            listener.QuestUpdated += QuestUpdated;
            listener.QuestCompleted += (_, msg) => _allQuests.Remove(msg);
        }

        private void QuestUpdated(DateTimeOffset timeStamp, QuestPageUpdate msg)
        {
            _allQuests.BatchUpdate(msg.Quests, removal: false);
            foreach (var raw in msg.Quests)
                _allQuests[raw.Id].CreationTime = timeStamp;
            _allQuests.RemoveAll(q => IsOutOfDate(q, timeStamp));
        }

        private static bool IsOutOfDate(Quest quest, DateTimeOffset timeStamp)
        {
            var questDate = quest.CreationTime.ToOffset(questUpdate);
            var updateDate = timeStamp.ToOffset(questUpdate);
            switch (quest.Period)
            {
                case QuestPeriod.Daily:
                    return questDate.Date != updateDate.Date;
                case QuestPeriod.Weekly:
                    return questDate.Ticks / (TimeSpan.TicksPerDay * 7)
                        != updateDate.Ticks / (TimeSpan.TicksPerDay * 7);
                case QuestPeriod.Monthly:
                    return questDate.Year != updateDate.Year
                        || questDate.Month != updateDate.Month;
                case QuestPeriod.Quarterly:
                    return questDate.Year * 4 + (questDate.Month + 1) / 3
                        != updateDate.Year * 4 + (questDate.Month + 1) / 3;
                case QuestPeriod.Once:
                    return false;
                default:
                    throw new InvalidOperationException($"Unknown quest period {(int)quest.Period}");
            }
        }

        private static readonly TimeSpan questUpdate = TimeSpan.FromHours(4);

        private readonly IdTable<int, Quest, IRawQuest, QuestManager> _allQuests;
        public ITable<int, Quest> AllQuests => _allQuests;

        public void Reset() => _allQuests.Clear();
    }
}
