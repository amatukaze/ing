using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Bootstrap
{
    interface IModuleList
    {
        IDictionary<string, ModuleInfo> Modules { get; }
    }
}
