using DryIoc;
using Sakuno.ING.Composition;
using System;
using System.Collections.Generic;

namespace Sakuno.ING.Bootstrap
{
    internal sealed class DryIocCompositor : Compositor
    {
        private readonly IContainer _container;
        private readonly IDictionary<string, Type> _registeredViews;

        public DryIocCompositor(Container container, IDictionary<string, Type> registeredViews)
        {
            _container = container;
            _registeredViews = registeredViews;
        }

        public override T Resolve<T>() => _container.Resolve<T>();
        public override object Resolve(Type type) => _container.Resolve(type);
        public override object? ResolveViewOrDefault(string? viewId) => viewId is not null && _registeredViews.TryGetValue(viewId, out var type) ? Resolve(type) : null;
    }
}
