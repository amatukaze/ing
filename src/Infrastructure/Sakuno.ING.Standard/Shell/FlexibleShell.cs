using System;
using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Localization;
using Sakuno.ING.Settings;

namespace Sakuno.ING.Shell
{
    public abstract class FlexibleShell : IShell
    {
        private bool started;
        private readonly ILocalizationService localization;

        protected FlexibleShell(ILocalizationService localization)
        {
            this.localization = localization;
        }

        public virtual void Run()
        {
            started = true;
        }

        private readonly Dictionary<string, Type> views
            = new Dictionary<string, Type>();
        private readonly List<(Type ViewType, SettingCategory Category)> settingViews
            = new List<(Type, SettingCategory)>();

        public void RegisterView(Type viewType, string id, bool isFixedSize = true, bool singleWindowRecommended = false)
        {
            if (started) throw new InvalidOperationException("Shell already started.");
            views.Add(id, viewType);
        }

        public void RegisterSettingView(Type viewType, SettingCategory category = SettingCategory.Misc)
        {
            if (started) throw new InvalidOperationException("Shell already started.");
            settingViews.Add((viewType, category));
        }

        protected string GetString(string id)
            => localization.GetLocalized("ViewTitle", id) ?? id;

        protected IReadOnlyDictionary<string, Type> Views => views;

        protected CategorizedSettingViews[] CreateSettingViews()
            => settingViews
                .GroupBy(e => e.Category)
                .OrderBy(e => e.Key)
                .Select(e => new CategorizedSettingViews
                (
                    localization.GetLocalized("SettingCategory", e.Key.ToString()) ?? e.Key.ToString(),
                    e.Select(t => Activator.CreateInstance(t.ViewType)).ToArray()
                )).ToArray();
    }
}
