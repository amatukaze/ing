using System;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.ING.Game.Models
{
    public partial class Fleet
    {
        partial void UpdateCore(IRawFleet raw, DateTimeOffset timeStamp)
        {
            ships.Query = raw.ShipIds.Select(x => owner.AllShips[x]);
            Expedition = owner.MasterData.Expeditions[raw.ExpeditionId];
            UpdateTimer(timeStamp);
            SlowestShipSpeed = ships.Count > 0 ? ships.Min(s => s.Speed) : ShipSpeed.None;
            UpdateSupplyingCost();
            UpdateRepairingCost();
        }

        internal void ChangeComposition(int? index, Ship ship, Fleet fromFleet)
        {
            var oldShips = ships.ToList();
            if (index is int i)
            {
                if (ship != null)
                {
                    if (fromFleet != null)
                        if (i >= ships.Count)
                            fromFleet.ships.Remove(ship);
                        else
                            fromFleet.ships.Replace(ship, ships[i]);
                    ships[i] = ship;
                }
                else
                    ships.RemoveAt(i);
            }
            else
                while (ships.Count > 1)
                    ships.RemoveAt(1);
        }

        internal void UpdateTimer(DateTimeOffset timeStamp)
        {
            if (Expedition == null)
                ExpeditionTimeRemaining = default;
            else
                ExpeditionTimeRemaining = ExpeditionCompletionTime - timeStamp;
        }

        internal bool IntersectWith(IEnumerable<ShipId> ids)
        {
            foreach (var id in ids)
                foreach (var ship in Ships)
                    if (id == ship.Id)
                        return true;
            return false;
        }

        internal void UpdateSupplyingCost()
        {
            Materials cost = default;
            foreach (var ship in Ships)
                cost += ship.SupplyingCost;
            SupplyingCost = cost;
        }

        internal void UpdateRepairingCost()
        {
            Materials cost = default;
            foreach (var ship in Ships)
                cost += ship.RepairingCost;
            RepairingCost = cost;
        }
    }
}
