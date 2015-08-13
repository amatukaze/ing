using System;
using System.Runtime.InteropServices;

namespace Sakuno.SystemInterop
{
    partial class NativeInterfaces
    {
        [Guid("D666063F-1587-4E43-81F1-B948E807363F")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IMMDevice
        {
            [PreserveSig]
            int Activate([In] ref Guid iid, [In] uint dwClsCtx, [In] IntPtr pActivationParams, [Out][MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);
            [PreserveSig]
            int GetId([Out][MarshalAs(UnmanagedType.LPWStr)] out string ppstrId);
            [PreserveSig]
            int GetState([Out] out NativeEnums.DeviceState pdwState);

            // Need more implement
        }

        [Guid("0BD7A1BE-7A1A-44DB-8397-CC5392387B5E")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IMMDeviceCollection
        {
            [PreserveSig]
            int GetCount([Out] out uint pcDevices);
            [PreserveSig]
            int Item([In] uint nDevice, [Out] out IMMDevice Device);
        }

        [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IMMDeviceEnumerator
        {
            [PreserveSig]
            int EnumAudioEndpoints([In] NativeConstants.DataFlow dataFlow, [In] NativeEnums.DeviceState role, [Out][MarshalAs(UnmanagedType.Interface)] out IMMDeviceCollection ppDevices);
            [PreserveSig]
            int GetDefaultAudioEndpoint([In] NativeConstants.DataFlow dataFlow, [In] NativeConstants.Role role, [Out][MarshalAs(UnmanagedType.Interface)] out IMMDevice ppDevice);
            [PreserveSig]
            int GetDevice([In] string pwstrId, [Out] out IMMDevice ppDevice);
            [PreserveSig]
            int RegisterEndpointNotificationCallback([In] IntPtr pNotify);
            [PreserveSig]
            int UnregisterEndpointNotificationCallback([In] IntPtr pNotify);
        }
        [ComImport]
        [Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
        public class MMDeviceEnumerator { }

        [Guid("87CE5498-68D6-44E5-9215-6DA47EF883D8")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ISimpleAudioVolume
        {
            [PreserveSig]
            int SetMasterVolume([In] float fLevel, [In] ref Guid EventContext);
            [PreserveSig]
            int GetMasterVolume([Out] out float pfLevel);
            [PreserveSig]
            int SetMute([In] bool bMute, [In] ref Guid EventContext);
            [PreserveSig]
            int GetMute([Out] out bool pbMute);
        }

        [Guid("BFA971F1-4D5E-40BB-935E-967039BFBEE4")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAudioSessionManager
        {
            [PreserveSig]
            int GetAudioSessionControl([In] ref Guid AudioSessionGuid, [In] uint StreamFlags, [Out] out IAudioSessionControl SessionControl);
            [PreserveSig]
            int GetSimpleAudioVolume([In] ref Guid AudioSessionGuid, [In] uint StreamFlags, [Out] out ISimpleAudioVolume SimpleAudioVolume);
        }
        [Guid("77AA99A0-1BD6-484F-8BC7-2C654C9A9B6F")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAudioSessionManager2
        {
            [PreserveSig]
            int GetAudioSessionControl([In] ref Guid AudioSessionGuid, [In] uint StreamFlags, [Out] out IAudioSessionControl SessionControl);
            [PreserveSig]
            int GetSimpleAudioVolume([In] ref Guid AudioSessionGuid, [In] uint StreamFlags, [Out] out ISimpleAudioVolume SimpleAudioVolume);
            [PreserveSig]
            int GetSessionEnumerator([Out] out IAudioSessionEnumerator SessionEnum);
            [PreserveSig]
            int RegisterSessionNotification([In] IAudioSessionEnumerator IAudioSessionNotification);
            [PreserveSig]
            int UnregisterSessionNotification([In] IAudioSessionEnumerator IAudioSessionNotification);

            // Need more implement
        }

        [Guid("E2F5BB11-0570-40CA-ACDD-3AA01277DEE8")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAudioSessionEnumerator
        {
            [PreserveSig]
            int GetCount([Out] out int SessionCount);
            [PreserveSig]
            int GetSession([In] int SessionCount, [Out] out IAudioSessionControl Session);
        }

        [Guid("F4B1A599-7266-4319-A8CA-E70ACB11E8CD")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAudioSessionControl
        {
            [PreserveSig]
            int GetState([Out] out NativeConstants.AudioSessionState pRetVal);
            [PreserveSig]
            int GetDisplayName([Out][MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);
            [PreserveSig]
            int SetDisplayName([In][MarshalAs(UnmanagedType.LPWStr)] string Value, [In] Guid EventContext);
            [PreserveSig]
            int GetIconPath([Out][MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);
            [PreserveSig]
            int SetIconPath([MarshalAs(UnmanagedType.LPWStr)] string Value, [In] ref Guid EventContext);
            [PreserveSig]
            int GetGroupingParam([Out] out Guid pRetVal);
            [PreserveSig]
            int SetGroupingParam(ref Guid Override, [In] ref Guid EventContext);
            [PreserveSig]
            int RegisterAudioSessionNotification([In] IAudioSessionEvents NewNotifications);
            [PreserveSig]
            int UnregisterAudioSessionNotification([In] IAudioSessionEvents NewNotifications);
        }
        [Guid("BFB7FF88-7239-4FC9-8FA2-07C950BE9C6D")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAudioSessionControl2
        {
            [PreserveSig]
            int GetState([Out] out NativeConstants.AudioSessionState pRetVal);
            [PreserveSig]
            int GetDisplayName([Out][MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);
            [PreserveSig]
            int SetDisplayName([In][MarshalAs(UnmanagedType.LPWStr)] string Value, [In] ref Guid EventContext);
            [PreserveSig]
            int GetIconPath([Out][MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);
            [PreserveSig]
            int SetIconPath([MarshalAs(UnmanagedType.LPWStr)] string Value, [In] ref Guid EventContext);
            [PreserveSig]
            int GetGroupingParam([Out] out Guid pRetVal);
            [PreserveSig]
            int SetGroupingParam(ref Guid Override, [In] ref Guid EventContext);
            [PreserveSig]
            int RegisterAudioSessionNotification([In] IAudioSessionEvents NewNotifications);
            [PreserveSig]
            int UnregisterAudioSessionNotification([In] IAudioSessionEvents NewNotifications);
            [PreserveSig]
            int GetSessionIdentifier([Out][MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);
            [PreserveSig]
            int GetSessionInstanceIdentifier([Out][MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);
            [PreserveSig]
            int GetProcessId([Out] out uint pRetVal);
            [PreserveSig]
            int IsSystemSoundsSession();
            [PreserveSig]
            int SetDuckingPreference([In] bool optOut);
        }
        [Guid("24918ACC-64B3-37C1-8CA9-74A66E9957A8")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAudioSessionEvents
        {
            [PreserveSig]
            int OnDisplayNameChanged([In][MarshalAs(UnmanagedType.LPWStr)] string NewDisplayName, [In] ref Guid EventContext);
            [PreserveSig]
            int OnIconPathChanged([In][MarshalAs(UnmanagedType.LPWStr)] string NewIconPath, [In] ref Guid EventContext);
            [PreserveSig]
            int OnSimpleVolumeChanged([In] float NewVolume, [In] bool NewMute, [In] ref Guid EventContext);
            [PreserveSig]
            int OnChannelVolumeChanged([In] uint ChannelCount, [In] IntPtr NewChannelVolumeArray, [In] uint ChangedChannel, [In] ref Guid EventContext);
            [PreserveSig]
            int OnGroupingParamChanged([In] ref Guid NewGroupingParam, [In] ref Guid EventContext);
            [PreserveSig]
            int OnStateChanged([In] NativeConstants.AudioSessionState NewState);
            [PreserveSig]
            int OnSessionDisconnected([In] NativeConstants.AudioSessionDisconnectReason DisconnectReason);
        }
    }
}
