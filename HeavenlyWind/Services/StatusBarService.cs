using Sakuno.KanColle.Amatsukaze.Models;
using Sakuno.UserInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

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

        public IList<UIZoomInfo> UIZoomFactors { get; private set; }

        double r_UIZoom;
        public double UIZoom
        {
            get { return r_UIZoom; }
            set
            {
                if (r_UIZoom != value)
                {
                    r_UIZoom = value;
                    OnPropertyChanged(nameof(UIZoom));
                }
            }
        }

        public ICommand UISetZoomCommand { get; private set; }
        public ICommand UIZoomInCommand { get; private set; }
        public ICommand UIZoomOutCommand { get; private set; }

        StatusBarService()
        {
            var rPropertyChangedSource = Observable.FromEventPattern<PropertyChangedEventArgs>(this, nameof(PropertyChanged))
                .Select(r => r.EventArgs.PropertyName);
            rPropertyChangedSource.Where(r => r == nameof(IsMessageObsolete)).Select(_ => IsMessageObsolete).Where(r => !r)
                .Throttle(TimeSpan.FromSeconds(30.0)).Subscribe(_ => IsMessageObsolete = true);
        }

        public void Initialize()
        {
            Logger.LogAdded += r => Message = r.Content;

            UIZoom = Preference.Current.UI.Zoom;

            UISetZoomCommand = new DelegatedCommand<double>(SetZoom);
            UIZoomFactors = new[] { .25, .5, .75, 1.0, 1.25, 1.5, 1.75, 2.0, 3.0, 4.0 }.Select(r => new UIZoomInfo(r, UISetZoomCommand)).ToArray();
            UIZoomInCommand = new DelegatedCommand(() => SetZoom(UIZoom + .05));
            UIZoomOutCommand = new DelegatedCommand(() => SetZoom(UIZoom - .05));
        }

        void SetZoom(double rpZoom)
        {
            if (rpZoom < .25)
                return;

            UpdateZoomSelection(rpZoom);

            UIZoom = rpZoom;
            Preference.Current.UI.Zoom = rpZoom;
        }
        void UpdateZoomSelection(double rpZoom)
        {
            foreach (var rInfo in UIZoomFactors)
                rInfo.IsSelected = rInfo.Zoom == rpZoom;
        }
    }
}
