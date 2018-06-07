using Sakuno.ING.Composition;

namespace Sakuno.ING.Shell.Desktop
{
    class Module : IModule, IExposableModule
    {
        public void Expose(IBuilder builder)
        {
            builder.RegisterService<DesktopShell, IShell>();
        }

        public void Initialize(IResolver resolver)
        {
        }
    }
}
