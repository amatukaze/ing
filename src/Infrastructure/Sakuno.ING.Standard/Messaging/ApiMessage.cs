using System;

namespace Sakuno.ING.Messaging
{
    public sealed class ApiMessage
    {
        public string Api { get; }
        public ReadOnlyMemory<char> Request { get; }
        public ReadOnlyMemory<byte> Response { get; }

        public ApiMessage(string api, ReadOnlyMemory<char> request, ReadOnlyMemory<byte> response)
        {
            Api = api;
            Request = request;
            Response = response;
        }
    }
}
