using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public abstract class CountdownModelBase : ModelBase, IDisposable
    {
        static IConnectableObservable<long> r_Interval;
        static IDisposable r_IntervalSubscription;

        DateTimeOffset? r_CompleteTime;
        public DateTimeOffset? CompleteTime
        {
            get { return r_CompleteTime; }
            set
            {
                if (r_CompleteTime != value)
                {
                    r_CompleteTime = value;
                    IsNotificated = false;
                    OnPropertyChanged();
                }
            }
        }
        TimeSpan? r_RemainingTime;
        public TimeSpan? RemainingTime
        {
            get { return r_RemainingTime; }
            set
            {
                if (r_RemainingTime != value)
                {
                    r_RemainingTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public virtual TimeSpan RemainingTimeToNotify => TimeSpan.Zero;
        public bool IsNotificated { get; protected set; }
        
        static CountdownModelBase()
        {
            r_Interval = Observable.Interval(TimeSpan.FromSeconds(1.0)).Publish();
            r_Interval.Connect();
        }
        public CountdownModelBase()
        {
            r_IntervalSubscription = r_Interval.Subscribe(_ => OnTick());
        }

        public void Dispose()
        {
            if (r_IntervalSubscription != null)
            {
                r_IntervalSubscription.Dispose();
                r_IntervalSubscription = null;
            }
        }

        void OnTick()
        {
            if (!CompleteTime.HasValue)
                RemainingTime = null;
            else
            {
                var rRemainingTime = CompleteTime.Value - DateTimeOffset.Now;
                if (rRemainingTime.Ticks < 0L)
                    rRemainingTime = TimeSpan.Zero;

                RemainingTime = rRemainingTime;

                if (rRemainingTime <= RemainingTimeToNotify && !IsNotificated)
                {
                    TimeOut();
                    IsNotificated = true;
                }
            }
        }

        protected abstract void TimeOut();
    }

}
