using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json
{
#nullable disable
    internal sealed class MapInfoJson
    {
        public RawMap[] api_map_info { get; set; }
        public RawAirForceGroup[] api_air_base { get; set; }
    }
#nullable enable
}
