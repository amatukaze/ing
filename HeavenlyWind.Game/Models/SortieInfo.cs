namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class SortieInfo : ModelBase
    {
        public MapMasterInfo Map { get; }

        SortieCellInfo r_Cell;
        public SortieCellInfo Cell
        {
            get { return r_Cell; }
            internal set
            {
                if (r_Cell != value)
                {
                    r_Cell = value;
                    OnPropertyChanged(nameof(Cell));
                }
            }
        }

        int r_DroppedShipCount;
        public int DroppedShipCount
        {
            get { return r_DroppedShipCount; }
            internal set
            {
                if (r_DroppedShipCount != value)
                {
                    r_DroppedShipCount = value;
                    OnPropertyChanged(nameof(DroppedShipCount));
                }
            }
        }

        internal SortieInfo(int rpMapID)
        {
            Map = KanColleGame.Current.MasterInfo.Maps[rpMapID];
        }
    }
}
