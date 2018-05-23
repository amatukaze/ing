using Sakuno.ING.Composition;

namespace Sakuno.ING.Game
{
    internal class Module : IExposableModule
    {
        public void Expose(IBuilder builder)
        {
            builder.RegisterType<GameListener>();
            builder.RegisterService<GameListener, IGameProvider>();
        }
        public void Initialize(IResolver resolver) { }
    }
}
