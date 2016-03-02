using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Sakuno.KanColle.Amatsukaze.Views.Preferences
{
    /// <summary>
    /// VersionInfo.xaml の相互作用ロジック
    /// </summary>
    public partial class VersionInfo : UserControl
    {
        public VersionInfo()
        {
            InitializeComponent();

            AddHandler(Hyperlink.ClickEvent, (RoutedEventHandler)HyperlinkClicked);
        }

        void HyperlinkClicked(object sender, RoutedEventArgs e)
        {
            var rHyperlink = e.OriginalSource as Hyperlink;
            if (rHyperlink == null)
                return;

            try
            {
                Process.Start(rHyperlink.NavigateUri.ToString());
            }
            catch { }
        }
    }
}
