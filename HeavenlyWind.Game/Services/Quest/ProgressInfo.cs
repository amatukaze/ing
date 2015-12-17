using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest
{
    public class ProgressInfo : ModelBase
    {
        public QuestInfo Quest { get; }

        public QuestState State { get; internal set; }

        int r_Progress;
        public int Progress
        {
            get { return r_Progress; }
            internal set
            {
                var rProgress = Math.Min(value, Quest.Total);
                if (r_Progress != rProgress)
                {
                    r_Progress = rProgress;
                    UpdateTime = DateTimeOffset.Now;
                    IsDirty = true;
                    OnPropertyChanged(nameof(Progress));

                    RecordService.Instance.QuestProgress.UpdateRecord(this);
                }
            }
        }
        public DateTimeOffset UpdateTime { get; internal set; }

        internal bool IsDirty { get; set; }

        internal ProgressInfo(int rpID, QuestState rpState, int rpProgress) : this(rpID, rpState, rpProgress, DateTimeOffset.Now) { }
        internal ProgressInfo(int rpID, QuestState rpState, int rpProgress, DateTimeOffset rpUpdateTime)
        {
            Quest = QuestProgressService.Instance.Infos[rpID];

            State = rpState;
            Progress = rpProgress;
            UpdateTime = rpUpdateTime;
        }
    }
}
