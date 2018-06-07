using Sakuno.ING.Composition;

namespace Sakuno.ING.Shell
{
    internal class Module : IExposableModule
    {
        public void Expose(IBuilder builder)
        {
            builder.RegisterType<LayoutSetting>();
        }
        public void Initialize(IResolver resolver) { }
    }
}
