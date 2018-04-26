using System.Collections.Generic;

namespace Sakuno.ING.Bootstrap
{
    interface IModuleList
    {
        IDictionary<string, ModuleInfo> Modules { get; }
    }
}
