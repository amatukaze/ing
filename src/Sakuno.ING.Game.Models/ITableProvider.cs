namespace Sakuno.ING.Game
{
    public interface ITableProvider
    {
        ITable<T> GetTable<T>();
    }
}
