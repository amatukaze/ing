using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Json.Converters;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Messaging;
using System;
using System.Collections.Specialized;
using System.Reactive.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace Sakuno.ING.Game
{
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
                "api_get_member/mapinfo" =>  Deserialize<MapInfoJson>(message),

                _ => (SvData?)null,
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
            });
            ConstructionDocksUpdated = Observable.Merge(new[]
            {
                deserialized.Parse<StartupInfoJson, RawConstructionDock[]>(raw => raw.api_kdock),
                deserialized.OfData<RawConstructionDock[]>(),
            });

            MapsUpdated = deserialized.Parse<MapInfoJson, RawMap[]>(raw => raw.api_map_info);

            deserialized.Connect();

            FleetCompositionChanged = apiMessageSource.ApiMessageSource
                .Where(message => message.Api == "api_req_hensei/change")
                .Select(message => ParseFleetCompositionChange(ParseRequest(message.Request)));

            RepairStarted = apiMessageSource.ApiMessageSource
                .Where(message => message.Api == "api_req_nyukyo/start")
                .Select(message => ParseRepairStart(ParseRequest(message.Request)));
            InstantRepairUsed = apiMessageSource.ApiMessageSource
                .Where(message => message.Api == "api_req_nyukyo/speedchange")
                .Select(message => ParseInstantRepair(ParseRequest(message.Request)));

            ConstructionStarted = apiMessageSource.ApiMessageSource
                .Where(message => message.Api == "api_req_kousyou/createship")
                .Select(message => ParseConstructionStart(ParseRequest(message.Request)));
            InstantConstructionUsed = apiMessageSource.ApiMessageSource
                .Where(message => message.Api == "api_req_kousyou/createship_speedchange")
                .Select(message => ParseInstantConstruction(ParseRequest(message.Request)));

            MaterialUpdate = Observable.Merge(new[]
            {
                deserialized.Parse<HomeportJson, IMaterialUpdate>(raw => new HomeportMaterialUpdate(raw.api_material)),
                deserialized.Parse<RawMaterialItem[], IMaterialUpdate>(raw => new HomeportMaterialUpdate(raw)),
                ConstructionStarted,
            });
        }

        private SvData<T> Deserialize<T>(ApiMessage message) =>
            JsonSerializer.Deserialize<SvData<T>>(message.Response.Span, _serializerOptions)!;
        private NameValueCollection ParseRequest(ReadOnlyMemory<char> request) =>
            HttpUtility.ParseQueryString(request.ToString());
    }
}
