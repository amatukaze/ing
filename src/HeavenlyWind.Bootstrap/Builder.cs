using Autofac;
using Sakuno.KanColle.Amatsukaze.Composition;
using System;

namespace Sakuno.KanColle.Amatsukaze.Bootstrap
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
    }
}
