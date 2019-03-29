using System;
using System.Linq;
using Sakuno.ING.Localization;

namespace Sakuno.ING.Game.Models
{
    public class QuestManager : BindableObject
    {
        public ILocalizationService Localization { get; }


        private DateTimeOffset _updationTime;
        public DateTimeOffset UpdationTime
        {
            get => _updationTime;
            set => Set(ref _updationTime, value);
        }

        internal QuestManager(GameProvider listener, ILocalizationService localization)
        {
            Localization = localization;

            _allQuests = new IdTable<QuestId, Quest, RawQuest, QuestManager>(this);
            _activeQuests = new OrderedSnapshotCollection<Quest>(_allQuests.Where(x => x.State != QuestState.Inactive));
            listener.QuestUpdated += (t, msg) =>
            {
                UpdationTime = t;
                _allQuests.RemoveAll(IsOutOfDate);
                _allQuests.BatchUpdate(msg.Quests, t, removal: false);
                _activeQuests.Update();
            };
            listener.QuestCompleted += (t, msg)
                => QuestCompleting?.Invoke(t, _allQuests.Remove(msg));
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

        private readonly IdTable<QuestId, Quest, RawQuest, QuestManager> _allQuests;
        public ITable<QuestId, Quest> AllQuests => _allQuests;
        public void Reset() => _allQuests.Clear();

        private readonly OrderedSnapshotCollection<Quest> _activeQuests;
        public IBindableCollection<Quest> ActiveQuests => _activeQuests;

        public event QuestCompletingHandler QuestCompleting;
    }

    public delegate void QuestCompletingHandler(DateTimeOffset timeStamp, Quest quest);
}
