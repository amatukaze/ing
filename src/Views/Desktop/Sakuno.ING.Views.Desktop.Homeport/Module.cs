using Sakuno.ING.Composition;
using Sakuno.ING.Shell;

namespace Sakuno.ING.Views.Desktop.Homeport
{
    internal class Module : IModule
    {
        public void Initialize(IResolver resolver)
        {
            var shell = resolver.Resolve<IShell>();
            shell.RegisterView(typeof(FleetsView), "Fleets", false);
        }
    }
}
