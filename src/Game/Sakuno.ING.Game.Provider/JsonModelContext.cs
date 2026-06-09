using System.Text.Json.Serialization;
using Sakuno.ING.Game.Provider.Json;
using Sakuno.ING.Game.Provider.Json.Converters;

namespace Sakuno.ING.Game.Provider;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata,
    Converters = [typeof(IdentifierConverterFactory), typeof(UnequippedSlotItemGroupConverter)])]
[JsonSerializable(typeof(SvData))]
[JsonSerializable(typeof(SvData<MasterDataJson>))]
[JsonSerializable(typeof(SvData<PreHomeportJson>))]
[JsonSerializable(typeof(SvData<HomeportJson>))]
[JsonSerializable(typeof(SvData<QuestListJson>))]
[JsonSerializable(typeof(SvData<Ship3Json>))]
[JsonSerializable(typeof(SvData<MapJson>))]
[JsonSerializable(typeof(SvData<RawAdmiral>))]
[JsonSerializable(typeof(SvData<RawMaterial[]>))]
[JsonSerializable(typeof(SvData<RawShip[]>))]
[JsonSerializable(typeof(SvData<RawSlotItem[]>))]
[JsonSerializable(typeof(SvData<RawFleet[]>))]
[JsonSerializable(typeof(SvData<RawConstructionDock[]>))]
[JsonSerializable(typeof(SvData<RawRepairDock[]>))]
[JsonSerializable(typeof(SvData<RawUseItemCount[]>))]
[JsonSerializable(typeof(SvData<RawUnequippedSlotItems[]>))]
public partial class JsonModelContext : JsonSerializerContext;
