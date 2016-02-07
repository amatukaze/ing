using Sakuno.SystemInterop;
using Sakuno.UserInterface;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Sakuno.KanColle.Amatsukaze.Services.Browser
{
    class BrowserHost : HwndHost
    {
        IntPtr r_Handle;

        public bool IsExtracted { get; private set; }

        public BrowserHost(IntPtr rpHandle)
        {
            r_Handle = rpHandle;

            BrowserService.Instance.Messages.SubscribeOnDispatcher(CommunicatorMessages.InvalidateArrange, _ => InvalidateArrange());
            BrowserService.Instance.Messages.SubscribeOnDispatcher(CommunicatorMessages.LoadCompleted, r =>
            {
                IsExtracted = false;
                InvalidateArrange();
            });
            BrowserService.Instance.Messages.SubscribeOnDispatcher(CommunicatorMessages.LoadGamePageCompleted, r =>
            {
                IsExtracted = true;
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

            if (IsExtracted)
            {
                var rZoom = DpiUtil.ScaleX + Preference.Current.Browser.Zoom - 1.0;

                rWidth = Math.Min(rWidth, GameConstants.GameWidth * rZoom / DpiUtil.ScaleX / DpiUtil.ScaleX);
                rHeight = Math.Min(rHeight, GameConstants.GameHeight * rZoom / DpiUtil.ScaleY / DpiUtil.ScaleY);
            }

            NativeMethods.User32.PostMessageW(r_Handle, CommunicatorMessages.ResizeBrowserWindow, (IntPtr)rWidth, (IntPtr)rHeight);

            return new Size(rWidth, rHeight);
        }
        protected override Size MeasureOverride(Size rpConstraint)
        {
            var rWidth = rpConstraint.Width;
            var rHeight = rpConstraint.Height;

            if (IsExtracted)
            {
                var rZoom = DpiUtil.ScaleX + Preference.Current.Browser.Zoom - 1.0;

                rWidth = GameConstants.GameWidth * rZoom / DpiUtil.ScaleX / DpiUtil.ScaleX;
                rHeight = GameConstants.GameHeight * rZoom / DpiUtil.ScaleY / DpiUtil.ScaleY;
            }

            return new Size(rWidth, rHeight);
        }
    }
}
