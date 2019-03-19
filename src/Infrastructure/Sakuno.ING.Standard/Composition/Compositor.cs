using System;
using System.Collections.Generic;
using Sakuno.ING.Settings;

namespace Sakuno.ING.Composition
{
    public abstract class Compositor
    {
        protected Compositor()
        {
            if (Default == null)
                Default = this;
        }

        public abstract T Resolve<T>() where T : class;
        public abstract object Resolve(Type type);
        public abstract object ResolveWithParameter<TParam>(Type type, TParam parameter);

        public abstract IReadOnlyDictionary<string, Type> ViewTypes { get; }
        public abstract object TryResolveView(string viewId);

        public abstract IReadOnlyCollection<KeyValuePair<Type, SettingCategory>> SettingViews { get; }
        public abstract IEnumerable<Type> GetSettingViewsForCategory(SettingCategory category);

        public static Compositor Default { get; protected set; }
        public static T Static<T>() where T : class => Default?.Resolve<T>();
    }
}
