using Sakuno.ING.Extensibility.Composition;
using Sakuno.ING.Extensibility.Services;
using System;

namespace Sakuno.ING.Core.Services
{
    [Export(typeof(IProxyService))]
    class ProxyService : IProxyService
    {
        public void Start()
        {
        }
    }
}
