using Sakuno.KanColle.Amatsukaze.Composition;

namespace Sakuno.KanColle.Amatsukaze.Shell
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
