using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews
{
    public class EquipmentGroupByFleet : ModelBase
    {
        public int FleetID { get; }

        Dictionary<Ship, EquipmentGroupByShip> r_Ships;
        public IReadOnlyCollection<EquipmentGroupByShip> Ships
        {
            get
            {
                if (FleetID == int.MaxValue)
                    return r_Ships.Values.OrderByDescending(r => r.Ship.Level).ThenBy(r => r.Ship.SortNumber).ToList();
                else
                    return r_Ships.Values.ToList();
            }
        }

        internal EquipmentGroupByFleet(int rpFleetID)
        {
            FleetID = rpFleetID;
            r_Ships = new Dictionary<Ship, EquipmentGroupByShip>();
        }

        internal void Update(Ship rpShip)
        {
            EquipmentGroupByShip rShip;
            if (!r_Ships.TryGetValue(rpShip, out rShip))
                r_Ships.Add(rpShip, rShip = new EquipmentGroupByShip(rpShip));

            rShip.Count++;
        }
    }
}
