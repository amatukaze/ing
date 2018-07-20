using Sakuno.KanColle.Amatsukaze.Game.Proxy;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Tools
{
    class SessionToolViewModel : ModelBase
    {
        ITargetBlock<NetworkSession> _sessionReceiver;

        public ObservableCollection<SessionViewModel> Sessions { get; } = new ObservableCollection<SessionViewModel>();

        bool _isRecording;
        public bool IsRecording
        {
            get { return _isRecording; }
            set
            {
                if (_isRecording != value)
                {
                    _isRecording = value;
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

        public SessionToolViewModel()
        {
            if (Preference.Instance.Other.SessionTool.StartRecordingOnAppStartup)
                _isRecording = true;

            _sessionReceiver = new ActionBlock<NetworkSession>(session =>
            {
                Sessions.Add(new SessionViewModel(session));

                if (Sessions.Count > 50)
                    Sessions.RemoveAt(0);
            }, new ExecutionDataflowBlockOptions() { TaskScheduler = TaskScheduler.FromCurrentSynchronizationContext() });

            KanColleProxy.SessionSource.LinkTo(_sessionReceiver, session => _isRecording && !session.DisplayUrl.OICContains("ShimakazeGo"));
        }
    }
}
