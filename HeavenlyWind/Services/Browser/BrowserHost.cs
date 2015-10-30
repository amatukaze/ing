﻿using Sakuno.SystemInterop;
using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows;
using Sakuno.UserInterface;

namespace Sakuno.KanColle.Amatsukaze.Services.Browser
{
    class BrowserHost : HwndHost
    {
        IntPtr r_Handle;

        bool r_IsExtracted;
        double r_LastWidth, r_LastHeight;

        public BrowserHost(IntPtr rpHandle)
        {
            r_Handle = rpHandle;

            BrowserService.Instance.Messages.SubscribeOnDispatcher(CommunicatorMessages.InvalidateArrange, _ => InvalidateArrange());
            BrowserService.Instance.Messages.SubscribeOnDispatcher(CommunicatorMessages.ExtractionResult, r =>
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
                rWidth = Math.Min(rWidth, 800);
                rHeight = Math.Min(rHeight, 480);
            }

            if (r_LastWidth != rWidth && r_LastHeight != rHeight)
            {
                BrowserService.Instance.Communicator.Write(CommunicatorMessages.Resize + $":{rWidth};{rHeight}");

                r_LastWidth = rWidth;
                r_LastHeight = rHeight;
            }

            return new Size(rWidth, rHeight);
        }
        protected override Size MeasureOverride(Size rpConstraint)
        {
            var rWidth = rpConstraint.Width;
            var rHeight = rpConstraint.Height;

            if (r_IsExtracted)
            {
                var rZoom = DpiUtil.ScaleX + Preference.Current.Browser.Zoom - 1.0;

                rWidth = 800 * rZoom / DpiUtil.ScaleX / DpiUtil.ScaleX;
                rHeight = 480 * rZoom / DpiUtil.ScaleY / DpiUtil.ScaleY;
            }

            return new Size(rWidth, rHeight);
        }
    }
}