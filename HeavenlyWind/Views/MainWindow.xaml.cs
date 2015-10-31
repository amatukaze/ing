using System.ComponentModel;
using Sakuno.UserInterface.Controls;
using System.Windows;

namespace Sakuno.KanColle.Amatsukaze.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (MessageBox.Show(this, StringResources.Instance.Main.Window_ClosingConfirmation, "HeavenlyWind", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.No) == MessageBoxResult.No)
            {
                e.Cancel = true;
                return;
            }

            base.OnClosing(e);
        }

    }
}
