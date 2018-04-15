using Sakuno.KanColle.Amatsukaze.Composition;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    internal class Module : IExposableModule
    {
        public void Expose(IBuilder builder)
        {
            builder.RegisterType<NavalBase>();
        }
        public void Initialize(IResolver resolver) { }
    }
}
