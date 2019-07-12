using System.Collections.Generic;

namespace Sakuno.ING.Bootstrap
{
    internal interface IModuleList
    {
        IDictionary<string, ModuleInfo> Modules { get; }
    }
}
