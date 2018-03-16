namespace Sakuno.KanColle.Amatsukaze.Composition
{
    public interface IBuilder
    {
        void RegisterService<TImpl, TService>() where TImpl : class, TService;
        void RegisterType<T>() where T : class;
        void RegisterInstance<T>(T instance) where T : class;
    }
}
