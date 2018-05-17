using System;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public abstract class CountdownModelBase : DisposableModelBase
    {
        DateTimeOffset? r_TimeToComplete;
        public DateTimeOffset? TimeToComplete
        {
            get { return r_TimeToComplete; }
            protected set
            {
                if (r_TimeToComplete != value)
                {
                    r_TimeToComplete = value;
                    IsNotificated = false;
                    OnPropertyChanged(nameof(TimeToComplete));
                }
            }
        }
        TimeSpan? r_RemainingTime;
        public TimeSpan? RemainingTime
        {
            get { return r_RemainingTime; }
            protected set
            {
                if (r_RemainingTime != value)
                {
                    r_RemainingTime = value;
                    OnPropertyChanged(nameof(RemainingTime));
                }
            }
        }

        public virtual TimeSpan RemainingTimeToNotify => TimeSpan.Zero;
        public bool IsNotificated { get; protected set; }

        static event Action<long> Tick = delegate { };

        static CountdownModelBase()
        {
            Observable.Interval(TimeSpan.FromSeconds(1.0)).Subscribe(r => Tick?.Invoke(r));
        }
        public CountdownModelBase()
        {
            Tick += OnTickCore;
        }

        protected override void DisposeManagedResources()
        {
            Tick -= OnTickCore;
        }

        void OnTickCore(long rpTick)
        {
            if (!TimeToComplete.HasValue)
                RemainingTime = null;
            else
            {
                var rRemainingTime = TimeToComplete.Value - DateTimeOffset.Now;
                if (rRemainingTime.Ticks < 0L)
                    rRemainingTime = TimeSpan.Zero;

                RemainingTime = rRemainingTime;

                OnTick();

                if (rRemainingTime <= RemainingTimeToNotify && !IsNotificated)
                {
                    TimeOut();
                    IsNotificated = true;
                }
            }
        }

        protected virtual void OnTick() { }

        protected abstract void TimeOut();
    }

}
