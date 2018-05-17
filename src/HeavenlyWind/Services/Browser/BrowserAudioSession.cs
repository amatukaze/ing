using Sakuno.SystemInterop;
using System;

namespace Sakuno.KanColle.Amatsukaze.Services.Browser
{
    class BrowserAudioSession : ModelBase, IDisposable
    {
        AudioSession r_Session;
        int r_BrowserProcess;

        int r_Volume;
        public int Volume
        {
            get { return r_Volume; }
            set
            {
                var rVolume = value.Clamp(-1, 101);
                if (r_Volume != rVolume)
                {
                    r_Volume = rVolume;
                    r_Session.Volume = rVolume;
                    OnPropertyChanged(nameof(Volume));
                }
            }
        }

        bool r_IsMute;
        public bool IsMute
        {
            get { return r_IsMute; }
            set
            {
                if (r_IsMute != value)
                {
                    r_IsMute = value;
                    r_Session.IsMute = value;
                    OnPropertyChanged(nameof(IsMute));
                }
            }
        }

        public BrowserAudioSession(AudioSession rpSession)
        {
            r_Session = rpSession;
            r_BrowserProcess = r_Session.ProcessID;

            r_IsMute = r_Session.IsMute;
            r_Volume = r_Session.Volume;

            r_Session.VolumeChanged += Session_VolumeChanged;
        }

        void Session_VolumeChanged(object sender, AudioSessionVolumeChangedEventArgs e)
        {
            r_IsMute = e.IsMute;
            r_Volume = e.Volume;

            OnPropertyChanged(nameof(IsMute));
            OnPropertyChanged(nameof(Volume));
        }

        public void Dispose()
        {
            r_Session.VolumeChanged -= Session_VolumeChanged;

            r_Session.Dispose();
            r_Session = null;
        }
    }
}
