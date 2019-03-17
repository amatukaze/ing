using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Sakuno.ING.Composition;
using Sakuno.ING.Settings;

namespace Sakuno.ING.Bootstrap
{
    internal class AutoFacCompositor : Compositor
    {
        private readonly IContainer container;
        private readonly Dictionary<string, Type> views;
        private readonly List<KeyValuePair<Type, SettingCategory>> settingViews;

        public AutoFacCompositor(IContainer container, Dictionary<string, Type> views, List<KeyValuePair<Type, SettingCategory>> settingViews)
        {
            this.container = container;
            this.views = views;
            this.settingViews = settingViews;
        }

        public override T Resolve<T>() => container.ResolveOptional<T>();
        public override object Resolve(Type type) => container.ResolveOptional(type);
        public override object ResolveWithParameter<TParam>(Type type, TParam parameter) => container.ResolveOptional(type, new AutoFacParameter<TParam>(parameter));

        public override IReadOnlyDictionary<string, Type> ViewTypes => views;
        public override object TryResolveView(string viewId)
            => views.TryGetValue(viewId, out var viewType)
            ? Resolve(viewType) : null;

        public override IReadOnlyList<KeyValuePair<Type, SettingCategory>> SettingViews => settingViews;
        public override IEnumerable<Type> GetSettingViewsForCategory(SettingCategory category)
            => settingViews.Where(x => x.Value == category).Select(x => x.Key);
    }
}
