using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class Fleet : RawDataWrapper<RawFleet>, IID
    {
        public Port Port { get; }

        public int ID => RawData.ID;

        string r_Name;
        public string Name
        {
            get { return r_Name; }
            internal set
            {
                if (r_Name != value)
                {
                    r_Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        int[] r_ShipIDs;
        List<Ship> r_ShipList;
        ReadOnlyCollection<Ship> r_Ships;
        public ReadOnlyCollection<Ship> Ships
        {
            get { return r_Ships; }
            private set
            {
                if (r_Ships != value)
                {
                    r_Ships = value;
                    OnPropertyChanged(nameof(Ships));
                }
            }
        }

        FleetState r_State;
        public FleetState State
        {
            get { return r_State; }
            private set
            {
                if (r_State != value)
                {
                    r_State = value;
                    OnPropertyChanged(nameof(State));
                }
            }
        }

        public FleetStatus Status { get; }
        public FleetExpeditionStatus ExpeditionStatus { get; }

        public event Action<IEnumerable<Ship>> ShipsUpdated = delegate { };

        internal Fleet(Port rpPort, RawFleet rpRawData) : base(rpRawData)
        {
            Port = rpPort;

            Status = new FleetStatus(this);
            ExpeditionStatus = new FleetExpeditionStatus(this);

            OnRawDataUpdated();
        }

        protected override void OnRawDataUpdated()
        {
            Name = RawData.Name;

            if (r_ShipIDs == null || !r_ShipIDs.SequenceEqual(RawData.Ships))
            {
                if (r_ShipList != null)
                    foreach (var rShip in r_ShipList)
                        rShip.OwnerFleet = null;

                r_ShipIDs = RawData.Ships;
                r_ShipList = RawData.Ships.TakeWhile(r => r != -1).Select(r => Port.Ships[r]).ToList();

                foreach (var rShip in r_ShipList)
                    rShip.OwnerFleet = this;

                UpdateShips();
            }

            Update();
        }

        internal void Update()
        {
            Status.Update();
            ExpeditionStatus.Update(RawData.Expedition);

            var rState = FleetState.None;

            if (KanColleGame.Current.Sortie?.Fleet == this ||
                KanColleGame.Current.Port.Fleets.CombinedFleetType != CombinedFleetType.None && KanColleGame.Current.Sortie?.Fleet.ID == 1 && ID == 2)
                rState |= FleetState.Sortie;
            else if (ExpeditionStatus.Expedition != null)
                rState |= FleetState.Expedition;
            else
                rState |= FleetState.Idle;

            if ((rState & FleetState.Idle) == FleetState.Idle)
            {
                if (r_Ships.Any(r => r.Fuel.Current < r.Fuel.Maximum || r.Bullet.Current < r.Bullet.Maximum))
                    rState |= FleetState.Unsupplied;

                if (r_Ships.Any(r => Port.RepairDocks.Values.Any(rpDock => rpDock.Ship == r)))
                    rState |= FleetState.Repairing;

                if (r_Ships.Any(r => (r.State & ShipState.HeavilyDamaged) == ShipState.HeavilyDamaged))
                    rState |= FleetState.HeavilyDamaged;
            }

            State = rState;
        }

        void UpdateShips()
        {
            Ships = r_ShipList.AsReadOnly();

            ShipsUpdated(Ships);
        }

        public Ship Organize(int rpIndex, Ship rpShip)
        {
            var rOriginalShip = rpIndex < r_ShipList.Count ? r_ShipList[rpIndex] : null;
            if (rOriginalShip != null)
                rOriginalShip.OwnerFleet = null;

            if (rpIndex >= r_ShipList.Count)
            {
                r_ShipList.Add(rpShip);
                r_ShipIDs = r_ShipList.Select(r => r.ID).ToArray();
            }
            else
            {
                r_ShipIDs[rpIndex] = rpShip.ID;
                r_ShipList[rpIndex] = rpShip;
            }

            rpShip.OwnerFleet = this;

            UpdateShips();

            return rOriginalShip;
        }

        public void Remove(int rpIndex)
        {
            var rShip = r_ShipList[rpIndex];
            rShip.OwnerFleet = null;

            r_ShipList.RemoveAt(rpIndex);
            r_ShipIDs = r_ShipList.Select(r => r.ID).ToArray();

            UpdateShips();
        }
        public void Remove(Ship rpShip)
        {
            var rIndex = r_ShipList.IndexOf(rpShip);
            if (rIndex != -1)
                Remove(rIndex);
        }
        public void RemoveAllExceptFlagship()
        {
            foreach (var rShip in r_ShipList.Skip(1))
                rShip.OwnerFleet = null;

            r_ShipIDs = r_ShipIDs.Take(1).ToArray();
            r_ShipList.RemoveRange(1, r_ShipList.Count - 1);

            UpdateShips();
        }

        public override string ToString()
        {
            var rBuilder = new StringBuilder(64);

            rBuilder.Append($"ID = {ID}, Name = \"{Name}\", Ship");
            if (Ships.Count > 1)
                rBuilder.Append('s');
            rBuilder.Append(" = ").Append(Ships.Select(r => $"\"{r.Info.Name}\"").Join(", "));

            return rBuilder.ToString();
        }
    }
}
