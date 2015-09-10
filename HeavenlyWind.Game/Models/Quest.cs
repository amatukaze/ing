using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public enum QuestCategory { Composition = 1, Sortie, Practice, Expedition, SupplyOrDocking, Arsenal, Modernization }
    public enum QuestType { Once = 1, Daily, Weekly, Special1, Special2, Monthly, }
    public enum QuestState { None = 1, Progress, Completed }
    public enum QuestProgress { None, Progress50, Progress80, }

    public class Quest : RawDataWrapper<RawQuest>, IID
    {
        public int ID => RawData.ID;

        public string Name => RawData.Name;
        public string Description => RawData.Description;

        public QuestCategory Category => RawData.Category;
        public QuestType Type => RawData.Type;
        public QuestState State => RawData.State;
        public QuestProgress Progress => RawData.Progress;

        internal Quest(RawQuest rpRawData) : base(rpRawData) { }

        public override string ToString() => $"ID = {ID}, Category = {Category}, Name = \"{Name}\", State = {State}";
    }
}
