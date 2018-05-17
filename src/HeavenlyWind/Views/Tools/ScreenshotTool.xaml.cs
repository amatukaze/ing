using Sakuno.KanColle.Amatsukaze.ViewModels.Tools;
using System;
using System.Windows;

namespace Sakuno.KanColle.Amatsukaze.Views.Tools
{
    /// <summary>
    /// ScreenshotTool.xaml の相互作用ロジック
    /// </summary>
    partial class ScreenshotTool
    {
        static ScreenshotTool r_Current;

        internal ScreenshotTool()
        {
            InitializeComponent();
        }

        public static void Open()
        {
            if (r_Current != null)
                r_Current.Activate();
            else
            {
                r_Current = new ScreenshotTool() { Owner = Application.Current.MainWindow, DataContext = new ScreenshotToolViewModel() };
                r_Current.Show();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            r_Current = null;

            base.OnClosed(e);
        }
    }
}
