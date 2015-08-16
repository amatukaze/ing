using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public class FleetManager : ModelBase
    {
        public IDTable<Fleet> Table { get; } = new IDTable<Fleet>();

        public CombinedFleetType CombinedFleetType { get; internal set; }

        public Fleet this[int rpID] => Table[rpID];

        internal FleetManager()
        {

        }

        internal void Update(RawPort rpPort)
        {
            CombinedFleetType = rpPort.CombinedFleetType;

            Table.UpdateRawData<RawFleet>(rpPort.Fleets, r => new Fleet(KanColleGame.Current.Port, r), (rpData, rpRawData) => rpData.Update(rpRawData));
        }
    }
}
