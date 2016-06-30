using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class Admiral : RawDataWrapper<RawBasic>
    {
        public int ID => RawData.ID;
        public string Name => RawData.Name;
        public string Comment => RawData.Comment;

        public int Level => RawData.Level;
        public AdmiralRank Rank => RawData.Rank;
        public int Experience => RawData.Experience;
        public int ExperienceToNextLevel => ExperienceTable.GetAdmiralExperienceToNextLevel(Level, Experience);

        public int MaxShipCount => RawData.MaxShipCount;
        public int MaxEquipmentCount => RawData.MaxEquipmentCount;

        public int ResourceRegenerationLimit => Level * 250 + 750;

        public AdmiralRankingPoints RankingPoints { get; }

        internal Admiral(RawBasic rpRawData) : base(rpRawData)
        {
            RankingPoints = new AdmiralRankingPoints(this);
        }

        protected override void OnRawDataUpdated()
        {
            RankingPoints.Update();

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Level));
            OnPropertyChanged(nameof(Rank));
            OnPropertyChanged(nameof(Experience));
            OnPropertyChanged(nameof(ExperienceToNextLevel));

            OnPropertyChanged(nameof(MaxShipCount));
            OnPropertyChanged(nameof(MaxEquipmentCount));
        }

        public override string ToString() => $"ID = {ID}, Name = \"{Name}\", Level = {Level}";
    }
}
