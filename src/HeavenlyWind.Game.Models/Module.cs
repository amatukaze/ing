using Sakuno.KanColle.Amatsukaze.Composition;
using Sakuno.KanColle.Amatsukaze.ViewModels;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    internal class Module : IExposableModule
    {
        internal static LocalizableTextStore Localize;
        public void Expose(IBuilder builder)
        {
            builder.RegisterType<NavalBase>();
        }
        public void Initialize(IResolver resolver)
        {
            Localize = resolver.Resolve<LocalizableTextStore>();
        }
    }
}
