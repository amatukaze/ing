namespace Sakuno.ING.Game
{
    internal interface IUpdatable<TId, TRaw> : IBindable, IIdentifiable<TId>
        where TId : struct
    {
        void Update(TRaw raw);
    }
}
