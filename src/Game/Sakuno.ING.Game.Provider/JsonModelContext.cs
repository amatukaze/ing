using Sakuno.ING.Game.Provider.Json;
using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Provider;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(SvData))]
[JsonSerializable(typeof(SvData<MasterDataJson>))]
public partial class JsonModelContext : JsonSerializerContext
{
}
