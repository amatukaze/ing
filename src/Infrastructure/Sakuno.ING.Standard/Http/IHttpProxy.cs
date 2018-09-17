namespace Sakuno.ING.Http
{
    public interface IHttpProxy : IHttpProvider
    {
        bool IsEnabled { get; set; }
        int ListeningPort { get; }
    }
}
