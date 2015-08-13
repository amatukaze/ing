using System;

namespace Sakuno.SystemInterop
{
    public class VolumeChangedEventArgs : EventArgs
    {
        public bool IsMute { get; set; }
        public int Volume { get; set; }

        public VolumeChangedEventArgs(bool rpIsMute, int rpVolumn)
        {
            IsMute = rpIsMute;
            Volume = rpVolumn;
        }
    }
}
