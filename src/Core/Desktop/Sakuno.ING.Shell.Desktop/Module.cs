using Sakuno.ING.Composition;

namespace Sakuno.ING.Shell.Desktop
{
    class Module : IExposableModule
    {
        public void Expose(IBuilder builder)
        {
            builder.RegisterService<DesktopShell, IShell>();
        }
    }
}
