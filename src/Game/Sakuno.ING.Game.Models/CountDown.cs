using System;

namespace Sakuno.ING.Game
{
    public sealed class CountDown : BindableObject
    {
        private DateTimeOffset? _completion;
        public DateTimeOffset? Completion
        {
            get => _completion;
            private set => Set(ref _completion, value);
        }

        private TimeSpan? _remaining;
        public TimeSpan? Remaining
        {
            get => _remaining;
            private set => Set(ref _remaining, value);
        }

        internal void Init(DateTimeOffset? completion, DateTimeOffset timeStamp)
        {
            using (EnterBatchNotifyScope())
            {
                Completion = completion;
                Update(timeStamp);
            }
        }

        internal void Clear()
        {
            using (EnterBatchNotifyScope())
            {
                Completion = null;
                Remaining = null;
            }
        }

        internal bool Update(DateTimeOffset timeStamp)
        {
            var lastRemaining = Remaining;

            if (Completion == null)
                Remaining = null;
            else if (timeStamp > Completion)
                Remaining = default(TimeSpan);
            else
                Remaining = Completion - timeStamp;

            return lastRemaining > default(TimeSpan) && Remaining == default(TimeSpan);
        }
    }
}
