using System;
using System.Runtime.InteropServices;

namespace Sakuno.SystemInterop
{
    partial class NativeMethods
    {
        public static class DwmApi
        {
            const string DllName = "dwmapi.dll";

            [return: MarshalAs(UnmanagedType.Bool)]
            [DllImport(DllName, PreserveSig = false)]
            public static extern bool DwmIsCompositionEnabled();

            [DllImport(DllName, PreserveSig = false)]
            public static extern int DwmEnableComposition([MarshalAs(UnmanagedType.Bool)] bool uCompositionAction);

            [DllImport(DllName, PreserveSig = false)]
            public static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref NativeStructs.MARGINS pMarInset);
        }
    }
}
