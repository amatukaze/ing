using System.Windows;
using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze.UWP.Bridge
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void UpdatePort(object sender, RoutedEventArgs args)
        {
            port.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void CancelPort(object sender, RoutedEventArgs args)
        {
            port.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }

        private void Exit(object sender, RoutedEventArgs args)
        {
            Program.Notify.Visible = false;
            Application.Current.Shutdown();
        }
    }
}
