using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.Models
{
    public class EventMap : ModelBase
    {
        public int ID { get; }

        public bool NeedCombinedFleet { get; }

        public EventMap(MapMasterInfo rpMapInfo)
        {
            ID = rpMapInfo.AreaSubID;

            NeedCombinedFleet = rpMapInfo.SortieFleetType > 0;
        }
    }
}
