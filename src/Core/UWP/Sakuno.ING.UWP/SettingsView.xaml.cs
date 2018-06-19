using Sakuno.ING.Localization;
using Sakuno.ING.Shell;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.UWP
{
    public sealed partial class SettingsView : Page
    {
        private readonly CategorizedSettingViews[] Pages;
        public SettingsView(CategorizedSettingViews[] pages, ILocalizationService localization)
        {
            this.InitializeComponent();
            Pages = pages;
        }
    }
}
