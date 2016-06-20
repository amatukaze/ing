using Sakuno.SystemInterop;
using Sakuno.UserInterface;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Sakuno.KanColle.Amatsukaze.Services.Browser
{
    class BrowserHost : HwndHost
    {
        IntPtr r_Handle;

        public bool IsExtracted { get; private set; }

        ScaleTransform r_ScaleTransform;

        public BrowserHost(IntPtr rpHandle)
        {
            r_Handle = rpHandle;

            BrowserService.Instance.Messages.SubscribeOnDispatcher(CommunicatorMessages.InvalidateArrange, _ => InvalidateArrange());

            LayoutTransform = r_ScaleTransform = new ScaleTransform(1.0 / StatusBarService.Instance.UIZoom, 1.0 / StatusBarService.Instance.UIZoom);

            var rPCEL = PropertyChangedEventListener.FromSource(StatusBarService.Instance);
            rPCEL.Add(nameof(StatusBarService.Instance.UIZoom), (s, e) => r_ScaleTransform.ScaleX = r_ScaleTransform.ScaleY = 1.0 / StatusBarService.Instance.UIZoom);
        }

        protected override HandleRef BuildWindowCore(HandleRef rpParentHandle)
        {
            NativeMethods.User32.SetParent(r_Handle, rpParentHandle.Handle);

            return new HandleRef(this, r_Handle);
        }

        protected override void DestroyWindowCore(HandleRef rpHandle) { }

        internal void ResizeBrowserToFitGame()
        {
            IsExtracted = true;
            InvalidateMeasure();
            InvalidateArrange();
        }

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

                rWidth = Math.Min(rWidth, GameConstants.GameWidth * rZoom / DpiUtil.ScaleX / DpiUtil.ScaleX);
                rHeight = Math.Min(rHeight, GameConstants.GameHeight * rZoom / DpiUtil.ScaleY / DpiUtil.ScaleY);
            }

            return new Size(rWidth, rHeight);
        }
    }
}
