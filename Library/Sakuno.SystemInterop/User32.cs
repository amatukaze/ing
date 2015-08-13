using System;
using System.Runtime.InteropServices;

namespace Sakuno.SystemInterop
{
    partial class NativeMethods
    {
        public static class User32
        {
            const string DllName = "user32.dll";

            [DllImport(DllName)]
            public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

            [return: MarshalAs(UnmanagedType.Bool)]
            [DllImport(DllName)]
            public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, NativeEnums.SetWindowPosition uFlags);

            [return: MarshalAs(UnmanagedType.Bool)]
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool GetWindowRect(IntPtr hWnd, out NativeStructs.RECT lpRect);

            #region Window Long
            public static IntPtr GetWindowLongPtr(IntPtr hWnd, NativeConstants.GetWindowLong nIndex)
            {
                return new IntPtr(OS.Is64Bit ? WindowLong.GetWindowLongPtrW(hWnd, nIndex) : WindowLong.GetWindowLongW(hWnd, nIndex));
            }
            public static IntPtr SetWindowLongPtr(IntPtr hWnd, NativeConstants.GetWindowLong nIndex, IntPtr dwNewLong)
            {
                return OS.Is64Bit ? WindowLong.SetWindowLongPtrW(hWnd, nIndex, dwNewLong) : new IntPtr(WindowLong.SetWindowLongW(hWnd, nIndex, dwNewLong.ToInt32()));
            }
            public static IntPtr GetClassLongPtr(IntPtr hWnd, NativeConstants.GetClassLong nIndex)
            {
                return new IntPtr(OS.Is64Bit ? WindowLong.GetClassLongPtrW(hWnd, nIndex) : WindowLong.GetClassLongW(hWnd, nIndex));
            }
            public static IntPtr SetClassLongPtr(IntPtr hWnd, NativeConstants.GetClassLong nIndex, IntPtr dwNewLong)
            {
                return OS.Is64Bit ? WindowLong.SetClassLongPtrW(hWnd, nIndex, dwNewLong) : new IntPtr(WindowLong.SetClassLongW(hWnd, nIndex, dwNewLong.ToInt32()));
            }
            static class WindowLong
            {
                [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
                public static extern int GetWindowLongW(IntPtr hWnd, NativeConstants.GetWindowLong nIndex);
                [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
                public static extern long GetWindowLongPtrW(IntPtr hWnd, NativeConstants.GetWindowLong nIndex);

                [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
                public static extern int SetWindowLongW(IntPtr hWnd, NativeConstants.GetWindowLong nIndex, int dwNewLong);
                [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
                public static extern IntPtr SetWindowLongPtrW(IntPtr hWnd, NativeConstants.GetWindowLong nIndex, IntPtr dwNewLong);

                [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
                public static extern int GetClassLongW(IntPtr hWnd, NativeConstants.GetClassLong nIndex);
                [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
                public static extern long GetClassLongPtrW(IntPtr hWnd, NativeConstants.GetClassLong nIndex);

                [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
                public static extern int SetClassLongW(IntPtr hWnd, NativeConstants.GetClassLong nIndex, int dwNewLong);
                [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
                public static extern IntPtr SetClassLongPtrW(IntPtr hWnd, NativeConstants.GetClassLong nIndex, IntPtr dwNewLong);
            }
            #endregion

            #region SystemParametersInfo
            [DllImport(DllName, CharSet = CharSet.Auto)]
            public static extern bool SystemParametersInfo(NativeConstants.SPI uiAction, int uiParam, ref NativeStructs.RECT pvParam, int fWinIni);

            #endregion

            #region Device Context
            [DllImport(DllName, SetLastError = true)]
            public static extern IntPtr GetDC(IntPtr hWnd);
            [DllImport(DllName)]
            public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);
            #endregion
        }
    }
}
