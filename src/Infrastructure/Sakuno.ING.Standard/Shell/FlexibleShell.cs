using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sakuno.ING.Composition;
using Sakuno.ING.Localization;
using Sakuno.ING.Settings;

namespace Sakuno.ING.Shell
{
    public abstract class FlexibleShell : IShell
    {
        private readonly ILocalizationService localization;
        private readonly Compositor viewCompositor;
        private readonly Compositor settingCompositor;

        protected FlexibleShell(ILocalizationService localization, Compositor compositor)
        {
            this.localization = localization;

            viewCompositor = new Compositor(AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.DefinedTypes
                .SelectMany(t => t.GetCustomAttributes<ExportViewAttribute>()
                .Select(attr => new Export
                {
                    Contract = attr.ViewId,
                    Implementation = t,
                    SingleInstance = false,
                    LazyCreate = true
                }))), compositor);

            settingViews = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.DefinedTypes
                .SelectMany(t => t.GetCustomAttributes<ExportSettingViewAttribute>()
                .Select(attr => (t.AsType(), attr.Category)))).ToList();
            settingCompositor = new Compositor(settingViews.Select(x => new Export
            {
                Contract = x.ViewType,
                Implementation = x.ViewType,
                SingleInstance = false,
                LazyCreate = true
            }), compositor);
        }

        public abstract void Run();

        private readonly List<(Type ViewType, SettingCategory Category)> settingViews;

        protected Func<string, object> Views => viewCompositor.Resolve;

        protected CategorizedSettingViews[] CreateSettingViews()
            => settingViews
                .GroupBy(e => e.Category)
                .OrderBy(e => e.Key)
                .Select(e => new CategorizedSettingViews
                (
                    localization.GetLocalized("SettingCategory", e.Key.ToString()) ?? e.Key.ToString(),
                    e.Select(t => settingCompositor.Resolve(t.ViewType)).ToArray()
                )).ToArray();
    }
}
