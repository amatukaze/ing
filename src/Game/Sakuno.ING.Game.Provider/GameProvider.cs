using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Json.Converters;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Messaging;
using System.Reactive.Linq;
using System.Text.Json;

namespace Sakuno.ING.Game
{
    public sealed partial class GameProvider
    {
        private readonly JsonSerializerOptions _serializerOptions;

        public GameProvider(IApiMessageSource apiMessageSource)
        {
            _serializerOptions = new JsonSerializerOptions();
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

                _ => (SvData?)null,
            }).Publish();

            var successful = deserialized.Where(svdata => svdata != null && svdata.api_result == 1);

            MasterDataUpdated = successful.Parse<MasterDataJson, MasterDataUpdate>(raw => new MasterDataUpdate
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

            SlotItemsUpdated = successful.Parse<StartupInfoJson, RawSlotItem[]>(raw => raw.api_slot_item);
            ConstructionDocksUpdated = successful.Parse<StartupInfoJson, RawConstructionDock[]>(raw => raw.api_kdock);
            UseItemsUpdated = successful.Parse<StartupInfoJson, RawUseItemCount[]>(raw => raw.api_useitem);
            UnequippedSlotItemInfoUpdated = successful.Parse<StartupInfoJson, RawUnequippedSlotItemInfo[]>(raw => raw.api_unsetslot);

            ShipsUpdate = successful.Parse<HomeportJson, RawShip[]>(raw => raw.api_ship);
            RepairDocksUpdate = successful.Parse<HomeportJson, RawRepairDock[]>(raw => raw.api_ndock);
            FleetsUpdate = successful.Parse<HomeportJson, RawFleet[]>(raw => raw.api_deck_port);

            MaterialUpdate = Observable.Merge
            (
                successful.Parse<HomeportJson, IMaterialUpdate>(raw => new HomeportMaterialUpdate(raw.api_material))
            );

            deserialized.Connect();
        }

        private SvData<T> Deserialize<T>(ApiMessage message) =>
            JsonSerializer.Deserialize<SvData<T>>(message.Response.Span, _serializerOptions)!;
    }
}
