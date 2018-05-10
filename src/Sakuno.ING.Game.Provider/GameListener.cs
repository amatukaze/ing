using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Json.MasterData;
using Sakuno.ING.Game.Json.Shipyard;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Messaging;
using Sakuno.ING.Services;

namespace Sakuno.ING.Game
{
    public sealed partial class GameListener
    {
        private ITextStreamProvider provider;
        private JsonSerializer jSerializer = new JsonSerializer
        {
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        public GameListener(ITextStreamProvider provider)
        {
            this.provider = provider;
            jSerializer.Error += JsonError;

            MasterDataUpdated = RegisterResponse<MasterDataJson>("api_start2")
                .Select(ParseMasterData);

            var requireInfo = RegisterResponse<GameStartupInfoJson>("api_get_member/require_info");
            AllEquipmentUpdated = requireInfo.Select(x => x.api_slot_item)
                .CombineWith(RegisterResponse<EquipmentJson[]>("api_get_member/slot_item"));
            UseItemUpdated = requireInfo.Select(x => x.api_useitem)
                .CombineWith(RegisterResponse<UseItemCountJson[]>("api_get_member/useitem"));
            FreeEquipmentUpdated = requireInfo.Select(x => x.api_unsetslot)
                .CombineWith(RegisterResponse<Dictionary<string, int[]>>("api_get_member/unsetslot"));

            var homeport = RegisterResponse<HomeportJson>("api_port/port");
            AdmiralUpdated = homeport.Select(x => x.api_basic)
                .CombineWith<IRawAdmiral>(RegisterResponse<AdmiralRecordJson>("api_get_member/record"));
            RepairingDockUpdated = homeport.Select(x => x.api_ndock)
                .CombineWith(RegisterResponse<RepairingDockJson[]>("api_get_member/ndock"));
            HomeportReturned = homeport;
            CompositionChanged = RegisterRequest("api_req_hensei/change")
                .Select(ParseCompositionChange);
            FleetPresetSelected = RegisterResponse<FleetJson>("api_req_hensei/preset_select");
            ShipExtraSlotOpened = RegisterRequest("api_req_kaisou/open_exslot")
                .Select(ParseShipExtraSlotOpen);
            ShipEquipmentUdated = RegisterRaw<ShipEquipmentJson>("api_req_kaisou/slot_exchange_index")
                .Select(x => ParseShipEquipmentUpdate(x.Request, x.Response));

            var ship3 = RegisterResponse<Ship3Json>("api_get_member/ship3")
                .CombineWith(RegisterResponse<Ship3Json>("api_get_member/ship_deck"));
            PartialFleetsUpdated = ship3.Select(x => x.api_deck_data);
            PartialShipsUpdated = ship3.Select(x => x.api_ship_data)
                .CombineWith(RegisterResponse<ShipJson[]>("api_get_member/ship2"),
                RegisterResponse<DepriveJson>("api_req_kaisou/slot_deprive").Select(ParseShipDeprive));

            RepairStarted = RegisterRequest("api_req_nyukyo/start")
                .Select(ParseRepairStart);
            InstantRepaired = RegisterRequest("api_req_nyukyo/speedchange")
                .Select(ParseInstantRepair);
            ShipCreated = RegisterRequest("api_req_kousyou/createship")
                .Select(ParseShipCreation);
            InstantBuilt = RegisterRequest("api_req_kousyou/createship_speedchange")
                .Select(ParseInstantBuilt);

            var charge = RegisterResponse<ShipsSupplyJson>("api_req_hokyu/charge");
            ShipSupplied = charge.Select(x => x.api_ship);

            var getShip = RegisterResponse<ShipBuildCompletionJson>("api_req_kousyou/getship");
            BuildingDockUpdated = requireInfo.Select(x => x.api_kdock)
                .CombineWith(getShip.Select(x => x.api_kdock),
                    RegisterResponse<BuildingDockJson[]>("api_get_member/kdock"));
            ShipBuildCompleted = getShip;

            var createItem = RegisterRaw<EquipmentCreationJson>("api_req_kousyou/createitem");
            EquipmentCreated = createItem.Select(x => ParseEquipmentCreation(x.Request, x.Response));

            var destroyShip = RegisterRaw<ShipDismantleJson>("api_req_kousyou/destroyship");
            ShipDismantled = destroyShip.Select(x => ParseShipDismantling(x.Request));

            var destroyItem = RegisterRaw<EquipmentDismantleJson>("api_req_kousyou/destroyitem2");
            EquipmentDismantled = destroyItem.Select(x => ParseEquipmentDimantling(x.Request));
            EquipmentImproved = RegisterRaw<EquipmentImproveJson>("api_req_kousyou/remodel_slot")
                .Select(x => ParseEquipmentImprove(x.Request, x.Response));

            var powerup = RegisterRaw<ShipPowerupJson>("api_req_kaisou/powerup");
            FleetsUpdated = homeport.Select(x => x.api_deck_port)
                .CombineWith(powerup.Select(x => x.Response.api_deck),
                    RegisterResponse<FleetJson[]>("api_get_member/deck"));
            ShipPoweruped = powerup.Select(x => ParseShipPowerup(x.Request, x.Response));

            QuestUpdated = RegisterResponse<QuestPageJson>("api_get_member/questlist")
                .Select(ParseQuestPage);
            QuestCompleted = RegisterRequest("api_req_quest/clearitemget")
                .Select(ParseQuestComplete);

            var mapinfo = RegisterResponse<MapsJson>("api_get_member/mapinfo");
            MapsUpdated = mapinfo.Select(x => x.api_map_info);
            AirForceUpdated = mapinfo.Select(x => x.api_air_base);

            var setPlane = RegisterRaw<AirForceSetPlaneJson>("api_req_air_corps/set_plane");
            AirForcePlaneSet = setPlane.Select(x => ParseAirForcePlaneSet(x.Request, x.Response));

            AirForceActionSet = RegisterRequest("api_req_air_corps/set_action")
                .Select(ParseAirForceActionSet);
            AirForceExpanded = RegisterResponse<AirForceJson>("api_req_air_corps/expand_base");

            var airSupply = RegisterRaw<AirForceSupplyJson>("api_req_air_corps/supply");
            AirForceSupplied = airSupply.Select(x => ParseAirForceSupply(x.Request, x.Response));

            MaterialsUpdated = homeport.Select(x => x.api_material)
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
            //Debug usage
        }
    }
}
