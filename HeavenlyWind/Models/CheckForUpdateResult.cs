using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models
{
    public enum CheckForUpdateFileAction { CreateOrOverwrite, Delete, Rename }

    public class CheckForUpdateResult : ModelBase
    {
        [JsonProperty("update")]
        public UpdateInfo Update { get; set; }

        [JsonProperty("files")]
        public File[] Files { get; set; }

        public class UpdateInfo : ModelBase
        {
            [JsonProperty("available")]
            public bool IsAvailable { get; set; }

            [JsonProperty("version")]
            public string Version { get; set; }
            [JsonProperty("important")]
            public bool IsImportantUpdate { get; set; }

            [JsonProperty("link")]
            public string Link { get; set; }
        }
        public class File : ModelBase
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("action")]
            public CheckForUpdateFileAction Action { get; set; }

            [JsonProperty("timestamp")]
            public long Timestamp { get; set; }

            [JsonProperty("content")]
            public string Content { get; set; }
        }
    }
}
