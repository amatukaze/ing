using System.Collections.Specialized;

namespace Sakuno.ING.Game.Json
{
#nullable disable
    internal class SvData
    {
        public int api_result { get; set; }
        public string api_result_msg { get; set; }
    }
    internal sealed class SvDataRequestOnly : SvData
    {
        public string Api { get; set; }
        public NameValueCollection Request { get; set; }
    }
    internal sealed class SvData<T> : SvData
    {
        public T api_data { get; set; }
    }
    internal sealed class SvDataWithRequest<T> : SvData
    {
        public NameValueCollection Request { get; set; }
        public T api_data { get; set; }
    }
#nullable enable
}
