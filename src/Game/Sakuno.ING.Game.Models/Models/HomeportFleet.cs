using System;
using System.Linq;

namespace Sakuno.ING.Game.Models
{
    public partial class HomeportFleet
    {
        private readonly BindableCollection<HomeportShip> ships = new BindableCollection<HomeportShip>();
        public IBindableCollection<HomeportShip> HomeportShips => ships;
        public override IBindableCollection<Ship> Ships => ships;
        protected override int AdmiralLevel => owner.Admiral.Leveling.Level;

        partial void CreateCore()
        {
            ships.ItemAdded += s =>
            {
                s.Fleet = this;
                CascadeUpdate();
            };
            ships.ItemRemoved += s =>
            {
                s.Fleet = null;
                CascadeUpdate();
            };
        }

        partial void UpdateCore(RawFleet raw, DateTimeOffset timeStamp)
        {
            HomeportShip Exclusive(HomeportShip ship)
            {
                if (ship.Fleet != null)
                    ship.Fleet.Remove(ship);
                return ship;
            }

            for (int i = 0; i < ships.Count || i < raw.ShipIds.Count; i++)
                if (i >= raw.ShipIds.Count)
                {
                    ships.RemoveAt(i);
                    i--;
                }
                else if (i >= ships.Count)
                    ships.Add(Exclusive(owner.AllShips[raw.ShipIds[i]]));
                else if (raw.ShipIds[i] != ships[i].Id)
                    ships[i] = Exclusive(owner.AllShips[raw.ShipIds[i]]);

            Expedition = owner.MasterData.Expeditions[raw.ExpeditionId];
            CascadeUpdate();
            if (ExpeditionTimer.SetCompletionTime(raw.ExpeditionCompletionTime, timeStamp))
                owner.Notification.SetExpeditionCompletion(this, raw.ExpeditionCompletionTime);
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
                            ships.Insert(i, ship);
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

        internal void UpdateTimer(DateTimeOffset timeStamp) => ExpeditionTimer.Update(timeStamp);

        public CountDown ExpeditionTimer { get; } = new CountDown();

        internal void CascadeUpdate()
        {
            DoCalculations();
            State = CheckFleetState();
        }

        private FleetState CheckFleetState()
        {
            if (Ships.AsList().Count == 0)
                return FleetState.Empty;
            if (Expedition != null)
                return FleetState.Expedition;
            if (ships.Any(s => s.IsRepairing))
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
