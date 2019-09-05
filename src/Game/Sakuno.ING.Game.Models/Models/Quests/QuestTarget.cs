using System;
using System.Collections.Generic;

namespace Sakuno.ING.Game.Models.Quests
{
    public abstract class QuestTarget : BindableObject
    {
        protected QuestTarget(IStatePersist statePersist)
        {
            StatePersist = statePersist;
        }
        protected IStatePersist StatePersist { get; }
        public abstract IReadOnlyList<QuestCounter> Counters { get; }

        protected void UpdateProgress()
        {
            int current = 0, max = 0;
            foreach (var c in Counters)
            {
                current += c.Progress.Current;
                max += c.Progress.Max;
            }
            TotalProgress = (current, max);
        }

        private ClampedValue _totalProgress;
        public ClampedValue TotalProgress
        {
            get => _totalProgress;
            private set => Set(ref _totalProgress, value);
        }

        public void Check(DateTimeOffset timeStamp, QuestPeriod period)
        {
            foreach (var c in Counters)
                c.Check(StatePersist, timeStamp, period);
            UpdateProgress();
        }
    }
}
