namespace Sakuno.ING.Composition
{
    public interface IExposableModule : IModule
    {
        void Expose(IBuilder builder);
    }
}
