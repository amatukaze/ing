using System;
using System.Runtime.InteropServices;

namespace Sakuno.SystemInterop
{
    partial class NativeMethods
    {
        public static class Gdi32
        {
            const string DllName = "gdi32.dll";

            [DllImport(DllName, SetLastError = true)]
            public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

            [DllImport(DllName)]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

            [DllImport(DllName)]
            public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
            [return: MarshalAs(UnmanagedType.Bool)]
            [DllImport(DllName, SetLastError = true)]
            public static extern bool DeleteObject(IntPtr hObject);

            [return: MarshalAs(UnmanagedType.Bool)]
            [DllImport(DllName, SetLastError = true)]
            public static extern bool DeleteDC(IntPtr hdc);

            [DllImport(DllName, SetLastError = true)]
            public static extern int GetDIBits(IntPtr hdc, IntPtr hbmp, uint uStartScan, uint cScanLines, [Out] byte[] lpvBits, ref NativeStructs.BITMAPINFO lpbi, int uUsage);

            [DllImport(DllName, SetLastError = true)]
            public static extern IntPtr CreateDIBSection(IntPtr hdc, ref NativeStructs.BITMAPINFO pbmi, uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);
        }
    }
}
