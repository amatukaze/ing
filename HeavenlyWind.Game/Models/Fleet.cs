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

            if (r_ShipIDs == null || !r_ShipIDs.SequenceEqual(RawData.Ship))
            {
                if (r_ShipList != null)
                    foreach (var rShip in r_ShipList)
                        rShip.OwnerFleet = null;

                r_ShipIDs = RawData.Ship;
                r_ShipList = RawData.Ship.TakeWhile(r => r != -1).Select(r => Port.Ships[r]).ToList();
                
                foreach (var rShip in r_ShipList)
                    rShip.OwnerFleet = this;

                Ships = r_ShipList.AsReadOnly();

                ShipsUpdated(Ships);
            }

            Status.Update();
            ExpeditionStatus.Update(RawData.Expedition);

            UpdateState();
        }
        void UpdateState()
        {
            var rState = FleetState.None;

            if (ExpeditionStatus.Expedition != null)
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
