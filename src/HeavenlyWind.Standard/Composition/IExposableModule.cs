namespace Sakuno.KanColle.Amatsukaze.Composition
{
    public interface IExposableModule : IModule
    {
        void Expose(IBuilder builder);
    }
}
