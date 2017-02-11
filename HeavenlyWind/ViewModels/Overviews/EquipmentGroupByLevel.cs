using Sakuno.Collections;
using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews
{
    class EquipmentGroupByLevel : ModelBase
    {
        EquipmentGroupByMasterID r_Owner;

        public EquipmentGroupingKey Key { get; }

        ListDictionary<int, EquipmentGroupByFleet> r_Fleets;
        public IReadOnlyCollection<EquipmentGroupByFleet> Fleets => r_Fleets.Values.OrderBy(r => r.FleetID).ToList();

        public int Count { get; set; }
        public int RemainingCount { get; set; }

        internal EquipmentGroupByLevel(EquipmentGroupByMasterID rpOwner, EquipmentGroupingKey rpKey, IEnumerable<Equipment> rpEquipment)
        {
            r_Owner = rpOwner;

            Key = rpKey;
            r_Fleets = new ListDictionary<int, EquipmentGroupByFleet>();

            Count = rpEquipment.Count();

            Equipment[] rUnequipedEquipment;
            if (!KanColleGame.Current.Port.UnequippedEquipment.TryGetValue(r_Owner.Info.Type, out rUnequipedEquipment) || rUnequipedEquipment == null)
                RemainingCount = 0;
            else
                RemainingCount = rUnequipedEquipment.Count(r => r.Info == r_Owner.Info && r.Level == Key.Level && r.Proficiency == Key.Proficiency);
        }

        internal void Update(Ship rpShip)
        {
            var rFleetID = rpShip.OwnerFleet?.ID ?? int.MaxValue;

            EquipmentGroupByFleet rFleet;
            if (!r_Fleets.TryGetValue(rFleetID, out rFleet))
                r_Fleets.Add(rFleetID, rFleet = new EquipmentGroupByFleet(rFleetID));

            rFleet.Update(rpShip);
        }
    }
}
