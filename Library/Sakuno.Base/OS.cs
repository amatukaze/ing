using System;

namespace Sakuno
{
    public static class OS
    {
        public static bool IsWindows => Environment.OSVersion.Platform == PlatformID.Win32NT;
        public static bool Is64Bit => IntPtr.Size == 8;
    }
}
