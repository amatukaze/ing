using System.Globalization;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    class UIFontProperty : Property<string>
    {
        public override string Value
        {
            get { return !base.Value.IsNullOrEmpty() ? base.Value : GetDefaultFont(); }

            set { base.Value = value; }
        }

        public UIFontProperty() : base("ui.font", GetDefaultFont()) { }

        static string GetDefaultFont()
        {
            var rCultures = StringResources.GetAncestorsAndSelfCultureNames(CultureInfo.CurrentCulture).ToArray();
            if (rCultures.Any(r => r.OICEquals("zh-Hans")))
                return "Microsoft YaHei UI, Segoe UI";
            else if (rCultures.Any(r => r.OICEquals("zh-Hant")))
                return "Microsoft JhengHei UI, Segoe UI";
            else
                return "Meiryo UI, Segoe UI";
        }
    }
}
