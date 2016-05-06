using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class MapInfo : RawDataWrapper<RawMapInfo>, IID
    {
        public int ID => RawData.ID;

        public MapMasterInfo MasterInfo => KanColleGame.Current.MasterInfo.Maps[ID];

        public bool IsCleared => RawData.IsCleared;
        public bool IsIncompleted => RawData.IsIncompleted;

        ClampedValue r_HP;
        public ClampedValue HP
        {
            get { return r_HP; }
            internal set
            {
                r_HP = value;
                OnPropertyChanged(nameof(HP));
            }
        }

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

        public bool TransportEscortOnly => MasterInfo.SortieFleetType == CombinedFleetType.TransportEscort;
        public MapGaugeType? GaugeType => RawData.Event?.GaugeType;

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
        }

        public override string ToString() => $"ID = {ID}, Name = \"{MasterInfo.Name}\"";
    }
}
