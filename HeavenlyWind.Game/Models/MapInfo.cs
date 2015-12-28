using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Services;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class MapInfo : RawDataWrapper<RawMapInfo>, IID
    {
        public int ID => RawData.ID;

        public MapMasterInfo MasterInfo => KanColleGame.Current.MasterInfo.Maps[ID];

        public bool IsCleared => RawData.IsCleared;
        public bool IsIncompleted => RawData.IsIncompleted;

        public ClampedValue HP { get; private set; }

        public bool IsEventMap => RawData.Event != null;

        EventMapDifficulty? r_Difficulty;
        public EventMapDifficulty? Difficulty
        {
            get { return r_Difficulty; }
            internal set
            {
                if (r_Difficulty != value)
                {
                    r_Difficulty = value;
                    OnPropertyChanged(nameof(Difficulty));
                }
            }
        }

        internal MapInfo(RawMapInfo rpRawData) : base(rpRawData)
        {
            OnRawDataUpdated();
        }

        protected override void OnRawDataUpdated()
        {
            if (RawData.DefeatedCount.HasValue)
            {
                var rMaximum = MasterInfo.RequiredDefeatCount.Value;
                var rCurrent = rMaximum - RawData.DefeatedCount.Value;

                HP = new ClampedValue(rMaximum, rCurrent);
            }
            else if (RawData.Event != null)
            {
                HP = new ClampedValue(RawData.Event.HPMaximum, RawData.Event.HPCurrent);
                Difficulty = RawData.Event.SelectedDifficulty;
            }

            OnPropertyChanged(nameof(IsCleared));
            OnPropertyChanged(nameof(IsIncompleted));
            OnPropertyChanged(nameof(HP));
        }

        public override string ToString() => $"ID = {ID}, Name = \"{MasterInfo.Name}\"";
    }
}
