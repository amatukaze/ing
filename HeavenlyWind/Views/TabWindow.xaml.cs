using Sakuno.SystemInterop;
using System;
using System.Windows.Interop;

namespace Sakuno.KanColle.Amatsukaze.Views
{
    /// <summary>
    /// TabWindow.xaml の相互作用ロジック
    /// </summary>
    partial class TabWindow
    {
        public TabWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var rHandle = new WindowInteropHelper(this).Handle;

            var rSystemMenu = NativeMethods.User32.GetSystemMenu(rHandle, false);
            NativeMethods.User32.EnableMenuItem(rSystemMenu, NativeConstants.SystemCommand.SC_MINIMIZE, NativeEnums.MF.MF_GRAYED);

            NativeStructs.POINT rCursorPoint;
            NativeMethods.User32.GetCursorPos(out rCursorPoint);

            Left = rCursorPoint.X;
            Top = rCursorPoint.Y;
        }
    }
}
