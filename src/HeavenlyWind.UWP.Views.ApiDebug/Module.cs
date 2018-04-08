using Sakuno.KanColle.Amatsukaze.Composition;
using Sakuno.KanColle.Amatsukaze.Shell;

namespace Sakuno.KanColle.Amatsukaze.UWP.Views.ApiDebug
{
    internal class Module : IModule
    {
        public void Initialize(IResolver resolver)
        {
            var shell = resolver.Resolve<IShell>();
            shell.RegisterView(typeof(ApiDebugView), "ApiDebug", false, true);
        }
    }
}
