using Sakuno.KanColle.Amatsukaze.Services;
using Sakuno.SystemInterop;
using Sakuno.UserInterface.Controls;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace Sakuno.KanColle.Amatsukaze.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        static MainWindow()
        {
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(int.MaxValue));
        }
        public MainWindow()
        {
            InitializeComponent();
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var rHandle = new WindowInteropHelper(this).Handle;

            PowerManager.RegisterMonitor(this);

            PanicKeyService.Instance.Initialize(rHandle);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var rAppName = StringResources.Instance.Main.Product_Name;
            if (MessageBox.Show(this, string.Format(StringResources.Instance.Main.Window_ClosingConfirmation, rAppName), rAppName, MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.No) == MessageBoxResult.No)
            {
                e.Cancel = true;
                return;
            }

            base.OnClosing(e);
        }
    }
}
