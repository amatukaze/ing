using System.Collections.Specialized;
using Sakuno.ING.Game.Json;

namespace Sakuno.ING.Game
{
    public class ParsedMessage
    {
        internal ParsedMessage(string name, NameValueCollection request, SvData response)
        {
            Name = name;
            Request = request;
            ResultCode = response.api_result;
            ResultMesssage = response.api_result_msg;
        }

        public string Name { get; }
        public NameValueCollection Request { get; }
        public int ResultCode { get; }
        public string ResultMesssage { get; }
        public bool IsSuccess => ResultCode == 1;
    }

    public sealed class ParsedMessage<T> : ParsedMessage
    {
        internal ParsedMessage(string name, NameValueCollection request, SvData<T> response)
            : base(name, request, response)
        {
            Response = response.api_data;
        }

        public T Response { get; }
    }
}
