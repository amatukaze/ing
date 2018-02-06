namespace Sakuno.KanColle.Amatsukaze.Collections
{
    public interface IProjector<TSource, TDestination>
    {
        TDestination Project(TSource source);
    }
}
