using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Sortie;
using System.Collections.Specialized;
using System.Reactive;
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

        private readonly Subject<RawSortieEvent> _mapStepped = new();
        private readonly Subject<Unit> _mapPartUnlocked = new();

        [Api("api_req_map/start")]
        private void HandleSortieStarted(NameValueCollection request, RawSortieEvent response)
        {
            HandleSortieAdvanced(response);
        }
        [Api("api_req_map/next")]
        private void HandleSortieAdvanced(RawSortieEvent response)
        {
            _mapStepped.OnNext(response);

            if (response.api_m1)
                _mapPartUnlocked.OnNext(Unit.Default);
        }

        [Api("api_req_map/anchorage_repair")]
        private void HandleAnchorageRepairing(AnchorageRepairJson response)
        {
            _partialShipsUpdated.OnNext(response.api_ship_data);
        }
    }
}
