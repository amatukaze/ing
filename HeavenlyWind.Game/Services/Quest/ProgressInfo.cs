using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest
{
    public class ProgressInfo : ModelBase
    {
        public int ID { get; }

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
                    OnPropertyChanged(nameof(Progress));
                }
            }
        }
        public DateTimeOffset UpdateTime { get; internal set; }

        internal bool IsDirty { get; set; }

        internal ProgressInfo(int rpID, int rpProgress) : this(rpID, rpProgress, DateTimeOffset.Now) { }
        internal ProgressInfo(int rpID, int rpProgress, DateTimeOffset rpUpdateTime)
        {
            Progress = rpProgress;
            UpdateTime = rpUpdateTime;
        }
    }
}
