using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.Collections.Generic;
using System.Linq;

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
        IList<Ship> r_Ships;
        public IList<Ship> Ships
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
        public FleetResupplyInfo Resupply { get; }
        public FleetExpeditionStatus ExpeditionStatus { get; }
        public FleetConditionRegeneration ConditionRegeneration { get; }
        public FleetAnchorageRepair AnchorageRepair { get; }

        internal Fleet(Port rpPort, RawFleet rpRawData) : base(rpRawData)
        {
            Port = rpPort;

            Status = new FleetStatus(this);
            Resupply = new FleetResupplyInfo(this);
            ExpeditionStatus = new FleetExpeditionStatus(this);
            ConditionRegeneration = new FleetConditionRegeneration(this);
            AnchorageRepair = new FleetAnchorageRepair(this);

            OnRawDataUpdated();
        }

        protected override void OnRawDataUpdated()
        {
            Name = RawData.Name;

            if (r_ShipIDs == null || !r_ShipIDs.SequenceEqual(RawData.Ships))
            {
                r_ShipIDs = RawData.Ships;
                r_ShipList = RawData.Ships.TakeWhile(r => r != -1).Select(r => Port.Ships[r]).ToList();

                UpdateShips();
            }

            Update();
        }

        internal void Update()
        {
            Status.Update();
            Resupply.Update();
            ExpeditionStatus.Update(RawData.Expedition);

            var rState = FleetState.None;
            var rSortie = KanColleGame.Current.Sortie;
            if (rSortie?.Fleet == this || Port.Fleets.CombinedFleetType != CombinedFleetType.None && rSortie?.Fleet.ID == 1 && ID == 2)
                rState |= FleetState.Sortie;
            else if (ExpeditionStatus.Expedition != null)
                rState |= FleetState.Expedition;
            else
                rState |= FleetState.Idle;

            Ship[] rShipsToBeRepaired = null;
            if ((rState & FleetState.Idle) != 0)
            {
                if (r_Ships.Any(r => r.Fuel.Current < r.Fuel.Maximum || r.Bullet.Current < r.Bullet.Maximum))
                    rState |= FleetState.Unsupplied;

                if (r_Ships.Any(r => Port.RepairDocks.Values.Any(rpDock => rpDock.Ship == r)))
                    rState |= FleetState.Repairing;

                if ((rState & FleetState.Repairing) == 0 && r_Ships.Any(r => (r.State & ShipState.HeavilyDamaged) != 0))
                    rState |= FleetState.HeavilyDamaged;

                if (r_Ships.Count > 0 && (ShipType)r_Ships[0].Info.Type.ID == ShipType.RepairShip)
                {
                    rShipsToBeRepaired = r_Ships.Take(2 + r_Ships[0].EquipedEquipment.Count(r => r.Info.Type == EquipmentType.ShipRepairFacility))
                        .Where(r => r.DamageState != ShipDamageState.FullyHealthy && r.DamageState < ShipDamageState.ModeratelyDamaged && !Port.RepairDocks.Values.Any(rpDock => rpDock.Ship == r)).ToArray();
                    if (rShipsToBeRepaired.Length > 0)
                        rState |= FleetState.AnchorageRepair;
                }
            }

            if ((rState & FleetState.Sortie) == 0)
                ConditionRegeneration.Update();
            else
                ConditionRegeneration.Reset();

            if ((rState & FleetState.AnchorageRepair) != 0)
                AnchorageRepair.Update(rShipsToBeRepaired);
            else if ((State & FleetState.AnchorageRepair) != 0)
                AnchorageRepair.Stop();

            var rConditionType = ShipConditionType.HighMorale;
            foreach (var rShip in Ships)
                if (rConditionType < rShip.ConditionType)
                    rConditionType = rShip.ConditionType;

            switch (rConditionType)
            {
                case ShipConditionType.HighMorale:
                    rState |= FleetState.HighMorale;
                    break;
                case ShipConditionType.ModerateTired:
                    rState |= FleetState.ModerateTired;
                    break;
                case ShipConditionType.SeriouslyTired:
                    rState |= FleetState.SeriouslyTired;
                    break;
            }

            State = rState;
        }

        void UpdateShips() => Ships = r_ShipList.AsReadOnly();

        internal Ship Organize(int rpIndex, Ship rpShip)
        {
            var rOriginalShip = rpIndex < r_ShipList.Count ? r_ShipList[rpIndex] : null;

            if (rpIndex >= r_ShipList.Count)
            {
                r_ShipList.Add(rpShip);
                r_ShipIDs = r_ShipList.Select(r => r.ID).ToArray();
            }
            else if (rpShip != null)
            {
                r_ShipIDs[rpIndex] = rpShip.ID;
                r_ShipList[rpIndex] = rpShip;
            }
            else
            {
                r_ShipList.RemoveAt(rpIndex);
                r_ShipIDs = r_ShipList.Select(r => r.ID).ToArray();
            }

            UpdateShips();

            return rOriginalShip;
        }
        internal void Swap(int x, int y)
        {
            if (x < r_ShipList.Count)
            {
                var rShip = r_ShipList[x];
                r_ShipList[x] = r_ShipList[y];
                r_ShipList[y] = rShip;
            }
            else
            {
                var rShip = r_ShipList[y];
                r_ShipList.RemoveAt(y);
                r_ShipList.Add(rShip);
            }

            UpdateShips();
        }

        internal void Remove(int rpIndex)
        {
            r_ShipList.RemoveAt(rpIndex);
            r_ShipIDs = r_ShipList.Select(r => r.ID).ToArray();

            UpdateShips();
        }
        internal void Remove(Ship rpShip)
        {
            var rIndex = r_ShipList.IndexOf(rpShip);
            if (rIndex != -1)
                Remove(rIndex);
        }
        internal void RemoveAllExceptFlagship()
        {
            r_ShipIDs = r_ShipIDs.Take(1).ToArray();
            r_ShipList.RemoveRange(1, r_ShipList.Count - 1);

            UpdateShips();
        }

        public override string ToString()
        {
            var rBuilder = StringBuilderCache.Acquire();

            rBuilder.Append($"ID = {ID}, Name = \"{Name}\", Ship");
            if (Ships.Count > 1)
                rBuilder.Append('s');
            rBuilder.Append(" = ");

            for (var i = 0; i < Ships.Count; i++)
            {
                if (i > 0)
                    rBuilder.Append(", ");

                rBuilder.Append('\"').Append(Ships[i].Info.Name).Append('\"');
            }

            return rBuilder.GetStringAndRelease();
        }
    }
}
