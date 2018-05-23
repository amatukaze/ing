using System;
using Autofac;
using Sakuno.ING.Composition;

namespace Sakuno.ING.Bootstrap
{
    class Builder : IBuilder
    {
        ContainerBuilder _builder;

        public Builder(ContainerBuilder builder)
        {
            _builder = builder;
        }

        public void Complete() => _builder = null;

        public void RegisterService<TImpl, TService>() where TImpl : class, TService
        {
            if (_builder == null)
                throw new InvalidOperationException("Cannot register because the composition has finished.");

            _builder.RegisterType<TImpl>().As<TService>().SingleInstance();
        }

        public void RegisterType<T>() where T : class
        {
            if (_builder == null)
                throw new InvalidOperationException("Cannot register because the composition has finished.");

            _builder.RegisterType<T>().AsSelf().SingleInstance();
        }

        public void RegisterInstance<T>(T instance) where T : class
        {
            if (_builder == null)
                throw new InvalidOperationException("Cannot register because the composition has finished.");

            _builder.RegisterInstance(instance).As<T>().SingleInstance();
        }
    }
}
