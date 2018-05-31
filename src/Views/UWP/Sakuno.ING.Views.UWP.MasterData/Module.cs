using Sakuno.ING.Composition;
using Sakuno.ING.Shell;

namespace Sakuno.ING.Views.UWP.MasterData
{
    internal class Module : IModule
    {
        public void Initialize(IResolver resolver)
        {
            var shell = resolver.Resolve<IShell>();
            shell.RegisterView(typeof(MasterDataView), "MasterData", false, true);
        }
    }
}
