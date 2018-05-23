using System;
using Autofac;
using Sakuno.ING.Composition;

namespace Sakuno.ING.Bootstrap
{
    class Resolver : IResolver
    {
        IContainer _container;

        public Resolver(IContainer container)
        {
            _container = container;
        }

        public T Resolve<T>() where T : class =>
            _container.Resolve<T>();
        public object Resolve(Type type) => _container.Resolve(type);
    }
}
