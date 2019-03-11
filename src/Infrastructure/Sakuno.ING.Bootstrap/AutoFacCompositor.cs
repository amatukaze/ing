using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Features.Metadata;
using Sakuno.ING.Composition;

namespace Sakuno.ING.Bootstrap
{
    internal class AutoFacCompositor : Compositor
    {
        private readonly IContainer container;
        private readonly HashSet<string> viewIds;

        public AutoFacCompositor(IContainer container, HashSet<string> viewIds)
        {
            this.container = container;
            this.viewIds = viewIds;
        }

        public override T Resolve<T>() => container.ResolveOptional<T>();
        public override object Resolve(Type type) => container.ResolveOptional(type);
        public override T ResolveWithParameter<T, TParam>(TParam parameter) => container.ResolveOptional<T>(new AutoFacParameter<TParam>(parameter));
        public override T ResolveNamed<T>(string name) => container.ResolveOptionalNamed<T>(name);
        public override object ResolveNamed(Type type, string name)
            => container.TryResolveNamed(name, type, out object instance)
            ? instance : null;
        public override T ResolveNamedWithParameter<T, TParam>(string name, TParam parameter)
            => container.ResolveOptionalNamed<T>(name, new AutoFacParameter<TParam>(parameter));
        public override IEnumerable<WithMeta<T>> ResolveWithMetadata<T>()
            => container.Resolve<IEnumerable<Meta<T>>>()
                .Select(m => new WithMeta<T>(m.Value, m.Metadata));
        public override bool IsViewRegistered(string viewId) => viewIds.Contains(viewId);
    }
}
