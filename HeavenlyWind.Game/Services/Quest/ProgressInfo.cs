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
                if (r_Progress != value)
                {
                    r_Progress = value;
                    UpdateTime = DateTimeOffset.Now;
                    IsDirty = true;
                    OnPropertyChanged(nameof(Progress));
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
