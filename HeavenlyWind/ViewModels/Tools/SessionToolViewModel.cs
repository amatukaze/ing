using Sakuno.KanColle.Amatsukaze.Game.Proxy;
using Sakuno.UserInterface;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Reactive.Linq;
using Sakuno.UserInterface.Commands;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Tools
{
    public class SessionToolViewModel : ModelBase
    {
        static readonly object r_ThreadLockSync = new object();

        public ObservableCollection<SessionViewModel> Sessions { get; } = new ObservableCollection<SessionViewModel>();

        bool r_IsRecording;
        public bool IsRecording
        {
            get { return r_IsRecording; }
            set
            {
                if (r_IsRecording != value)
                {
                    r_IsRecording = value;
                    OnPropertyChanged(nameof(IsRecording));
                }
            }
        }

        bool r_AutoScroll = true;
        public bool AutoScroll
        {
            get { return r_AutoScroll; }
            set
            {
                if (r_AutoScroll != value)
                {
                    r_AutoScroll = value;
                    OnPropertyChanged(nameof(AutoScroll));
                }
            }
        }

        public ICommand ClearCommand { get; }

        public SessionToolViewModel()
        {
            if (Preference.Instance.Other.SessionTool.StartRecordingOnAppStartup)
                r_IsRecording = true;

            ClearCommand = new DelegatedCommand(() => Sessions.Clear());

            KanColleProxy.SessionSubject.ObserveOnDispatcher().Subscribe(r =>
            {
                if (!r_IsRecording || r.DisplayUrl.OICContains("ShimakazeGo"))
                    return;

                lock (r_ThreadLockSync)
                {
                    Sessions.Add(new SessionViewModel(r));

                    if (Sessions.Count > 50)
                        Sessions.RemoveAt(0);
                }
            });
        }
    }
}
