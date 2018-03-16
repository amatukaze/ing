using Sakuno.KanColle.Amatsukaze.Shell;
using Sakuno.KanColle.Amatsukaze.Composition;
using Sakuno.KanColle.Amatsukaze.Services;

namespace Sakuno.KanColle.Amatsukaze.UWP
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
