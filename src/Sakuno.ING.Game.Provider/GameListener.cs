using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

            MasterDataUpdated = RegisterRaw<MasterDataJson>("api_start2")
                .Select(x => x.SelectResponse(ParseMasterData));

            var requireInfo = RegisterRaw<GameStartupInfoJson>("api_get_member/require_info");
            AllEquipmentUpdated = requireInfo.Select(x => x.SelectResponse(r => r.api_slot_item))
                .CombineWith<ITimedMessage<IReadOnlyCollection<EquipmentJson>>>(RegisterRaw<EquipmentJson[]>("api_get_member/slot_item"));
            UseItemUpdated = requireInfo.Select(x => x.SelectResponse(r => r.api_useitem))
                .CombineWith<ITimedMessage<IReadOnlyCollection<UseItemCountJson>>>(RegisterRaw<UseItemCountJson[]>("api_get_member/useitem"));
            FreeEquipmentUpdated = requireInfo.Select(x => x.SelectResponse(r => r.api_unsetslot))
                .CombineWith(RegisterRaw<Dictionary<string, int[]>>("api_get_member/unsetslot"));

            var homeport = RegisterRaw<HomeportJson>("api_port/port");
            AdmiralUpdated = homeport.Select(x => x.SelectResponse(r => r.api_basic))
                .CombineWith<ITimedMessage<IRawAdmiral>>(RegisterRaw<AdmiralRecordJson>("api_get_member/record"));
            RepairingDockUpdated = homeport.Select(x => x.SelectResponse(r => r.api_ndock))
                .CombineWith<ITimedMessage<IReadOnlyCollection<RepairingDockJson>>>(RegisterRaw<RepairingDockJson[]>("api_get_member/ndock"));
            HomeportReturned = homeport;
            CompositionChanged = RegisterRaw("api_req_hensei/change")
                .Select(x => x.SelectRequest(ParseCompositionChange));
            FleetPresetSelected = RegisterRaw<FleetJson>("api_req_hensei/preset_select");
            ShipExtraSlotOpened = RegisterRaw("api_req_kaisou/open_exslot")
                .Select(x => x.SelectRequest(ParseShipExtraSlotOpen));
            ShipEquipmentUdated = RegisterRaw<ShipEquipmentJson>("api_req_kaisou/slot_exchange_index")
                .Select(x => x.SelectRequestAndResponse(ParseShipEquipmentUpdate));

            var ship3 = RegisterRaw<Ship3Json>("api_get_member/ship3")
                .CombineWith(RegisterRaw<Ship3Json>("api_get_member/ship_deck"));
            PartialFleetsUpdated = ship3.Select(x => x.SelectResponse(r => r.api_deck_data));
            PartialShipsUpdated = ship3.Select(x => x.SelectResponse(r => r.api_ship_data))
                .CombineWith(RegisterRaw<ShipJson[]>("api_get_member/ship2"),
                RegisterRaw<DepriveJson>("api_req_kaisou/slot_deprive").Select(x => x.SelectResponse(ParseShipDeprive)));

            RepairStarted = RegisterRaw("api_req_nyukyo/start")
                .Select(x => x.SelectRequest(ParseRepairStart));
            InstantRepaired = RegisterRaw("api_req_nyukyo/speedchange")
                .Select(x => x.SelectRequest(ParseInstantRepair));
            ShipCreated = RegisterRaw("api_req_kousyou/createship")
                .Select(x => x.SelectRequest(ParseShipCreation));
            InstantBuilt = RegisterRaw("api_req_kousyou/createship_speedchange")
                .Select(x => x.SelectRequest(ParseInstantBuilt));

            var charge = RegisterRaw<ShipsSupplyJson>("api_req_hokyu/charge");
            ShipSupplied = charge.Select(x => x.SelectResponse(r => r.api_ship));

            var getShip = RegisterRaw<ShipBuildCompletionJson>("api_req_kousyou/getship");
            BuildingDockUpdated = requireInfo.Select(x => x.SelectResponse(r => r.api_kdock))
                .CombineWith<ITimedMessage<IReadOnlyCollection<BuildingDockJson>>>
                    (RegisterRaw<BuildingDockJson[]>("api_get_member/kdock"),
                    getShip.Select(x => x.SelectResponse(r => r.api_kdock)));
            ShipBuildCompleted = getShip;

            var createItem = RegisterRaw<EquipmentCreationJson>("api_req_kousyou/createitem");
            EquipmentCreated = createItem.Select(x => x.SelectRequestAndResponse(ParseEquipmentCreation));

            var destroyShip = RegisterRaw<ShipDismantleJson>("api_req_kousyou/destroyship");
            ShipDismantled = destroyShip.Select(x => x.SelectRequest(ParseShipDismantling));

            var destroyItem = RegisterRaw<EquipmentDismantleJson>("api_req_kousyou/destroyitem2");
            EquipmentDismantled = destroyItem.Select(x => x.SelectRequest(ParseEquipmentDimantling));
            EquipmentImproved = RegisterRaw<EquipmentImproveJson>("api_req_kousyou/remodel_slot")
                .Select(x => x.SelectRequestAndResponse(ParseEquipmentImprove));

            var powerup = RegisterRaw<ShipPowerupJson>("api_req_kaisou/powerup");
            FleetsUpdated = homeport.Select(x => x.SelectResponse(r => r.api_deck_port))
                .CombineWith(powerup.Select(x => x.SelectResponse(r => r.api_deck)),
                    RegisterRaw<FleetJson[]>("api_get_member/deck"));
            ShipPoweruped = powerup.Select(x => x.SelectRequestAndResponse(ParseShipPowerup));

            QuestUpdated = RegisterRaw<QuestPageJson>("api_get_member/questlist")
                .Select(x => x.SelectRequestAndResponse(ParseQuestPage));
            QuestCompleted = RegisterRaw("api_req_quest/clearitemget")
                .Select(x => x.SelectRequest(ParseQuestComplete));

            var mapinfo = RegisterRaw<MapsJson>("api_get_member/mapinfo");
            MapsUpdated = mapinfo.Select(x => x.SelectResponse(r => r.api_map_info));
            AirForceUpdated = mapinfo.Select(x => x.SelectResponse(r => r.api_air_base));

            var setPlane = RegisterRaw<AirForceSetPlaneJson>("api_req_air_corps/set_plane");
            AirForcePlaneSet = setPlane.Select(x => x.SelectRequestAndResponse(ParseAirForcePlaneSet));

            AirForceActionSet = RegisterRaw("api_req_air_corps/set_action")
                .Select(x => x.SelectRequest(ParseAirForceActionSet));
            AirForceExpanded = RegisterRaw<AirForceJson>("api_req_air_corps/expand_base");

            var airSupply = RegisterRaw<AirForceSupplyJson>("api_req_air_corps/supply");
            AirForceSupplied = airSupply.Select(x => x.SelectRequestAndResponse(ParseAirForceSupply));

            MaterialsUpdated = homeport.Select(x => x.SelectResponse(r => r.api_material))
                .CombineWith<ITimedMessage<IMaterialsUpdate>>
                    (RegisterRaw<MaterialJsonArray>("api_get_member/material"),
                    charge, createItem, destroyShip, destroyItem, airSupply,
                    setPlane.Where(x => x.Response.api_after_bauxite.HasValue));
        }

        private void JsonError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            //Debug usage
        }

        private T Convert<T>(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            return jSerializer.Deserialize<T>(new JsonTextReader(new StreamReader(stream)));
        }

        public IProducer<ParsedMessage<T>> RegisterRaw<T>(string api) => provider
            .Where(arg => arg.Key.Equals(api, StringComparison.Ordinal))
            .Select(arg => new ParsedMessage<T>
            (
                arg.Key,
                arg.TimeStamp,
                HttpUtility.ParseQueryString(arg.Request),
                Convert<SvData<T>>(arg.Stream)
            ))
            .Where(m => m.IsSuccess);

        public IProducer<ParsedMessage> RegisterRaw(string api) => provider
            .Where(arg => arg.Key.Equals(api, StringComparison.Ordinal))
            .Select(arg => new ParsedMessage
            (
                arg.Key,
                arg.TimeStamp,
                HttpUtility.ParseQueryString(arg.Request),
                Convert<SvData>(arg.Stream)
            ))
            .Where(m => m.IsSuccess);

        public IProducer<SvData> RegisterFail() => provider
            .Select(arg => Convert<SvData>(arg.Stream))
            .Where(v => v.api_result != 1);

        public IProducer<ParsedMessage<JToken>> RegisterAny() => provider
            .Select(arg => new ParsedMessage<JToken>
            (
                arg.Key,
                arg.TimeStamp,
                HttpUtility.ParseQueryString(arg.Request),
                Convert<SvData<JToken>>(arg.Stream)
            ));
    }
}
