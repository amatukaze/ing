using Sakuno.SystemInterop;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace Sakuno.KanColle.Amatsukaze.Views.Tools
{
    class ScreenshotToolOverlayWindow : Border
    {
        HwndSource r_HwndSource;

        public ScreenshotToolOverlayWindow()
        {
            BorderBrush = Brushes.Red;
            BorderThickness = new Thickness(4);
        }

        public void Show(Int32Rect rpRect)
        {
            var rMainWindowHandle = new WindowInteropHelper(App.Current.MainWindow).Handle;

            if (r_HwndSource == null)
            {
                var rParam = new HwndSourceParameters(nameof(ScreenshotToolOverlayWindow))
                {
                    Width = 0,
                    Height = 0,
                    PositionX = 0,
                    PositionY = 0,
                    WindowStyle = 0,
                    UsesPerPixelOpacity = true,
                    HwndSourceHook = WndProc,
                    ParentWindow = rMainWindowHandle,
                };

                r_HwndSource = new HwndSource(rParam) { SizeToContent = SizeToContent.Manual, RootVisual = this };
            }

            var rBrowserWindowHandle = NativeMethods.User32.GetWindow(rMainWindowHandle, NativeConstants.GetWindow.GW_CHILD);

            NativeStructs.RECT rBrowserWindowRect;
            NativeMethods.User32.GetWindowRect(rBrowserWindowHandle, out rBrowserWindowRect);

            NativeMethods.User32.SetWindowPos(r_HwndSource.Handle, IntPtr.Zero, rBrowserWindowRect.Left + rpRect.X, rBrowserWindowRect.Top + rpRect.Y, rpRect.Width, rpRect.Height, NativeEnums.SetWindowPosition.SWP_NOZORDER | NativeEnums.SetWindowPosition.SWP_NOACTIVATE | NativeEnums.SetWindowPosition.SWP_SHOWWINDOW);
        }

        IntPtr WndProc(IntPtr rpHandle, int rpMessage, IntPtr rpWParam, IntPtr rpLParam, ref bool rrpHandled)
        {
            if (rpMessage == (int)NativeConstants.WindowMessage.WM_NCHITTEST)
            {
                rrpHandled = true;
                return (IntPtr)NativeConstants.HitTest.HTTRANSPARENT;
            }

            return IntPtr.Zero;
        }

        public void Hide()
        {
            r_HwndSource.Dispose();
            r_HwndSource = null;
        }
    }
}
