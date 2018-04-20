using Sakuno.KanColle.Amatsukaze.Composition;

namespace Sakuno.KanColle.Amatsukaze.Settings
{
    class Module : IModule, IExposableModule
    {
        public void Expose(IBuilder builder)
        {
            builder.RegisterType<ProxySetting>();
            builder.RegisterType<LocaleSetting>();
        }

        public void Initialize(IResolver resolver) { }
    }
}
