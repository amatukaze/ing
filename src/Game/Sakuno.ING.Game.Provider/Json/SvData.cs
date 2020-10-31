namespace Sakuno.ING.Game.Json
{
#nullable disable
    internal class SvData
    {
        public int api_result { get; set; }
        public string api_result_msg { get; set; }
    }
    internal sealed class SvData<T> : SvData
    {
        public T api_data { get; set; }
    }
#nullable enable
}
