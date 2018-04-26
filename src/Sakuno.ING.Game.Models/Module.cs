using Sakuno.ING.Composition;
using Sakuno.ING.Services;

namespace Sakuno.ING.Game.Models
{
    internal class Module : IExposableModule
    {
        internal static ILocalizationService Localize;
        public void Expose(IBuilder builder)
        {
            builder.RegisterType<NavalBase>();
        }
        public void Initialize(IResolver resolver)
        {
            Localize = resolver.Resolve<ILocalizationService>();
        }
    }
}
