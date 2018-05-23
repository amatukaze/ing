using Sakuno.ING.Composition;
using Sakuno.ING.Services;
using Sakuno.ING.Settings;
using Sakuno.ING.Shell;

namespace Sakuno.ING.UWP.Bridge
{
    internal class Module : IExposableModule
    {
        public void Expose(IBuilder builder)
        {
            builder.RegisterService<Provider, ITextStreamProvider>();
        }
        public void Initialize(IResolver resolver)
        {
            var shell = resolver.Resolve<IShell>();
            shell.RegisterSettingView(typeof(BridgeInfo), SettingCategory.Network);
        }
    }
}
