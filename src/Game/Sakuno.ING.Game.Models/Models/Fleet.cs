using System;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.ING.Game.Models
{
    public partial class Fleet
    {
        private readonly BindableCollection<HomeportShip> ships = new BindableCollection<HomeportShip>();
        public IReadOnlyList<HomeportShip> Ships => ships;

        partial void CreateCore()
        {
            ships.ItemAdded += s =>
            {
                s.Fleet = this;
                UpdateState();
            };
            ships.ItemRemoved += s =>
            {
                s.Fleet = null;
                UpdateState();
            };
        }

        partial void UpdateCore(RawFleet raw, DateTimeOffset timeStamp)
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
            UpdateState();
            ExpeditionTimer.Init(raw.ExpeditionCompletionTime, timeStamp);
        }

        internal void ChangeComposition(int? index, HomeportShip ship)
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
                        else if (i < ships.Count)
                        {
                            var oldShip = ships[i];
                            ships.RemoveAt(i);
                            fromFleet.ships.RemoveAt(oldIndex);
                            ships.Insert(i, oldShip);
                            fromFleet.ships.Insert(oldIndex, oldShip);
                        }
                        else
                        {
                            fromFleet.ships.Remove(ship);
                            ships.Add(ship);
                        }
                    }
                    else if (i < ships.Count)
                        ships[i] = ship;
                    else
                        ships.Add(ship);
                }
                else
                    ships.RemoveAt(i);
            }
            else
                while (ships.Count > 1)
                    ships.RemoveAt(1);
        }

        internal bool Remove(HomeportShip ship) => ships.Remove(ship);

        internal bool UpdateTimer(DateTimeOffset timeStamp) => ExpeditionTimer.Update(timeStamp);

        public CountDown ExpeditionTimer { get; } = new CountDown();

        internal void UpdateState()
        {
            SlowestShipSpeed = ships.Count > 0 ? ships.Min(s => s.Speed) : ShipSpeed.None;
            SupplyingCost = Ships.Sum(s => s.SupplyingCost);
            RepairingCost = Ships.Sum(s => s.RepairingCost);
            AirFightPower = Ships.Sum(s => s.AirFightPower);
            SimpleLoS = Ships.Sum(s => s.LineOfSight.Displaying);
            var sumLoS = Ships.Sum(s => s.EffectiveLoS);
            EffectiveLoS = new LineOfSight(sumLoS.Multiplied, sumLoS.Baseline + 2 * (6 - Ships.Count), owner.Admiral.Leveling.Level);
            State = CheckFleetState();
        }

        private FleetState CheckFleetState()
        {
            if (Ships.Count == 0)
                return FleetState.Empty;
            if (Expedition != null)
                return FleetState.Expedition;
            if (Ships.Any(s => s.IsRepairing))
                return FleetState.Repairing;
            if (Ships.Any(s => s.HP.DamageState >= ShipDamageState.HeavilyDamaged))
                return FleetState.Damaged;
            if (Ships.Any(s => !s.Fuel.IsMaximum || !s.Bullet.IsMaximum))
                return FleetState.Insufficient;
            if (Ships.Any(s => s.Morale < 40))
                return FleetState.Fatigued;
            if (owner.CombinedFleet > 0 && (Id == 1 || Id == 2))
                return FleetState.Combined;
            return FleetState.Ready;
        }
    }
}
