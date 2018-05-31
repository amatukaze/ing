using Sakuno.ING.Composition;

namespace Sakuno.ING.Shell.Desktop
{
    class Module : IModule, IExposableModule
    {
        public void Expose(IBuilder builder)
        {
            builder.RegisterService<Shell, IShell>();
        }

        public void Initialize(IResolver resolver)
        {
        }
    }
}
