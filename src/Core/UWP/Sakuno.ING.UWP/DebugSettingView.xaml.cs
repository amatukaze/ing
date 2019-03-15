using Sakuno.ING.Settings;
using Sakuno.ING.Shell;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.UWP
{
    [ExportSettingView(SettingCategory.Browser)]
    internal sealed partial class DebugSettingView : UserControl
    {
        private readonly ISettingItem<bool> DebugData;

        public DebugSettingView(UWPHttpProviderSelector selector)
        {
            DebugData = selector.Settings.Debug;
            this.InitializeComponent();
        }
    }
}
