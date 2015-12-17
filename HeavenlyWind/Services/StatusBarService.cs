using Sakuno.KanColle.Amatsukaze.Models;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public class StatusBarService : ModelBase
    {
        public static StatusBarService Instance { get; } = new StatusBarService();

        string r_Message;
        public string Message
        {
            get { return r_Message; }
            set
            {
                var rMessage = value.Replace(Environment.NewLine, " ");
                if (r_Message != rMessage)
                {
                    r_Message = rMessage;
                    OnPropertyChanged(nameof(Message));
                    IsMessageObsolete = false;
                }
            }
        }

        bool r_IsMessageObsolete = true;
        public bool IsMessageObsolete
        {
            get { return r_IsMessageObsolete; }
            private set
            {
                r_IsMessageObsolete = value;
                OnPropertyChanged(nameof(IsMessageObsolete));
            }
        }

        public Power Power { get; } = new Power();

        StatusBarService()
        {
            var rPropertyChangedSource = Observable.FromEventPattern<PropertyChangedEventArgs>(this, nameof(PropertyChanged))
                .Select(r => r.EventArgs.PropertyName);
            rPropertyChangedSource.Where(r => r == nameof(IsMessageObsolete)).Select(_ => IsMessageObsolete).Where(r => !r)
                .Throttle(TimeSpan.FromSeconds(30.0)).Subscribe(_ => IsMessageObsolete = true);
        }

        public void Initialize()
        {
            Logger.LogAdded += r => Message = $"{r.Time}: {r.Content}";
        }
    }
}
