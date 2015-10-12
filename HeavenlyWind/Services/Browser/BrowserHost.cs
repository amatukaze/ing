using Sakuno.SystemInterop;
using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows;
using Sakuno.UserInterface;
using Sakuno.KanColle.Amatsukaze.Models;

namespace Sakuno.KanColle.Amatsukaze.Services.Browser
{
    class BrowserHost : HwndHost
    {
        IntPtr r_Handle;

        bool r_IsExtracted;

        public BrowserHost(IntPtr rpHandle)
        {
            r_Handle = rpHandle;

            BrowserService.Instance.Messages.SubscribeOnDispatcher("InvalidateArrange", _ => InvalidateArrange());
            BrowserService.Instance.Messages.SubscribeOnDispatcher("ExtractFlashResult", r =>
            {
                r_IsExtracted = bool.Parse(r);
                InvalidateArrange();
            });
        }

        protected override HandleRef BuildWindowCore(HandleRef rpParentHandle)
        {
            NativeMethods.User32.SetParent(r_Handle, rpParentHandle.Handle);

            return new HandleRef(this, r_Handle);
        }

        protected override void DestroyWindowCore(HandleRef rpHandle) { }

        protected override Size ArrangeOverride(Size rpFinalSize)
        {
            var rWidth = rpFinalSize.Width;
            var rHeight = rpFinalSize.Height;

            if (r_IsExtracted)
            {
                var rZoom = DpiUtil.ScaleX + Preference.Current.Browser.Zoom - 1.0;

                rWidth = 800 * rZoom / DpiUtil.ScaleX / DpiUtil.ScaleX;
                rHeight = 480 * rZoom / DpiUtil.ScaleY / DpiUtil.ScaleY;
            }
            
            BrowserService.Instance.Communicator.Write($"Resize:{rWidth};{rHeight}");

            return new Size(rWidth, rHeight);
        }
    }
}
