using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews.Equipments
{
    public class EquipmentsGroupByLevel
    {
        public EquipmentGroupingKey Key { get; }

        Dictionary<int, EquipmentsGroupByFleet> r_Fleets;
        public IReadOnlyCollection<EquipmentsGroupByFleet> Fleets => r_Fleets.Values.OrderBy(r => r.FleetID).ToList();

        public int Count { get; set; }
        public int RemainingCount { get; set; }

        internal EquipmentsGroupByLevel(EquipmentGroupingKey rpKey, IEnumerable<Equipment> rpEquipments)
        {
            Key = rpKey;
            r_Fleets = new Dictionary<int, EquipmentsGroupByFleet>(5);

            Count = RemainingCount = rpEquipments.Count();
        }

        internal void Update(Ship rpShip)
        {
            var rFleetID = rpShip.OwnerFleet?.ID ?? int.MaxValue;

            EquipmentsGroupByFleet rFleet;
            if (!r_Fleets.TryGetValue(rFleetID, out rFleet))
                r_Fleets.Add(rFleetID, rFleet = new EquipmentsGroupByFleet(rFleetID));

            rFleet.Update(rpShip);

            RemainingCount--;
        }
    }
}
