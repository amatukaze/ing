using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Services.Browser
{
    class LayoutEngineInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("displayname")]
        public string DisplayName { get; set; }

        [JsonProperty("entry")]
        public string EntryFile { get; set; }

        [JsonProperty("dependencies")]
        public LayoutEngineDependency[] Dependencies { get; set; }
    }
}
