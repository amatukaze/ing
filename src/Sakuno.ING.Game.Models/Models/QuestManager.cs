using System;
using Sakuno.ING.Game.Events;

namespace Sakuno.ING.Game.Models
{
    public class QuestManager : ITableProvider
    {
        internal QuestManager(GameListener listener)
        {
            _allQuests = new IdTable<Quest, IRawQuest>(this);
            listener.QuestUpdated.Received += QuestUpdated;
            listener.QuestCompleted.Received += msg => _allQuests.Remove(msg.Message.QuestId);
        }

        private void QuestUpdated(ITimedMessage<QuestPageUpdate> msg)
        {
            _allQuests.BatchUpdate(msg.Message.Quests, removal: false);
            foreach (var raw in msg.Message.Quests)
                _allQuests[raw.Id].CreationTime = msg.TimeStamp;
            _allQuests.RemoveAll(q => IsOutOfDate(q, msg.TimeStamp));
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

        private readonly IdTable<Quest, IRawQuest> _allQuests;
        public ITable<Quest> AllQuests => _allQuests;

        public void Reset() => _allQuests.Clear();

        public ITable<T> TryGetTable<T>()
        {
            if (typeof(T) == typeof(Quest))
                return (ITable<T>)AllQuests;

            return null;
        }
    }
}
