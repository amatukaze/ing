using System.Collections.Specialized;
using Sakuno.ING.Game.Json;

namespace Sakuno.ING.Game
{
    public class ParsedMessage
    {
        public ParsedMessage(string name, NameValueCollection request, SvData response)
        {
            Name = name;
            Request = request;
            IsSuccess = response.api_result == 1;
        }

        public string Name { get; }
        public NameValueCollection Request { get; }
        public bool IsSuccess { get; }
    }

    public sealed class ParsedMessage<T> : ParsedMessage
    {
        public ParsedMessage(string name, NameValueCollection request, SvData<T> response)
            : base(name, request, response)
        {
            Response = response.api_data;
        }

        public T Response { get; }
    }
}
