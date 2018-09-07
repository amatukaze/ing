namespace Sakuno.ING.Http
{
    public interface IHttpProviderSelector
    {
        IHttpProvider Current { get; }
    }
}
