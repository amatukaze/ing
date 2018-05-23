using Sakuno.ING.Shell;
using Sakuno.ING.Composition;
using Sakuno.ING.Services;

namespace Sakuno.ING.UWP
{
    public class Module : IModule, IExposableModule
    {
        public void Expose(IBuilder builder)
        {
            builder.RegisterService<Shell, IShell>();
            builder.RegisterService<LocalizationService, ILocalizationService>();
        }
        public void Initialize(IResolver resolver) { }
    }
}
