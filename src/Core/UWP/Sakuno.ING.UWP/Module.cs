using Sakuno.ING.Composition;
using Sakuno.ING.Localization;
using Sakuno.ING.Shell;
using Sakuno.ING.Shell.Layout;

namespace Sakuno.ING.UWP
{
    public class Module : IModule, IExposableModule
    {
        public void Expose(IBuilder builder)
        {
            builder.RegisterService<UWPShell, IShell>();
            builder.RegisterService<LocalizationService, ILocalizationService>();
        }
        public void Initialize(IResolver resolver)
        {
            LocalizedTitleExtension.LocalizationService = resolver.Resolve<ILocalizationService>();
        }
    }
}
