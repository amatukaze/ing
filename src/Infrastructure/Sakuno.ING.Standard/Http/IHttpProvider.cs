using Sakuno.ING.Messaging;

namespace Sakuno.ING.Http
{
    public interface IHttpProvider : ITimedMessageProvider<HttpMessage>
    {
        bool Enabled { get; set; }
    }
}
