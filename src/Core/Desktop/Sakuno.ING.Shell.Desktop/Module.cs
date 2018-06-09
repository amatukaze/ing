using Sakuno.ING.Composition;
using Sakuno.ING.Localization;
using Sakuno.ING.Shell.Layout;

namespace Sakuno.ING.Shell.Desktop
{
    class Module : IModule, IExposableModule
    {
        public void Expose(IBuilder builder)
        {
            builder.RegisterService<DesktopShell, IShell>();
        }

        public void Initialize(IResolver resolver)
        {
            LocalizedTitleExtension.LocalizationService = resolver.Resolve<ILocalizationService>();
        }
    }
}
