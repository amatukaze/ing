using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Models;
using System.Reactive.Subjects;

namespace Sakuno.ING.Game
{
    public partial class GameProvider
    {
        private readonly Subject<RawMap[]> _mapsUpdated = new();
        private readonly Subject<RawAirForceGroup[]> _airForceGroupsUpdated = new();

        [Api("api_get_member/mapinfo")]
        private void HandleMapInfoApi(MapInfoJson response)
        {
            _mapsUpdated.OnNext(response.api_map_info);
            _airForceGroupsUpdated.OnNext(response.api_air_base);
        }
    }
}
