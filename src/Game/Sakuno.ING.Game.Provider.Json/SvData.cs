#nullable disable

namespace Sakuno.ING.Game.Provider.Json;

public class SvData
{
    public int api_result { get; set; }
    public string api_result_msg { get; set; }
}
public sealed class SvData<T> : SvData
{
    public T api_data { get; set; }
}
