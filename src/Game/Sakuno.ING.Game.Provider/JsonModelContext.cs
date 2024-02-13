using Sakuno.ING.Game.Provider.Json;
using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Provider;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(SvData))]
[JsonSerializable(typeof(SvData<MasterDataJson>))]
[JsonSerializable(typeof(SvData<PreHomeportJson>))]
[JsonSerializable(typeof(SvData<HomeportJson>))]
[JsonSerializable(typeof(SvData<RawShip[]>))]
[JsonSerializable(typeof(SvData<RawSlotItem[]>))]
[JsonSerializable(typeof(SvData<RawFleet[]>))]
[JsonSerializable(typeof(SvData<RawConstructionDock[]>))]
[JsonSerializable(typeof(SvData<RawRepairDock[]>))]
[JsonSerializable(typeof(SvData<RawUseItemCount[]>))]
[JsonSerializable(typeof(SvData<RawUnequippedSlotItems[]>))]
public partial class JsonModelContext : JsonSerializerContext
{
}
