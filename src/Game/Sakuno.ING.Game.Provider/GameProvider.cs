using Sakuno.ING.Composition;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Json.Converters;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Quests;
using Sakuno.ING.Messaging;
using System;
using System.Collections.Specialized;
using System.Reactive.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace Sakuno.ING.Game
{
    [Export]
    public sealed partial class GameProvider
    {
        private readonly JsonSerializerOptions _serializerOptions;

        public GameProvider(IApiMessageSource apiMessageSource)
        {
            _serializerOptions = new JsonSerializerOptions()
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
            };
            _serializerOptions.Converters.Add(new IdentifierConverterFactory());
            _serializerOptions.Converters.Add(new MaterialsConverter());
            _serializerOptions.Converters.Add(new ShipModernizationConverter());
            _serializerOptions.Converters.Add(new ExpeditionUseItemRewardConverter());
            _serializerOptions.Converters.Add(new TimestampInMillisecondConverter());
            _serializerOptions.Converters.Add(new UnequippedSlotItemInfoConverter());

            var deserialized = apiMessageSource.ApiMessageSource.Select(message => message.Api switch
            {
                "api_start2/getData" => Deserialize<MasterDataJson>(message),
                "api_get_member/require_info" => Deserialize<StartupInfoJson>(message),
                "api_port/port" => Deserialize<HomeportJson>(message),
                "api_get_member/basic" => Deserialize<BasicAdmiral>(message),
                "api_get_member/record" => Deserialize<RecordAdmiral>(message),
                "api_get_member/slot_item" => Deserialize<RawSlotItem[]>(message),
                "api_get_member/ndock" => Deserialize<RawRepairDock[]>(message),
                "api_get_member/material" => Deserialize<RawMaterialItem[]>(message),
                "api_get_member/deck" => Deserialize<RawFleet[]>(message),
                "api_get_member/useitem" => Deserialize<RawUseItemCount[]>(message),
                "api_get_member/unsetslot" => Deserialize<RawUnequippedSlotItemInfo[]>(message),
                "api_get_member/kdock" => Deserialize<RawConstructionDock[]>(message),
                "api_get_member/mapinfo" => Deserialize<MapInfoJson>(message),
                "api_get_member/ship2" => Deserialize<RawShip[]>(message),
                "api_get_member/ship3" => Deserialize<Ship3Json>(message),
                "api_get_member/ship_deck" => Deserialize<ShipDeckJson>(message),
                "api_req_hokyu/charge" => Deserialize<ShipsSupplyJson>(message),
                "api_req_kaisou/marriage" => Deserialize<RawShip>(message),
                "api_req_air_corps/set_plane" => DeserializeWithRequest<AirForceSquadronDeploymentJson>(message),
                "api_get_member/questlist" => Deserialize<QuestListJson>(message),

                _ => (SvData)DeserializeRequestOnly(message),
            }).Publish();

            MasterDataUpdated = deserialized.Parse<MasterDataJson, MasterDataUpdate>(raw => new MasterDataUpdate
            (
                shipInfos: raw.api_mst_ship,
                shipTypes: raw.api_mst_stype,
                slotItemInfos: raw.api_mst_slotitem,
                slotItemTypes: raw.api_mst_slotitem_equiptype,
                useItems: raw.api_mst_useitem,
                mapAreas: raw.api_mst_maparea,
                maps: raw.api_mst_mapinfo,
                expeditions: raw.api_mst_mission
            ));

            AdmiralUpdated = Observable.Merge(new[]
            {
                deserialized.Parse<HomeportJson, RawAdmiral>(raw => raw.api_basic),
                deserialized.OfData<BasicAdmiral>(),
                deserialized.OfData<RecordAdmiral>(),
            });

            ShipsUpdate = deserialized.Parse<HomeportJson, RawShip[]>(raw => raw.api_ship);

            FleetsUpdate = Observable.Merge(new[]
            {
                deserialized.Parse<HomeportJson, RawFleet[]>(raw => raw.api_deck_port),
                deserialized.OfData<RawFleet[]>(),
            });
            SlotItemsUpdated = Observable.Merge(new[]
            {
                deserialized.Parse<StartupInfoJson, RawSlotItem[]>(raw => raw.api_slot_item),
                deserialized.OfData<RawSlotItem[]>(),
            });
            RepairDocksUpdate = Observable.Merge(new[]
            {
                deserialized.Parse<HomeportJson, RawRepairDock[]>(raw => raw.api_ndock),
                deserialized.OfData<RawRepairDock[]>(),
            });
            UseItemsUpdated = Observable.Merge(new[]
            {
                deserialized.Parse<StartupInfoJson, RawUseItemCount[]>(raw => raw.api_useitem),
                deserialized.OfData<RawUseItemCount[]>(),
            });
            UnequippedSlotItemInfoUpdated = Observable.Merge(new[]
            {
                deserialized.Parse<StartupInfoJson, RawUnequippedSlotItemInfo[]>(raw => raw.api_unsetslot),
                deserialized.OfData<RawUnequippedSlotItemInfo[]>(),
                deserialized.Parse<Ship3Json, RawUnequippedSlotItemInfo[]>(raw => raw.api_slot_data),
            });
            ConstructionDocksUpdated = Observable.Merge(new[]
            {
                deserialized.Parse<StartupInfoJson, RawConstructionDock[]>(raw => raw.api_kdock),
                deserialized.OfData<RawConstructionDock[]>(),
            });

            MapsUpdated = deserialized.Parse<MapInfoJson, RawMap[]>(raw => raw.api_map_info);
            AirForceGroupsUpdated = deserialized.Parse<MapInfoJson, RawAirForceGroup[]>(raw => raw.api_air_base);

            ShipUpdate = Observable.Merge(new[]
            {
                deserialized.OfData<RawShip>(),
                deserialized.OfData<RawShip[]>().SelectMany(ships => ships),
                deserialized.Parse<Ship3Json, RawShip[]>(raw => raw.api_ship_data).SelectMany(ships => ships),
                deserialized.Parse<ShipDeckJson, RawShip[]>(raw => raw.api_ship_data).SelectMany(ships => ships),
            });
            FleetUpdate = Observable.Merge(new[]
            {
                deserialized.Parse<Ship3Json, RawFleet[]>(raw => raw.api_deck_data),
                deserialized.Parse<ShipDeckJson, RawFleet[]>(raw => raw.api_deck_data),
            }).SelectMany(fleets => fleets);

            FleetCompositionChanged = deserialized.Parse("api_req_hensei/change", ParseFleetCompositionChange);

            ShipSupplied = deserialized.OfData<ShipsSupplyJson>().SelectMany(raw => raw.api_ship);

            RepairStarted = deserialized.Parse("api_req_nyukyo/start", ParseRepairStart);
            InstantRepairUsed = deserialized.Parse("api_req_nyukyo/speedchange", ParseInstantRepair);

            ConstructionStarted = deserialized.Parse("api_req_kousyou/createship", ParseConstructionStart);
            InstantConstructionUsed = deserialized.Parse("api_req_kousyou/createship_speedchange", ParseInstantConstruction);

            AirForceSquadronDeployed = deserialized.Parse<AirForceSquadronDeploymentJson, AirForceSquadronDeployment>((request, raw) =>
                ParseAirForceSquadronDeployment(request, raw));
            AirForceActionUpdated = deserialized.Parse("api_req_air_corps/set_action", ParseAirForceActionUpdates).SelectMany(updates => updates);

            QuestListUpdated = deserialized.Parse<QuestListJson, RawQuest[]>(raw => raw.api_list);
            QuestCompleted = deserialized.Parse("api_req_quest/clearitemget", ParseQuestCompleted);

            deserialized.Connect();

            MaterialUpdate = Observable.Merge(new[]
            {
                deserialized.Parse<HomeportJson, IMaterialUpdate>(raw => new HomeportMaterialUpdate(raw.api_material)),
                deserialized.Parse<RawMaterialItem[], IMaterialUpdate>(raw => new HomeportMaterialUpdate(raw)),
                deserialized.OfData<ShipsSupplyJson>(),
                ConstructionStarted,
                deserialized.OfData<AirForceSquadronDeploymentJson>(),
            });
        }

        private NameValueCollection ParseRequest(ReadOnlyMemory<char> request) =>
            HttpUtility.ParseQueryString(request.ToString());

        private SvData<T> Deserialize<T>(ApiMessage message) =>
            JsonSerializer.Deserialize<SvData<T>>(message.Response.Span, _serializerOptions)!;
        private SvDataRequestOnly DeserializeRequestOnly(ApiMessage message)
        {
            var result = JsonSerializer.Deserialize<SvDataRequestOnly>(message.Response.Span, _serializerOptions)!;
            result.Api = message.Api;
            result.Request = ParseRequest(message.Request);

            return result;
        }
        private SvDataWithRequest<T> DeserializeWithRequest<T>(ApiMessage message)
        {
            var result = JsonSerializer.Deserialize<SvDataWithRequest<T>>(message.Response.Span, _serializerOptions)!;
            result.Request = ParseRequest(message.Request);

            return result;
        }
    }
}
