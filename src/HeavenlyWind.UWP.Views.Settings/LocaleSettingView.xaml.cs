using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sakuno.KanColle.Amatsukaze.Composition;
using Sakuno.KanColle.Amatsukaze.Settings;
using Windows.UI.Xaml.Controls;

namespace Sakuno.KanColle.Amatsukaze.UWP.Views.Settings
{
    public sealed partial class LocaleSettingView : UserControl
    {
        private LocaleSetting Instance = StaticResolver.Instance.Resolve<LocaleSetting>();
        private KeyValuePair<string, string>[] Languages = Windows.Globalization.ApplicationLanguages.ManifestLanguages
            .Select(x => new KeyValuePair<string, string>(x, new CultureInfo(x).NativeName))
            .Prepend(new KeyValuePair<string, string>(string.Empty, "(default)"))
            .ToArray();

        public LocaleSettingView()
        {
            this.InitializeComponent();
        }
    }
}
