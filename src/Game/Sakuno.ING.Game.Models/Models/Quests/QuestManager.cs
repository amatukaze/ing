using System;

namespace Sakuno.ING.Game.Models.Quests
{
    public class QuestManager : BindableObject
    {
        private DateTimeOffset _updationTime;
        public DateTimeOffset UpdationTime
        {
            get => _updationTime;
            set => Set(ref _updationTime, value);
        }

        internal QuestManager(GameProvider listener)
        {
            _allQuests = new IdTable<QuestId, Quest, RawQuest, QuestManager>(this);
            _activeQuests = new OrderedSnapshotCollection<Quest>(_allQuests, x => x.State != QuestState.Inactive);
            listener.QuestUpdated += (t, msg) =>
            {
                UpdationTime = t;
                _allQuests.RemoveAll(IsOutOfDate);
                _allQuests.BatchUpdate(msg.Quests, t, removal: false);
            };
            listener.QuestCompleted += (t, msg) =>
            {
                var quest = _allQuests.Remove(msg);
                QuestCompleting?.Invoke(t, quest);
            };
        }

        private bool IsOutOfDate(Quest quest)
        {
            var questDate = quest.UpdationTime.ToOffset(questUpdate);
            var updateDate = UpdationTime.ToOffset(questUpdate);
            return quest.Period switch
            {
                QuestPeriod.Daily => questDate.Date != updateDate.Date,
                QuestPeriod.Weekly => questDate.Ticks / (TimeSpan.TicksPerDay * 7)
                    != updateDate.Ticks / (TimeSpan.TicksPerDay * 7),
                QuestPeriod.Monthly => (questDate.Year, questDate.Month)
                    != (updateDate.Year, updateDate.Month),
                QuestPeriod.Quarterly => questDate.Year * 4 + (questDate.Month + 1) / 3
                    != updateDate.Year * 4 + (questDate.Month + 1) / 3,
                QuestPeriod.Once => false,
                _ => false // Unknown quest period {(int)quest.Period}
            };
        }

        private static readonly TimeSpan questUpdate = TimeSpan.FromHours(4);

        private readonly IdTable<QuestId, Quest, RawQuest, QuestManager> _allQuests;
        public ITable<QuestId, Quest> AllQuests => _allQuests;
        public void Reset() => _allQuests.Clear();

        private readonly OrderedSnapshotCollection<Quest> _activeQuests;
        public IBindableCollection<Quest> ActiveQuests => _activeQuests;

        public event QuestCompletingHandler QuestCompleting;
    }

    public delegate void QuestCompletingHandler(DateTimeOffset timeStamp, Quest quest);
}
