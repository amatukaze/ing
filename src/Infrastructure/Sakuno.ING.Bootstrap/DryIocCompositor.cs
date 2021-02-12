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

        public override T Resolve<T>(string? serviceKey = null) => _container.Resolve<T>(serviceKey);
        public override object Resolve(Type type, string? serviceKey = null) => _container.Resolve(type, serviceKey);
        public override T ResolveOrDefault<T>(string? serviceKey = null) => _container.Resolve<T>(serviceKey, IfUnresolved.ReturnDefaultIfNotRegistered);
        public override object ResolveOrDefault(Type type, string? serviceKey = null) => _container.Resolve(type, serviceKey, IfUnresolved.ReturnDefaultIfNotRegistered);

        public override object? ResolveViewOrDefault(string? viewId, string? serviceKey = null) =>
            viewId is not null && _registeredViews.TryGetValue(viewId, out var type) ? Resolve(type, serviceKey) : null;
    }
}
