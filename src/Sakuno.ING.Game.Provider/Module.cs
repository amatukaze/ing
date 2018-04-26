using Sakuno.ING.Composition;

namespace Sakuno.ING.Game
{
    internal class Module : IExposableModule
    {
        public void Expose(IBuilder builder)
        {
            builder.RegisterType<GameListener>();
        }
        public void Initialize(IResolver resolver) { }
    }
}
