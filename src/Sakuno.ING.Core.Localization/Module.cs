using Sakuno.ING.Composition;
using Sakuno.ING.Services;

namespace Sakuno.ING.Localization
{
    internal class Module : IExposableModule
    {
        public void Expose(IBuilder builder)
        {
            builder.RegisterService<LocalizationService, ILocalizationService>();
        }
        public void Initialize(IResolver resolver) { }
    }
}
