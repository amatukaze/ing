using Sakuno.ING.Composition;

namespace Sakuno.ING.Services.Listener
{
    internal class Module : IExposableModule
    {
        public void Expose(IBuilder builder)
        {
            builder.RegisterType<Nekomimi.ProxyServer>();
            builder.RegisterService<NekomimiProvider, ITextStreamProvider>();
        }

        public void Initialize(IResolver resolver) { }
    }
}
