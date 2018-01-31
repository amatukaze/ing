using System.Collections.Generic;
using Sakuno.KanColle.Amatsukaze.Bootstrap;

namespace Sakuno.KanColle.Amatsukaze
{
    static class BootstrapperLoader
    {
        public static void Startup(IDictionary<string, object> args) => Bootstraper.Startup(args);
    }
}
