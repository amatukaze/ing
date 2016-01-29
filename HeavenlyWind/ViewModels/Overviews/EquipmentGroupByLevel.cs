using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews
{
    public class EquipmentGroupByLevel
    {
        public EquipmentGroupingKey Key { get; }

        Dictionary<int, EquipmentGroupByFleet> r_Fleets;
        public IReadOnlyCollection<EquipmentGroupByFleet> Fleets => r_Fleets.Values.OrderBy(r => r.FleetID).ToList();

        public int Count { get; set; }
        public int RemainingCount { get; set; }

        internal EquipmentGroupByLevel(EquipmentGroupingKey rpKey, IEnumerable<Equipment> rpEquipment)
        {
            Key = rpKey;
            r_Fleets = new Dictionary<int, EquipmentGroupByFleet>(5);

            Count = RemainingCount = rpEquipment.Count();
        }

        internal void Update(Ship rpShip)
        {
            var rFleetID = rpShip.OwnerFleet?.ID ?? int.MaxValue;

            EquipmentGroupByFleet rFleet;
            if (!r_Fleets.TryGetValue(rFleetID, out rFleet))
                r_Fleets.Add(rFleetID, rFleet = new EquipmentGroupByFleet(rFleetID));

            rFleet.Update(rpShip);

            RemainingCount--;
        }
    }
}
