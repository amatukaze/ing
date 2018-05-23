using System.IO;

namespace Sakuno.ING.Services
{
    public sealed class TextMessage
    {
        public TextMessage(string key, string request, Stream stream)
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
