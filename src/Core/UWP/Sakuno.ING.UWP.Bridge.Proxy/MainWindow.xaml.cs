using System.Windows;
using System.Windows.Controls;

namespace Sakuno.ING.UWP.Bridge
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

        private void Update(object sender, RoutedEventArgs args)
        {
            useUpstream.GetBindingExpression(CheckBox.IsCheckedProperty).UpdateSource();
            upstream.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            port.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            listening.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            Program.Worker.Update();
        }

        private void Cancel(object sender, RoutedEventArgs args)
        {
            useUpstream.GetBindingExpression(CheckBox.IsCheckedProperty).UpdateTarget();
            upstream.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            port.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            listening.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }

        private void Exit(object sender, RoutedEventArgs args)
        {
            Program.Notify.Visible = false;
            Application.Current.Shutdown();
        }
    }
}
