using Sakuno.KanColle.Amatsukaze.Composition;
using Sakuno.KanColle.Amatsukaze.Services;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
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
