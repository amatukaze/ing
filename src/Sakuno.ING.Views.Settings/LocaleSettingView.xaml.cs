using Sakuno.ING.Composition;
using Sakuno.ING.Services;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Sakuno.ING.Views.Settings
{
    public partial class LocaleSettingView : UserControl
    {
        public LocaleSettingView()
        {
            InitializeComponent();
            languages.ItemsSource = new[]
            {
                new { Name = "(default)", Key = string.Empty }
            }.Concat(StaticResolver.Instance.Resolve<ILocalizationService>()
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

            string GetLocalizedName(LanguageSpecificStringDictionary names)
            {
                if (names.TryGetValue(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.Name), out string name))
                    return name;
                return names.First().Value;
            }
        }
    }
}
