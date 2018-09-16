using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Json.MasterData;
using Sakuno.ING.Game.Json.Shipyard;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Http;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game
{
    [Export(typeof(IGameProvider))]
    [Export(typeof(GameListener))]
    public sealed partial class GameListener : IGameProvider
    {
        private IHttpProvider provider;
        private JsonSerializer jSerializer = new JsonSerializer
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = new GameContractResolver()
        };

        public GameListener(IHttpProviderSelector providerSelector)
        {
            this.provider = providerSelector.Current;
            jSerializer.Error += JsonError;

            masterDataUpdated = RegisterResponse<MasterDataJson>("api_start2")
                .CombineWith(RegisterResponse<MasterDataJson>("api_start2/getData"))
                .Select(ParseMasterData);

            var requireInfo = RegisterResponse<GameStartupInfoJson>("api_get_member/require_info");
            allEquipmentUpdated = requireInfo.Select(x => x.api_slot_item)
                .CombineWith(RegisterResponse<EquipmentJson[]>("api_get_member/slot_item"));
            useItemUpdated = requireInfo.Select(x => x.api_useitem)
                .CombineWith(RegisterResponse<UseItemCountJson[]>("api_get_member/useitem"));
            freeEquipmentUpdated = requireInfo.Select(x => x.api_unsetslot)
                .CombineWith(RegisterResponse<Dictionary<string, int[]>>("api_get_member/unsetslot"));

            var homeport = RegisterResponse<HomeportJson>("api_port/port");
            admiralUpdated = homeport.Select(x => x.api_basic)
                .CombineWith<IRawAdmiral>(RegisterResponse<AdmiralRecordJson>("api_get_member/record"));
            repairingDockUpdated = homeport.Select(x => x.api_ndock)
                .CombineWith(RegisterResponse<RepairingDockJson[]>("api_get_member/ndock"));
            homeportReturned = homeport.Select(ParseHomeport);
            compositionChanged = RegisterRequest("api_req_hensei/change")
                .Select(ParseCompositionChange);
            fleetPresetSelected = RegisterResponse<FleetJson>("api_req_hensei/preset_select");
            shipExtraSlotOpened = RegisterRequest("api_req_kaisou/open_exslot")
                .Select(ParseShipExtraSlotOpen);
            shipEquipmentUpdated = RegisterRaw<ShipEquipmentJson>("api_req_kaisou/slot_exchange_index")
                .Select(x => ParseShipEquipmentUpdate(x.Request, x.Response));
            expeditionCompleted = RegisterRaw<ExpeditionCompletionJson>("api_req_mission/result")
                .Select(x => ParseExpeditionCompletion(x.Request, x.Response));

            var ship3 = RegisterResponse<Ship3Json>("api_get_member/ship3")
                .CombineWith(RegisterResponse<Ship3Json>("api_get_member/ship_deck"));
            partialFleetsUpdated = ship3.Select(x => x.api_deck_data);
            partialShipsUpdated = ship3.Select(x => x.api_ship_data)
                .CombineWith(RegisterResponse<ShipJson[]>("api_get_member/ship2"),
                RegisterResponse<DepriveJson>("api_req_kaisou/slot_deprive").Select(ParseShipDeprive));

            repairStarted = RegisterRequest("api_req_nyukyo/start")
                .Select(ParseRepairStart);
            instantRepaired = RegisterRequest("api_req_nyukyo/speedchange")
                .Select(ParseInstantRepair);
            shipCreated = RegisterRequest("api_req_kousyou/createship")
                .Select(ParseShipCreation);
            instantBuilt = RegisterRequest("api_req_kousyou/createship_speedchange")
                .Select(ParseInstantBuilt);

            var charge = RegisterResponse<ShipsSupplyJson>("api_req_hokyu/charge");
            shipSupplied = charge.Select(x => x.api_ship);

            var getShip = RegisterResponse<ShipBuildCompletionJson>("api_req_kousyou/getship");
            buildingDockUpdated = requireInfo.Select(x => x.api_kdock)
                .CombineWith(getShip.Select(x => x.api_kdock),
                    RegisterResponse<BuildingDockJson[]>("api_get_member/kdock"));
            shipBuildCompleted = getShip.Select(ParseShipBuildCompletion);

            var createItem = RegisterRaw<EquipmentCreationJson>("api_req_kousyou/createitem");
            equipmentCreated = createItem.Select(x => ParseEquipmentCreation(x.Request, x.Response));

            var destroyShip = RegisterRaw<ShipDismantleJson>("api_req_kousyou/destroyship");
            shipDismantled = destroyShip.Select(x => ParseShipDismantling(x.Request));

            var destroyItem = RegisterRaw<EquipmentDismantleJson>("api_req_kousyou/destroyitem2");
            equipmentDismantled = destroyItem.Select(x => ParseEquipmentDimantling(x.Request));
            equipmentImproved = RegisterRaw<EquipmentImproveJson>("api_req_kousyou/remodel_slot")
                .Select(x => ParseEquipmentImprove(x.Request, x.Response));

            var powerup = RegisterRaw<ShipPowerupJson>("api_req_kaisou/powerup");
            fleetsUpdated = homeport.Select(x => x.api_deck_port)
                .CombineWith(powerup.Select(x => x.Response.api_deck),
                    RegisterResponse<FleetJson[]>("api_get_member/deck"));
            shipPoweruped = powerup.Select(x => ParseShipPowerup(x.Request, x.Response));

            questUpdated = RegisterResponse<QuestPageJson>("api_get_member/questlist")
                .Select(ParseQuestPage);
            questCompleted = RegisterRequest("api_req_quest/clearitemget")
                .Select(ParseQuestComplete);

            var mapinfo = RegisterResponse<MapsJson>("api_get_member/mapinfo");
            mapsUpdated = mapinfo.Select(x => x.api_map_info);
            airForceUpdated = mapinfo.Select(x => x.api_air_base);

            var setPlane = RegisterRaw<AirForceSetPlaneJson>("api_req_air_corps/set_plane");
            airForcePlaneSet = setPlane.Select(x => ParseAirForcePlaneSet(x.Request, x.Response));

            airForceActionSet = RegisterRequest("api_req_air_corps/set_action")
                .Select(ParseAirForceActionSet);
            airForceExpanded = RegisterResponse<AirForceJson>("api_req_air_corps/expand_base");

            var airSupply = RegisterRaw<AirForceSupplyJson>("api_req_air_corps/supply");
            airForceSupplied = airSupply.Select(x => ParseAirForceSupply(x.Request, x.Response));

            materialsUpdated = homeport.Select(x => x.api_material)
                .CombineWith<IMaterialsUpdate>
                    (charge,
                    RegisterResponse<MaterialJsonArray>("api_get_member/material"),
                    createItem.Select(x => x.Response),
                    destroyShip.Select(x => x.Response),
                    destroyItem.Select(x => x.Response),
                    airSupply.Select(x => x.Response),
                    setPlane.Select(x => x.Response).Where(x => x.api_after_bauxite.HasValue));
        }

        private void JsonError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            SerialzationError?.Invoke(new SerializationError(e.ErrorContext.Error, e.ErrorContext.Path));
            e.ErrorContext.Handled = true;
        }

        public event Action<SerializationError> SerialzationError;
    }
}
