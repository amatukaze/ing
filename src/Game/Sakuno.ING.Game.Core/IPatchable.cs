namespace Sakuno.ING.Game;

public interface IPatchable<TId, in TPatch>
    where TId : struct
    where TPatch : IPatch<TId>
{
    void Patch(TPatch patch);
}
