using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

namespace Sakuno.KanColle.Amatsukaze.Views.Tools
{
    /// <summary>
    /// CompositionSharingWindow.xaml の相互作用ロジック
    /// </summary>
    partial class CompositionSharingWindow
    {
        public CompositionSharingWindow()
        {
            InitializeComponent();

            AddHandler(Hyperlink.ClickEvent, (RoutedEventHandler)OnHyperlinkClicked);
        }

        void OnHyperlinkClicked(object sender, RoutedEventArgs e)
        {
            var rHyperlink = e.OriginalSource as Hyperlink;
            if (rHyperlink == null)
                return;

            Process.Start(rHyperlink.NavigateUri.ToString());
        }
    }
}
