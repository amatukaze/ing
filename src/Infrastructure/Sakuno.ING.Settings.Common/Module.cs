using Sakuno.ING.Composition;

namespace Sakuno.ING.Settings
{
    class Module : IModule, IExposableModule
    {
        public void Expose(IBuilder builder)
        {
            builder.RegisterType<ProxySetting>();
            builder.RegisterType<LocaleSetting>();
            builder.RegisterType<LayoutSetting>();
        }

        public void Initialize(IResolver resolver) { }
    }
}
