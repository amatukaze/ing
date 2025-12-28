namespace Sakuno.ING;

public interface IServiceInitializable
{
    Task InitializeAsync(CancellationToken cancellationToken);
}
