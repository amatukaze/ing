using Sakuno.ING.Composition;
using Sakuno.ING.Localization;
using Sakuno.ING.Shell;

namespace Sakuno.ING.UWP
{
    public class Module : IExposableModule
    {
        public void Expose(IBuilder builder)
        {
            builder.RegisterService<UWPShell, IShell>();
            builder.RegisterService<LocalizationService, ILocalizationService>();
        }
    }
}
