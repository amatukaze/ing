using System;

namespace Sakuno.ING.Http
{
    public sealed class HttpMessage
    {
        public HttpMessage(string key, ReadOnlyMemory<char> request, ReadOnlyMemory<char> response)
        {
            Key = key;
            Request = request;
            Response = response;
        }

        public string Key { get; }
        public ReadOnlyMemory<char> Request { get; }
        public ReadOnlyMemory<char> Response { get; }
    }
}
