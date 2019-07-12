using Sakuno.ING.Http;
using Sakuno.ING.Settings;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.UWP
{
#if DEBUG
    [Shell.ExportSettingView(SettingCategory.Browser)]
#endif
    internal sealed partial class DebugSettingView : UserControl
    {
        private readonly ISettingItem<bool> DebugData;
        private readonly DebugHttpProvider DebugProvider;

        public DebugSettingView(UWPHttpProviderSelector selector)
        {
            DebugData = selector.Settings.Debug;
            DebugProvider = selector.Current as DebugHttpProvider;
            InitializeComponent();
        }
    }
}
