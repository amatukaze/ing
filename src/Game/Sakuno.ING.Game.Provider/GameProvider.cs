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
                "api_req_hensei/lock" => DeserializeWithRequest<RawShipLockInfo>(message),
                "api_req_hensei/preset_select" => Deserialize<RawFleet>(message),
                "api_req_hokyu/charge" => Deserialize<ShipsSupplyJson>(message),
                "api_req_kaisou/lock" => DeserializeWithRequest<RawSlotItemLockInfo>(message),
                "api_req_kaisou/slot_deprive" => Deserialize<SlotItemTransferJson>(message),
                "api_req_kaisou/powerup" => DeserializeWithRequest<ShipModernizationResultJson>(message),
                "api_req_kaisou/marriage" => Deserialize<RawShip>(message),
                "api_req_kousyou/getship" => Deserialize<ShipConstructionResultJson>(message),
                "api_req_kousyou/createitem" => DeserializeWithRequest<SlotItemsDevelopedJson>(message),
                "api_req_kousyou/destroyship" => DeserializeWithRequest<ShipsDismantlingJson>(message),
                "api_req_kousyou/destroyitem2" => DeserializeWithRequest<SlotItemsScrappingJson>(message),
                "api_req_kousyou/remodel_slot" => DeserializeWithRequest<SlotItemImprovementJson>(message),
                "api_req_air_corps/set_plane" => DeserializeWithRequest<AirForceSquadronDeploymentJson>(message),
                "api_req_air_corps/supply" => DeserializeWithRequest<AirForceSquadronSupplyJson>(message),
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

            ShipsUpdated = deserialized.Parse<HomeportJson, RawShip[]>(raw => raw.api_ship);

            FleetsUpdated = Observable.Merge(new[]
            {
                deserialized.Parse<HomeportJson, RawFleet[]>(raw => raw.api_deck_port),
                deserialized.OfData<RawFleet[]>(),
                deserialized.Parse<ShipModernizationResultJson, RawFleet[]>(raw => raw.api_deck),
                deserialized.Parse<Ship3Json, RawFleet[]>(raw => raw.api_deck_data),
            });
            SlotItemsUpdated = Observable.Merge(new[]
            {
                deserialized.Parse<StartupInfoJson, RawSlotItem[]>(raw => raw.api_slot_item),
                deserialized.OfData<RawSlotItem[]>(),
            });
            RepairDocksUpdated = Observable.Merge(new[]
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
                deserialized.Parse<ShipConstructionResultJson, RawConstructionDock[]>(raw => raw.api_kdock),
            });

            MapsUpdated = deserialized.Parse<MapInfoJson, RawMap[]>(raw => raw.api_map_info);
            AirForceGroupsUpdated = deserialized.Parse<MapInfoJson, RawAirForceGroup[]>(raw => raw.api_air_base);

            FleetCompositionChanged = deserialized.Parse("api_req_hensei/change", ParseFleetCompositionChange);

            ShipLocked = deserialized.OfDataWithRequest<RawShipLockInfo>().Where(raw => raw.api_data.api_locked).Select(raw => (ShipId)raw.Request.GetInt("api_ship_id"));
            ShipUnlocked = deserialized.OfDataWithRequest<RawShipLockInfo>().Where(raw => !raw.api_data.api_locked).Select(raw => (ShipId)raw.Request.GetInt("api_ship_id"));

            ShipSupplied = deserialized.OfData<ShipsSupplyJson>().SelectMany(raw => raw.api_ship);

            SlotItemLocked = deserialized.OfDataWithRequest<RawSlotItemLockInfo>().Where(raw => raw.api_data.api_locked).Select(raw => (SlotItemId)raw.Request.GetInt("api_slotitem_id"));
            SlotItemUnlocked = deserialized.OfDataWithRequest<RawSlotItemLockInfo>().Where(raw => !raw.api_data.api_locked).Select(raw => (SlotItemId)raw.Request.GetInt("api_slotitem_id"));

            ShipModernization = deserialized.Parse<ShipModernizationResultJson, ShipModernization>(ParseShipModernization);

            RepairStarted = deserialized.Parse("api_req_nyukyo/start", ParseRepairStart);
            InstantRepairUsed = deserialized.Parse("api_req_nyukyo/speedchange", ParseInstantRepair);

            ConstructionStarted = deserialized.Parse("api_req_kousyou/createship", ParseConstructionStart);
            InstantConstructionUsed = deserialized.Parse("api_req_kousyou/createship_speedchange", ParseInstantConstruction);

            SlotItemsDeveloped = deserialized.Parse<SlotItemsDevelopedJson, SlotItemsDeveloped>(ParseSlotItemsDeveloped);

            ShipsDismantled = deserialized.OfDataWithRequest<ShipsDismantlingJson>().Select(ParseShipDismantled);
            SlotItemsScrapped = deserialized.OfDataWithRequest<SlotItemsScrappingJson>().Select(raw => raw.Request.GetSlotItemIds("api_slotitem_ids"));

            SlotItemImproved = deserialized.OfDataWithRequest<SlotItemImprovementJson>().Select(ParseSlotItemImproved);

            AirForceGroupActionUpdated = deserialized.Parse("api_req_air_corps/set_action", ParseAirForceGroupActionUpdates).SelectMany(updates => updates);
            AirForceSquadronDeployed = deserialized.Parse<AirForceSquadronDeploymentJson, AirForceSquadronDeployment>(ParseAirForceSquadronDeployment);
            AirForceSquadronSupplied = deserialized.Parse<AirForceSquadronSupplyJson, AirForceSquadronSupplied>(ParseAirForceSquadronSupply);

            QuestListUpdated = deserialized.Parse<QuestListJson, RawQuest[]>(raw => raw.api_list);
            QuestCompleted = deserialized.Parse("api_req_quest/clearitemget", ParseQuestCompleted);

            MaterialUpdated = Observable.Merge(new[]
            {
                deserialized.Parse((Func<HomeportJson, IMaterialUpdate>)(raw => new HomeportMaterialUpdate(raw.api_material))),
                deserialized.Parse((Func<RawMaterialItem[], IMaterialUpdate>)(raw => new HomeportMaterialUpdate(raw))),
                deserialized.OfData<ShipsSupplyJson>(),
                ConstructionStarted,
                deserialized.OfData<SlotItemsDevelopedJson>(),
                deserialized.OfDataWithRequest<ShipsDismantlingJson>().Select(raw => raw.api_data),
                deserialized.OfDataWithRequest<SlotItemsScrappingJson>().Select(raw => raw.api_data),
                deserialized.OfDataWithRequest<SlotItemImprovementJson>().Select(raw => raw.api_data),
                deserialized.OfData<AirForceSquadronDeploymentJson>(),
                deserialized.OfData<AirForceSquadronSupplyJson>(),
            });

            SlotItemUpdated = Observable.Merge(new[]
            {
                deserialized.Parse<ShipConstructionResultJson, RawSlotItem[]>(raw => raw.api_slotitem).SelectMany(slotItems => slotItems),
                //SlotItemsDeveloped.SelectMany(message => message.SlotItems).Where(slotItem => slotItem is not null),

                SlotItemImproved.Where(message => message.IsSuccessful).Select(message => message.NewRawData),
            });

            ShipUpdated = Observable.Merge(new[]
            {
                deserialized.OfData<RawShip>(),
                deserialized.OfData<RawShip[]>().SelectMany(ships => ships),
                deserialized.Parse<Ship3Json, RawShip[]>(raw => raw.api_ship_data).SelectMany(ships => ships),
                deserialized.Parse<ShipDeckJson, RawShip[]>(raw => raw.api_ship_data).SelectMany(ships => ships),
                deserialized.Parse<ShipModernizationResultJson, RawShip>(raw => raw.api_ship),
                deserialized.OfData<SlotItemTransferJson>().SelectMany(raw => raw.Ships),
                deserialized.Parse<ShipConstructionResultJson, RawShip>(raw => raw.api_ship),
            });
            FleetUpdated = Observable.Merge(new[]
            {
                deserialized.OfData<RawFleet>(),
                deserialized.Parse<ShipDeckJson, RawFleet[]>(raw => raw.api_deck_data).SelectMany(fleets => fleets),
            });

            deserialized.Connect();
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
