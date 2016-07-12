using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Services.Quest;
using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public enum QuestCategory { Composition = 1, Sortie, Practice, Expedition, SupplyOrDocking, Arsenal, Modernization, Sortie2 }
    public enum QuestType { Daily = 1, Weekly, Monthly, Once, Special }
    public enum QuestState { None = 1, Active, Completed }
    public enum QuestProgress { None, Progress50, Progress80, }

    public class Quest : RawDataWrapper<RawQuest>, IID, ITranslatedName
    {
        public static Quest Dummy { get; } = new Quest(new RawQuest() { ID = -1, Name = StringResources.Instance.Main.Quest_Unknown });

        public int ID => RawData.ID;

        public string Name => RawData.Name;
        public string TranslatedName => StringResources.Instance.Extra?.GetQuestName(ID) ?? Name;
        public string Description => RawData.Description;

        public int FuelReward => RawData.RewardMaterials[0];
        public int BulletReward => RawData.RewardMaterials[1];
        public int SteelReward => RawData.RewardMaterials[2];
        public int BauxiteReward => RawData.RewardMaterials[3];

        public QuestCategory Category => RawData.Category;
        public QuestType Type => RawData.Type;
        public QuestState State => RawData.State;
        public QuestProgress Progress => RawData.Progress;

        internal DateTimeOffset CreationTime { get; } = DateTimeOffset.Now;

        public ProgressInfo RealtimeProgress { get; internal set; }

        internal Quest(RawQuest rpRawData) : base(rpRawData) { }

        public override string ToString() => $"ID = {ID}, Category = {Category}, Name = \"{Name}\", State = {State}";
    }
}
