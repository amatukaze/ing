using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class MapInfo : RawDataWrapper<RawMapInfo>, IID
    {
        public int ID => RawData.ID;

        public MapMasterInfo MasterInfo => KanColleGame.Current.MasterInfo.Maps[ID];

        public bool IsCleared => RawData.IsCleared;
        public bool IsIncompleted => RawData.IsIncompleted;

        public ClampedValue HP { get; private set; }

        public bool HasGauge => !IsCleared && HP != null && HP.Current > 0;

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

        public int AvailableAirBaseGroupCount => RawData.AvailableAirBaseGroupCount;

        internal MapInfo(RawMapInfo rpRawData) : base(rpRawData)
        {
            if (rpRawData.Event != null || rpRawData.DefeatedCount.HasValue)
                HP = new ClampedValue(9999, 9999);

            OnRawDataUpdated();
        }

        protected override void OnRawDataUpdated()
        {
            if (RawData.DefeatedCount is int defeatedCount && RawData.RequiredDefeatCount is int requredDefeatCount)
            {
                var rCurrent = requredDefeatCount - defeatedCount;

                if (HP != null)
                    HP.Set(requredDefeatCount, rCurrent);
                else
                {
                    HP = new ClampedValue(requredDefeatCount, rCurrent);
                    OnPropertyChanged(nameof(HP));
                }
                HP.Before = HP.Current;
            }
            else if (RawData.Event != null)
            {
                HP.Set(RawData.Event.HPMaximum, RawData.Event.HPCurrent);
                HP.Before = HP.Current;
                Difficulty = RawData.Event.SelectedDifficulty;
            }

            OnPropertyChanged(nameof(IsCleared));
            OnPropertyChanged(nameof(IsIncompleted));
            OnPropertyChanged(nameof(HasGauge));
        }

        internal void UpdateGauge() => OnPropertyChanged(nameof(HasGauge));

        public override string ToString() => $"ID = {ID}, Name = \"{MasterInfo.Name}\"";
    }
}
