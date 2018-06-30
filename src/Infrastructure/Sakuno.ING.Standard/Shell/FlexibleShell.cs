using System.Linq;
using Sakuno.ING.Composition;
using Sakuno.ING.Localization;
using Sakuno.ING.Settings;

namespace Sakuno.ING.Shell
{
    public abstract class FlexibleShell<TElement>
        where TElement : class
    {
        private readonly ILocalizationService localization;
        public const string SettingCategoryName = "SettingCategory";

        protected FlexibleShell(ILocalizationService localization)
        {
            this.localization = localization;
        }

        protected CategorizedSettingViews[] CreateSettingViews()
            => Compositor.Default.ResolveWithMetadata<TElement>()
                .GroupBy(m => (SettingCategory)m.MetaData[SettingCategoryName])
                .OrderBy(g => g.Key)
                .Select(g => new CategorizedSettingViews
                (
                    localization.GetLocalized(SettingCategoryName, g.Key.ToString()) ?? g.Key.ToString(),
                    g.Select(m => (object)m.Value).ToArray()
                )).ToArray();
    }
}
