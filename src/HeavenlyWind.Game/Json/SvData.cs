namespace Sakuno.KanColle.Amatsukaze.Game.Json
{
    class SvData
    {
        public int api_result;
        public string api_result_msg;
    }
    class SvData<T> : SvData
    {
        public T api_data;
    }
}
