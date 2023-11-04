namespace Sakuno.ING.Game;

public interface IModel<T, TId, TRaw> : IIdentifiable<TId>
    where T : IModel<T, TId, TRaw>
    where TId : struct
    where TRaw : IIdentifiable<TId>
{
    static abstract T Create(TRaw raw);

    void Update(TRaw raw);
}
