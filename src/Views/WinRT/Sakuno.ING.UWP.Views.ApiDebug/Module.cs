using Sakuno.ING.Composition;
using Sakuno.ING.Shell;

namespace Sakuno.ING.UWP.Views.ApiDebug
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
