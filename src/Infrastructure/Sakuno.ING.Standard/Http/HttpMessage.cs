using System.IO;

namespace Sakuno.ING.Http
{
    public sealed class HttpMessage
    {
        public HttpMessage(string key, Stream request, Stream response)
        {
            Key = key;
            Request = request;
            Response = response;
        }

        public string Key { get; }
        public Stream Request { get; }
        public Stream Response { get; }
    }
}
