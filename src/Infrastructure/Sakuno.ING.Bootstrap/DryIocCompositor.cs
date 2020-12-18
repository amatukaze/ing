using DryIoc;
using Sakuno.ING.Composition;
using System;

namespace Sakuno.ING.Bootstrap
{
    internal sealed class DryIocCompositor : Compositor
    {
        private readonly IContainer _container;

        public DryIocCompositor(Container container)
        {
            _container = container;
        }

        public override T Resolve<T>() => _container.Resolve<T>();
        public override object Resolve(Type type) => _container.Resolve(type);
    }
}
