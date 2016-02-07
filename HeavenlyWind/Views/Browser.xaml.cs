using Sakuno.KanColle.Amatsukaze.Services;
using Sakuno.KanColle.Amatsukaze.Services.Browser;
using System.ComponentModel;
using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze.Views
{
    /// <summary>
    /// Browser.xaml の相互作用ロジック
    /// </summary>
    public partial class Browser : UserControl
    {
        internal BrowserHost Host => BrowserControl.Content as BrowserHost;

        public Browser()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
                BrowserService.Instance.Initialize();

            InitializeComponent();
        }
    }
}
