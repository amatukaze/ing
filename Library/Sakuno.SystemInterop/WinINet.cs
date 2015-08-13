using System;
using System.Runtime.InteropServices;

namespace Sakuno.SystemInterop
{
    partial class NativeMethods
    {
        public static class WinINet
        {
            const string DllName = "wininet.dll";

            [DllImport(DllName, SetLastError = true)]
            public static extern IntPtr FindFirstUrlCacheGroup(int dwFlags, int dwFilter, IntPtr lpSearchCondition, int dwSearchCondition, ref long lpGroupId, IntPtr lpReserved);
            [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern IntPtr FindFirstUrlCacheEntryW([MarshalAs(UnmanagedType.LPWStr)] string lpszUrlSearchPattern, IntPtr lpFirstCacheEntryInfo, ref int lpdwFirstCacheEntryInfoBufferSize);

            [DllImport(DllName, SetLastError = true)]
            public static extern bool FindNextUrlCacheGroup(IntPtr hFind, ref long lpGroupId, IntPtr lpReserved);
            [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern bool FindNextUrlCacheEntryW(IntPtr hFind, IntPtr lpNextCacheEntryInfo, ref int lpdwNextCacheEntryInfoBufferSize);

            [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern bool DeleteUrlCacheEntryW([MarshalAs(UnmanagedType.LPWStr)] string lpszUrlName);
            
            [DllImport(DllName, SetLastError = true)]
            public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);
        }
    }
}
