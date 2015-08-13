namespace Sakuno.SystemInterop
{
    partial class NativeConstants
    {
        public enum DataFlow
        {
            Render,
            Capture,
            All,
            DataFlow_EnumCount,
        }
        public enum Role
        {
            Console,
            Multimedia,
            Communications,
            Role_EnumCount,
        }
        public enum AudioSessionState
        {
            AudioSessionStateInactive,
            AudioSessionStateActive,
            AudioSessionStateExpired
        }
        public enum AudioSessionDisconnectReason
        {
            DisconnectReasonDeviceRemoval,
            DisconnectReasonServerShutdown,
            DisconnectReasonFormatChanged,
            DisconnectReasonSessionLogoff,
            DisconnectReasonSessionDisconnected,
            DisconnectReasonExclusiveModeOverride,
        }
    }
}
