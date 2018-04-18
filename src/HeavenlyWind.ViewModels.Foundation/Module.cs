using Sakuno.KanColle.Amatsukaze.Composition;

namespace Sakuno.KanColle.Amatsukaze.ViewModels
{
    internal class Module : IExposableModule
    {
        public void Expose(IBuilder builder)
        {
            builder.RegisterType<LocalizableTextStore>();
        }
        public void Initialize(IResolver resolver) { }
    }
}
