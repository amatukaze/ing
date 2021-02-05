using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Models.Quests
{
#nullable disable
    public sealed class RawQuest : IIdentifiable<QuestId>
    {
        [JsonPropertyName("api_no")]
        public QuestId Id { get; set; }

        [JsonPropertyName("api_title")]
        public string Name { get; set; }

        [JsonPropertyName("api_state")]
        public QuestState State { get; set; }
    }
#nullable enable
}
