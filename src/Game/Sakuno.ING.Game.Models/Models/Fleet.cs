using System;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.ING.Game.Models
{
    public partial class Fleet
    {
        private readonly BindableCollection<Ship> ships = new BindableCollection<Ship>();
        public IReadOnlyList<Ship> Ships => ships;

        partial void CreateDummy()
        {
            ships.ItemAdded += s => s.Fleet = this;
            ships.ItemRemoved += s => s.Fleet = null;
        }

        partial void UpdateCore(IRawFleet raw, DateTimeOffset timeStamp)
        {
            for (int i = 0; i < ships.Count || i < raw.ShipIds.Count; i++)
                if (i >= raw.ShipIds.Count)
                {
                    ships.RemoveAt(i);
                    i--;
                }
                else if (i >= ships.Count)
                    ships.Add(owner.AllShips[raw.ShipIds[i]]);
                else if (raw.ShipIds[i] != ships[i].Id)
                    ships[i] = owner.AllShips[raw.ShipIds[i]];

            Expedition = owner.MasterData.Expeditions[raw.ExpeditionId];
            UpdateStatus();
            UpdateTimer(timeStamp);
        }

        internal void ChangeComposition(int? index, Ship ship)
        {
            if (index is int i)
            {
                if (ship != null)
                {
                    var fromFleet = ship.Fleet;
                    if (fromFleet != null)
                    {
                        var oldIndex = fromFleet.ships.IndexOf(ship);
                        if (fromFleet == this)
                            ships.Exchange(i, oldIndex);
                        else
                        {
                            var oldShip = ships[i];
                            ships.RemoveAt(i);
                            fromFleet.ships.RemoveAt(oldIndex);
                            ships.Insert(i, oldShip);
                            fromFleet.ships.Insert(oldIndex, oldShip);
                        }
                    }
                    else
                        ships[i] = ship;
                }
                else
                    ships.RemoveAt(i);
            }
            else
                while (ships.Count > 1)
                    ships.RemoveAt(1);
        }

        internal bool Remove(Ship ship) => ships.Remove(ship);

        internal void UpdateTimer(DateTimeOffset timeStamp)
        {
            if (Expedition == null || timeStamp > ExpeditionCompletionTime)
                ExpeditionTimeRemaining = default;
            else
                ExpeditionTimeRemaining = ExpeditionCompletionTime - timeStamp;
        }

        internal void UpdateStatus()
        {
            SlowestShipSpeed = ships.Count > 0 ? ships.Min(s => s.Speed) : ShipSpeed.None;
            SupplyingCost = Ships.Sum(s => s.SupplyingCost);
            RepairingCost = Ships.Sum(s => s.RepairingCost);
            AirFightPower = Ships.Sum(s => s.AirFightPower);
            SimpleLos = Ships.Sum(s => s.LineOfSight.Displaying);
            EffectiveLoS = Ships.Sum(s => s.EffectiveLoS) + (2 * (6 - Ships.Count) - Math.Ceiling(0.4 * owner.Admiral.Leveling.Level));
        }
    }
}
