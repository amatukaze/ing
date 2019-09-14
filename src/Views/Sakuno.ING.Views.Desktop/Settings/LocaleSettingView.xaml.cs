using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using Sakuno.ING.Localization;
using Sakuno.ING.Settings;
using Sakuno.ING.Shell;

namespace Sakuno.ING.Views.Desktop.Settings
{
    [ExportSettingView(SettingCategory.Appearance)]
    public partial class LocaleSettingView : UserControl
    {
        public LocaleSettingView(ILocalizationService localization)
        {
            InitializeComponent();
            languages.ItemsSource = new[]
            {
                new { Name = "(default)", Key = string.Empty }
            }.Concat(localization
            .SupportedCultures
            .Select(x => new
            {
                Name = x.NativeName,
                Key = x.Name
            }));

            userFont.ItemsSource = contentFont.ItemsSource = new[]
            {
                new { Name = "(default)", Key = string.Empty }
            }.Concat(Fonts.SystemFontFamilies
            .Select(x => new
            {
                Name = GetLocalizedName(x.FamilyNames),
                Key = x.Source
            }));

            static string GetLocalizedName(LanguageSpecificStringDictionary names)
            {
                if (names.TryGetValue(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.Name), out string name))
                    return name;
                return names.First().Value;
            }
        }
    }
}
