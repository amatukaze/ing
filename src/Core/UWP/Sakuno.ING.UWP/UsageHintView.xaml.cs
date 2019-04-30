using Sakuno.ING.Settings;
using Sakuno.ING.Shell;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.UWP
{
    [ExportSettingView(SettingCategory.Misc)]
    public sealed partial class UsageHintView : UserControl
    {
        private readonly StorageFolder LocalFolder = ApplicationData.Current.LocalFolder;
        private readonly string LoopbackCommand = "CheckNetIsolation LoopbackExempt -a -n=" + Package.Current.Id.FamilyName;
        public UsageHintView()
        {
            this.InitializeComponent();
        }

        private void LaunchDataFolder()
        {
            _ = Launcher.LaunchFolderAsync(LocalFolder);
        }
    }
}
