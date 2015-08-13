using System.Runtime.InteropServices;

namespace Sakuno.SystemInterop
{
    partial class NativeMethods
    {
        public static class Kernel32
        {
            const string DllName = "kernel32.dll";

            [return: MarshalAs(UnmanagedType.Bool)]
            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern bool DeleteFileW([MarshalAs(UnmanagedType.LPWStr)] string lpFileName);
        }
    }
}
