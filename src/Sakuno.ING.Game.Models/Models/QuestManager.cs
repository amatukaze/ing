using System;
using Sakuno.ING.Game.Events;

namespace Sakuno.ING.Game.Models
{
    public class QuestManager : BindableObject
    {
        private DateTimeOffset _updationTime;
        public DateTimeOffset UpdationTime
        {
            get => _updationTime;
            set => Set(ref _updationTime, value);
        }

        internal QuestManager(IGameProvider listener)
        {
            _allQuests = new IdTable<QuestId, Quest, IRawQuest, QuestManager>(this);
            listener.QuestUpdated += QuestUpdated;
            listener.QuestCompleted += (t, msg) => _allQuests.Remove(msg);
        }

        private void QuestUpdated(DateTimeOffset timeStamp, QuestPageUpdate msg)
        {
            UpdationTime = timeStamp;
            _allQuests.BatchUpdate(msg.Quests, timeStamp, removal: false);
            _allQuests.RemoveAll(IsOutOfDate);
        }

        private bool IsOutOfDate(Quest quest)
        {
            var questDate = quest.UpdationTime.ToOffset(questUpdate);
            var updateDate = UpdationTime.ToOffset(questUpdate);
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

        private readonly IdTable<QuestId, Quest, IRawQuest, QuestManager> _allQuests;
        public ITable<QuestId, Quest> AllQuests => _allQuests;

        public void Reset() => _allQuests.Clear();
    }
}
