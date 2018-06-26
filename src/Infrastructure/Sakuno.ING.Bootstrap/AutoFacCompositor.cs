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

        public AutoFacCompositor(IContainer container) => this.container = container;

        public override T Resolve<T>() => container.ResolveOptional<T>();
        public override object Resolve(Type type) => container.ResolveOptional(type);
        public override T ResolveNamed<T>(string name) => container.ResolveOptionalNamed<T>(name);
        public override object ResolveNamed(Type type, string name)
            => container.TryResolveNamed(name, type, out object instance)
            ? instance : null;
        public override IEnumerable<WithMeta<T>> ResolveWithMetadata<T>()
            => container.Resolve<IEnumerable<Meta<T>>>()
                .Select(m => new WithMeta<T>(m.Value, m.Metadata));
    }
}
