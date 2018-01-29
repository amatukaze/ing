namespace Sakuno.KanColle.Amatsukaze.Composition
{
    public interface IBuilder
    {
        void RegisterService<TImpl, TService>() where TImpl : class;
    }
}
