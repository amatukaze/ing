using System;
using Newtonsoft.Json;
using Sakuno.ING.Game.Json.Converters;

namespace Sakuno.ING.Game.Models.Quests
{
    [Identifier]
    public readonly struct QuestId : IEquatable<QuestId>, IComparable<QuestId>
    {
        private readonly int value;
        public QuestId(int value) => this.value = value;

        public int CompareTo(QuestId other) => value - other.value;
        public bool Equals(QuestId other) => value == other.value;

        public static implicit operator int(QuestId id) => id.value;
        public static explicit operator QuestId(int value) => new QuestId(value);

        public static bool operator ==(QuestId left, QuestId right) => left.value == right.value;
        public static bool operator !=(QuestId left, QuestId right) => left.value != right.value;
        public override bool Equals(object obj) => (QuestId)obj == this;
        public override int GetHashCode() => value;
        public override string ToString() => value.ToString();
    }

    public sealed class RawQuest : IIdentifiable<QuestId>
    {
        internal RawQuest() { }

        [JsonProperty("api_no")]
        public QuestId Id { get; internal set; }

        internal int api_category;
        public QuestCategory Category =>
            api_category == 8 || api_category == 9 ?
            QuestCategory.Sortie : (QuestCategory)api_category;

        internal int api_type;
        public QuestPeriod Period =>
            Id == 211 || Id == 212 ?
            QuestPeriod.Daily : (QuestPeriod)api_type;

        [JsonProperty("api_state")]
        public QuestState State { get; internal set; }
        [JsonProperty("api_title")]
        public string Name { get; internal set; }
        [JsonProperty("api_detail"), JsonConverter(typeof(HtmlNewLineEater))]
        public string Description { get; internal set; }
        [JsonProperty("api_get_material"), JsonConverter(typeof(MaterialsConverter))]
        public Materials Rewards { get; internal set; }
        [JsonProperty("api_progress_flag")]
        public QuestProgress Progress { get; internal set; }
    }

    public enum QuestCategory
    {
        Composition = 1,
        Sortie = 2,
        Exercise = 3,
        Expedition = 4,
        Supply = 5,
        Arsenal = 6,
        Mordenization = 7
    }

    public enum QuestPeriod
    {
        Daily = 1,
        Weekly = 2,
        Monthly = 3,
        Once = 4,
        Quarterly = 5
    }

    public enum QuestState
    {
        Inactive = 1,
        Active = 2,
        Completed = 3
    }

    public enum QuestProgress
    {
        None = 0,
        Half = 1,
        Almost = 2
    }
}
