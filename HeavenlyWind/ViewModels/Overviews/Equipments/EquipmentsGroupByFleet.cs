using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews.Equipments
{
    public class EquipmentsGroupByFleet
    {
        public int FleetID { get; }

        Dictionary<Ship, EquipmentsGroupByShip> r_Ships;
        public IReadOnlyCollection<EquipmentsGroupByShip> Ships
        {
            get
            {
                if (FleetID == int.MaxValue)
                    return r_Ships.Values.OrderByDescending(r => r.Ship.Level).ThenBy(r => r.Ship.SortNumber).ToList();
                else
                    return r_Ships.Values.ToList();
            }
        }

        internal EquipmentsGroupByFleet(int rpFleetID)
        {
            FleetID = rpFleetID;
            r_Ships = new Dictionary<Ship, EquipmentsGroupByShip>();
        }

        internal void Update(Ship rpShip)
        {
            EquipmentsGroupByShip rShip;
            if (!r_Ships.TryGetValue(rpShip, out rShip))
                r_Ships.Add(rpShip, rShip = new EquipmentsGroupByShip(rpShip));

            rShip.Count++;
        }
    }
}
