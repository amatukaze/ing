using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Services.Browser
{
    class LayoutEngineDependency
    {
        [JsonProperty("assembly")]
        public string AssemblyName { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }
    }
}
