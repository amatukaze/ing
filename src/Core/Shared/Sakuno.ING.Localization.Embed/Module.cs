using Sakuno.ING.Composition;

namespace Sakuno.ING.Localization.Embed
{
    internal class Module : IExposableModule
    {
        public void Expose(IBuilder builder)
        {
            builder.RegisterService<LocalizationService, ILocalizationService>();
        }
    }
}
