namespace Sakuno.ING.Game.Json
{
    public class SvData
    {
        public int api_result;
        public string api_result_msg;
    }
    public class SvData<T> : SvData
    {
        public T api_data;
    }
}
