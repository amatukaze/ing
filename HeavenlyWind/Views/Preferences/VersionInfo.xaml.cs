using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

namespace Sakuno.KanColle.Amatsukaze.Views.Preferences
{
    /// <summary>
    /// VersionInfo.xaml の相互作用ロジック
    /// </summary>
    partial class VersionInfo
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
