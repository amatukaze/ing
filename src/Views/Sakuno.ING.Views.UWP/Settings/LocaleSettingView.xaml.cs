using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sakuno.ING.Settings;
using Sakuno.ING.Shell;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Settings
{
    [ExportSettingView(SettingCategory.Appearance)]
    public sealed partial class LocaleSettingView : UserControl
    {
        private readonly LocaleSetting Instance;
        private readonly KeyValuePair<string, string>[] Languages = Windows.Globalization.ApplicationLanguages.ManifestLanguages
            .Select(x => new KeyValuePair<string, string>(x, new CultureInfo(x).NativeName))
            .Prepend(new KeyValuePair<string, string>(string.Empty, "(default)"))
            .ToArray();

        public LocaleSettingView(LocaleSetting instance)
        {
            Instance = instance;
            InitializeComponent();
        }
    }
}
