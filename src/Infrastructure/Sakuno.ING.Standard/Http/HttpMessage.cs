using System.IO;

namespace Sakuno.ING.Http
{
    public sealed class HttpMessage
    {
        public HttpMessage(string key, string request, Stream stream)
        {
            Key = key;
            Request = request;
            Stream = stream;
        }

        public string Key { get; }
        public string Request { get; }
        public Stream Stream { get; }
    }
}
