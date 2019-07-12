using Sakuno.ING.Settings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.UWP
{
    internal sealed partial class MainView : Page
    {
        private readonly LayoutSetting LayoutSetting;

        public MainView(LayoutSetting layoutSetting, object content)
        {
            LayoutSetting = layoutSetting;
            InitializeComponent();
            mainContent.Child = content as FrameworkElement ?? new ContentPresenter { Content = content };
            Window.Current.SetTitleBar(DraggableTitle);
        }
    }
}
