using Sakuno.KanColle.Amatsukaze.Services;
using System.ComponentModel;
using System.Windows;

namespace Sakuno.KanColle.Amatsukaze.Views
{
    /// <summary>
    /// Browser.xaml の相互作用ロジック
    /// </summary>
    partial class Browser
    {
        public static Browser Instance { get; private set; }

        UIElement r_Bar;

        public Browser()
        {
            Instance = this;

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                BrowserService.Instance.Initialize();

                ((MainWindow)App.Current.MainWindow).SubscribeBrowserPreferenceChanged();
            }

            InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            r_Bar = Template.FindName("Bar", this) as UIElement;
        }

        public Size GetBarSize() => r_Bar.DesiredSize;
    }
}
