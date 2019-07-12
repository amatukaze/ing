using Sakuno.ING.Settings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.UWP
{
    public sealed partial class SubView : Page
    {
        private readonly LayoutSetting LayoutSetting;

        public SubView(LayoutSetting layoutSetting, string title, object content)
        {
            LayoutSetting = layoutSetting;
            InitializeComponent();
            TitleBar.Text = title;
            actualContent.Child = content as FrameworkElement ?? new ContentPresenter { Content = content };
        }
    }
}
