using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Json;
using System.Reactive.Subjects;

namespace Sakuno.ING.Game
{
    public partial class GameProvider
    {
        private readonly Subject<MasterDataUpdate> _masterDataUpdated = new();

        [Api("api_start2/getData")]
        private void HandleMasterData(MasterDataJson response) =>
            _masterDataUpdated.OnNext(new
            (
                shipInfos: response.api_mst_ship,
                shipTypes: response.api_mst_stype,
                slotItemInfos: response.api_mst_slotitem,
                slotItemTypes: response.api_mst_slotitem_equiptype,
                useItems: response.api_mst_useitem,
                mapAreas: response.api_mst_maparea,
                maps: response.api_mst_mapinfo,
                expeditions: response.api_mst_mission
            ));
    }
}
