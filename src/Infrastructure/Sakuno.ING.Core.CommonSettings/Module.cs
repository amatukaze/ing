using Sakuno.ING.Composition;

namespace Sakuno.ING.Settings
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
