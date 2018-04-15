namespace Sakuno.KanColle.Amatsukaze.Game
{
    public interface ITableProvider
    {
        ITable<T> GetTable<T>();
    }
}
