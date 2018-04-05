using Sakuno.KanColle.Amatsukaze.Composition;
using Sakuno.KanColle.Amatsukaze.Services;
using Sakuno.KanColle.Amatsukaze.Settings;
using Sakuno.KanColle.Amatsukaze.Shell;

namespace Sakuno.KanColle.Amatsukaze.UWP.Bridge
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
