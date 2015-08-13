using System;

namespace Sakuno.SystemInterop
{
    partial class NativeEnums
    {
        [Flags]
        public enum DeviceState
        {
            Active = 1,
            Disabled = 2,
            NotPresent = 4,
            Unplugged = 8,
            All = 15,
        }

        [Flags]
        public enum EndpointHardwareSupport
        {
            Volume = 1,
            Mute = 2,
            Meter = 4,
        }
    }
}
