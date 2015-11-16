using Sakuno.KanColle.Amatsukaze.Game.Services;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class SortieInfo : ModelBase
    {
        static SortieInfo r_Current;

        public Fleet Fleet { get; }
        public MapMasterInfo Map { get; }

        SortieCellInfo r_Cell;
        public SortieCellInfo Cell
        {
            get { return r_Cell; }
            private set
            {
                if (r_Cell != value)
                {
                    r_Cell = value;
                    OnPropertyChanged(nameof(Cell));
                }
            }
        }

        static SortieInfo()
        {
            SessionService.Instance.Subscribe("api_port/port", _ => r_Current = null);
        }
        internal SortieInfo(Fleet rpFleet, int rpMapID)
        {
            r_Current = this;

            Fleet = rpFleet;
            Map = KanColleGame.Current.MasterInfo.Maps[rpMapID];
        }
    }
}
