using Sakuno.SystemInterop;
using System;

namespace Sakuno.KanColle.Amatsukaze.Services.Browser
{
    class BrowserVolume : ModelBase, IDisposable
    {
        VolumeSession r_Session;

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

        public BrowserVolume(VolumeSession rpSession)
        {
            r_Session = rpSession;

            r_IsMute = r_Session.IsMute;
            r_Volume = r_Session.Volume;

            r_Session.VolumeChanged += r =>
            {
                r_IsMute = r.IsMute;
                r_Volume = r.Volume;

                OnPropertyChanged(nameof(IsMute));
                OnPropertyChanged(nameof(Volume));
            };
        }

        public void Dispose()
        {
            r_Session.Dispose();
            r_Session = null;
        }
    }
}
