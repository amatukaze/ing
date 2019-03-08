using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Sakuno.ING.Composition;

namespace Sakuno.ING
{
    internal class AutofacCompositor : Compositor
    {
        private readonly IContainer container;

        public AutofacCompositor(IContainer container) => this.container = container;

        public override T Resolve<T>() => container.ResolveOptional<T>();
        public override object Resolve(Type type) => container.ResolveOptional(type);
    }
}
