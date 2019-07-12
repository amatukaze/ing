using Sakuno.ING.Settings;
using Sakuno.ING.Shell;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.UWP
{
    public sealed partial class SettingsView : Page
    {
        private readonly CategorizedSettingViews[] Pages;
        private readonly LayoutSetting LayoutSetting;

        public SettingsView(CategorizedSettingViews[] pages, LayoutSetting layoutSetting)
        {
            LayoutSetting = layoutSetting;
            InitializeComponent();
            Pages = pages;
        }
    }
}
