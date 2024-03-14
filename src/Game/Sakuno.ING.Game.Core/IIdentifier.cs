namespace Sakuno.ING.Game;

public interface IIdentifier<T, TRaw>
{
    static abstract T From(TRaw value);
}
