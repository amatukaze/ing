using System.Linq;
using Sakuno.ING.Composition;
using Sakuno.ING.Localization;

namespace Sakuno.ING.Shell
{
    public abstract class FlexibleShell
    {
        private readonly ILocalizationService localization;
        public const string SettingCategoryName = "SettingCategory";

        protected FlexibleShell(ILocalizationService localization)
        {
            this.localization = localization;
        }

        protected CategorizedSettingViews[] CreateSettingViews()
            => Compositor.Default.SettingViews
                .GroupBy(m => m.Value)
                .OrderBy(g => g.Key)
                .Select(g => new CategorizedSettingViews
                (
                    localization.GetLocalized(SettingCategoryName, g.Key.ToString()) ?? g.Key.ToString(),
                    g.Select(m => Compositor.Default.Resolve(m.Key)).ToArray()
                )).ToArray();
    }
}
