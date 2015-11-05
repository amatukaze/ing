using Sakuno.SystemInterop;
using System;

namespace Sakuno.KanColle.Amatsukaze.Views
{
    /// <summary>
    /// TabWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class TabWindow
    {
        public TabWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            NativeStructs.POINT rCursorPoint;
            NativeMethods.User32.GetCursorPos(out rCursorPoint);

            Left = rCursorPoint.X;
            Top = rCursorPoint.Y;
        }
    }
}
