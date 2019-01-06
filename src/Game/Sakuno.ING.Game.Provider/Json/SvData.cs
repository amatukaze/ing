namespace Sakuno.ING.Game.Json
{
    internal class SvData
    {
        public int api_result;
        public string api_result_msg;
    }
    internal class SvData<T> : SvData
    {
        public T api_data;
    }
}
