using System;
using System.Runtime.InteropServices;

namespace Sakuno.SystemInterop
{
    public class VolumeManager : NativeInterfaces.IAudioSessionEvents
    {
        public int ProcessID { get; private set; }

        public int Volume
        {
            get
            {
                var rSimpleAudioVolume = GetSimpleAudioVolume();
                if (rSimpleAudioVolume == null)
                    return 0;

                float rResult;
                Marshal.ThrowExceptionForHR(rSimpleAudioVolume.GetMasterVolume(out rResult));
                Marshal.ReleaseComObject(rSimpleAudioVolume);
                return (int)(rResult * 100);
            }
            set
            {
                var rSimpleAudioVolume = GetSimpleAudioVolume();
                if (rSimpleAudioVolume == null) 
                    return;

                var rGuid = Guid.Empty;
                Marshal.ThrowExceptionForHR(rSimpleAudioVolume.SetMasterVolume((float)(value / 100.0), ref rGuid));
                Marshal.ReleaseComObject(rSimpleAudioVolume);
            }
        }
        public bool IsMute
        {
            get
            {
                var rSimpleAudioVolume = GetSimpleAudioVolume();
                if (rSimpleAudioVolume == null)
                    return false;

                bool rResult;
                Marshal.ThrowExceptionForHR(rSimpleAudioVolume.GetMute(out rResult));
                Marshal.ReleaseComObject(rSimpleAudioVolume);
                return rResult;
            }
            set
            {
                var rSimpleAudioVolume = GetSimpleAudioVolume();
                if (rSimpleAudioVolume == null)
                    return;

                var rGuid = Guid.Empty;
                Marshal.ThrowExceptionForHR(rSimpleAudioVolume.SetMute(value, ref rGuid));
                Marshal.ReleaseComObject(rSimpleAudioVolume);
            }
        }

        public string DisplayName
        {
            get
            {
                var rSessionControl = (NativeInterfaces.IAudioSessionControl2)GetSimpleAudioVolume();
                if (rSessionControl == null)
                    return null;

                string rResult;
                Marshal.ThrowExceptionForHR(rSessionControl.GetDisplayName(out rResult));
                Marshal.ReleaseComObject(rSessionControl);
                return rResult;
            }
            set
            {
                var rSessionControl = (NativeInterfaces.IAudioSessionControl2)GetSimpleAudioVolume();
                if (rSessionControl == null)
                    return;

                var rGuid = Guid.Empty;
                Marshal.ThrowExceptionForHR(rSessionControl.SetDisplayName(value, ref rGuid));
                Marshal.ReleaseComObject(rSessionControl);
            }
        }

        public event Action<VolumeChangedEventArgs> VolumeChanged = delegate { };

        public VolumeManager(int rpProcessID)
        {
            ProcessID = rpProcessID;
        }

        NativeInterfaces.ISimpleAudioVolume GetSimpleAudioVolume()
        {
            var rDeviceEnumerator = (NativeInterfaces.IMMDeviceEnumerator)new NativeInterfaces.MMDeviceEnumerator();
            NativeInterfaces.IMMDevice rDevice;
            Marshal.ThrowExceptionForHR(rDeviceEnumerator.GetDefaultAudioEndpoint(NativeConstants.DataFlow.Render, NativeConstants.Role.Multimedia, out rDevice));

            var rAudioSessionManagerGuid = typeof(NativeInterfaces.IAudioSessionManager2).GUID;
            object rObject;
            Marshal.ThrowExceptionForHR(rDevice.Activate(ref rAudioSessionManagerGuid, 0, IntPtr.Zero, out rObject));

            var rSessionManager = (NativeInterfaces.IAudioSessionManager2)rObject;
            NativeInterfaces.IAudioSessionEnumerator rSessionEnumerator;
            Marshal.ThrowExceptionForHR(rSessionManager.GetSessionEnumerator(out rSessionEnumerator));

            int rCount;
            Marshal.ThrowExceptionForHR(rSessionEnumerator.GetCount(out rCount));
            for (var i = 0; i < rCount; i++)
            {
                NativeInterfaces.IAudioSessionControl rSessionControl;
                Marshal.ThrowExceptionForHR(rSessionEnumerator.GetSession(i, out rSessionControl));

                var rSessionControl2 = (NativeInterfaces.IAudioSessionControl2)rSessionControl;
                uint rProcessID;
                Marshal.ThrowExceptionForHR(rSessionControl2.GetProcessId(out rProcessID));

                if (rProcessID == ProcessID)
                {
                    Marshal.ThrowExceptionForHR(rSessionControl.RegisterAudioSessionNotification(this));

                    return (NativeInterfaces.ISimpleAudioVolume)rSessionControl2;
                }

                if (rSessionControl2 != null)
                    Marshal.ReleaseComObject(rSessionControl2);
            }

            Marshal.ReleaseComObject(rSessionEnumerator);
            Marshal.ReleaseComObject(rSessionManager);
            Marshal.ReleaseComObject(rDevice);
            Marshal.ReleaseComObject(rDeviceEnumerator);

            return null;
        }

        int NativeInterfaces.IAudioSessionEvents.OnDisplayNameChanged(string NewDisplayName, ref Guid EventContext)
        {
            return 0;
        }

        int NativeInterfaces.IAudioSessionEvents.OnIconPathChanged(string NewIconPath, ref Guid EventContext)
        {
            return 0;
        }

        int NativeInterfaces.IAudioSessionEvents.OnSimpleVolumeChanged(float NewVolume, bool NewMute, ref Guid EventContext)
        {
            VolumeChanged(new VolumeChangedEventArgs(NewMute, (int)(100 * NewVolume)));

            return 0;
        }

        int NativeInterfaces.IAudioSessionEvents.OnChannelVolumeChanged(uint ChannelCount, IntPtr NewChannelVolumeArray, uint ChangedChannel, ref Guid EventContext)
        {
            return 0;
        }

        int NativeInterfaces.IAudioSessionEvents.OnGroupingParamChanged(ref Guid NewGroupingParam, ref Guid EventContext)
        {
            return 0;
        }

        int NativeInterfaces.IAudioSessionEvents.OnStateChanged(NativeConstants.AudioSessionState NewState)
        {
            return 0;
        }

        int NativeInterfaces.IAudioSessionEvents.OnSessionDisconnected(NativeConstants.AudioSessionDisconnectReason DisconnectReason)
        {
            return 0;
        }
    }
}
