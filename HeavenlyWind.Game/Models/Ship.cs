using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class Ship : RawDataWrapper<RawShip>, IID, ICombatAbility
    {
        public int ID => RawData.ID;
        public ShipInfo Info { get; private set; }
        public int SortNumber => RawData.SortNumber;

        public int Level => RawData.Level;
        public bool IsMarried => Level > 99;

        int r_Condition;
        public int Condition
        {
            get { return r_Condition; }
            internal set
            {
                if (r_Condition != value)
                {
                    r_Condition = value;
                    OnPropertyChanged(nameof(Condition));
                    OnPropertyChanged(nameof(ConditionType));
                }
            }
        }
        public ShipConditionType ConditionType
        {
            get
            {
                if (Condition >= 50) return ShipConditionType.HighMorale;
                if (Condition >= 40) return ShipConditionType.Normal;
                if (Condition >= 30) return ShipConditionType.SightlyTired;
                if (Condition >= 20) return ShipConditionType.ModerateTired;
                return ShipConditionType.SeriouslyTired;
            }
        }

        public bool IsLocked => RawData.IsLocked;

        public ShipLocking LockingTag => ShipLockingService.Instance?.GetLocking(RawData.LockingTag);

        public int Experience => RawData.Experience[0];
        public int ExperienceToNextLevel => RawData.Experience[1];
        public int ExperienceToMarriage => ExperienceTable.GetShipExperienceToLevel(99, Experience);
        public int ExperienceToMaxLevel => ExperienceTable.GetShipExperienceToLevel(150, Experience);

        public TimeSpan? RepairTime => RawData.RepairTime == 0 ? (TimeSpan?)null : TimeSpan.FromMilliseconds(RawData.RepairTime);
        public int RepairFuelConsumption => RawData.RepairConsumption[0];
        public int RepairSteelConsumption => RawData.RepairConsumption[1];

        public Fleet OwnerFleet { get; internal set; }

        RepairDock r_OwnerRepairDock;
        public RepairDock OwnerRepairDock
        {
            get { return r_OwnerRepairDock; }
            internal set
            {
                if (r_OwnerRepairDock != value)
                {
                    r_OwnerRepairDock = value;
                    OnPropertyChanged(nameof(OwnerRepairDock));
                }
            }
        }

        ClampedValue r_HP;
        public ClampedValue HP
        {
            get { return r_HP; }
            internal set
            {
                if (r_HP != value)
                {
                    r_HP = value;
                    OnPropertyChanged(nameof(HP));

                    if (r_HP.Current / (double)r_HP.Maximum <= .25)
                        State |= ShipState.HeavilyDamaged;
                    else
                        State &= ~ShipState.HeavilyDamaged;
                }
            }
        }

        ClampedValue r_Fuel;
        public ClampedValue Fuel
        {
            get { return r_Fuel; }
            internal set
            {
                if (r_Fuel != value)
                {
                    r_Fuel = value;
                    OnPropertyChanged(nameof(Fuel));
                }
            }
        }
        ClampedValue r_Bullet;
        public ClampedValue Bullet
        {
            get { return r_Bullet; }
            internal set
            {
                if (r_Bullet != value)
                {
                    r_Bullet = value;
                    OnPropertyChanged(nameof(Bullet));
                }
            }
        }

        ShipState r_State;
        public ShipState State
        {
            get { return r_State; }
            internal set
            {
                if (r_State != value)
                {
                    r_State = value;
                    OnPropertyChanged(nameof(State));
                }
            }
        }

        public ShipStatus Status { get; }

        public ShipCombatAbility CombatAbility { get; }

        public bool CanParticipantInAnchorageRepair
        {
            get
            {
                var rPort = KanColleGame.Current.Port;
                return HP.Current != HP.Maximum && HP.Current / (double)HP.Maximum > .5 &&
                    !rPort.RepairDocks.Values.Any(r => r.Ship == this) &&
                    rPort.Materials.Fuel >= RepairFuelConsumption && rPort.Materials.Steel >= RepairSteelConsumption;
            }
        }
        public ShipAnchorageRepairStatus AnchorageRepairStatus { get; private set; }

        #region Equipment

        int[] r_EquipmentIDs;
        ReadOnlyCollection<ShipSlot> r_Slots;
        public ReadOnlyCollection<ShipSlot> Slots
        {
            get { return r_Slots; }
            private set
            {
                if (r_Slots != value)
                {
                    r_Slots = value;
                    OnPropertyChanged(nameof(Slots));
                }
            }
        }
        int r_ExtraEquipmentID;
        ShipSlot r_ExtraSlot;
        public ShipSlot ExtraSlot
        {
            get { return r_ExtraSlot; }
            private set
            {
                if (r_ExtraSlot != value)
                {
                    r_ExtraSlot = value;
                    OnPropertyChanged(nameof(ExtraSlot));
                }
            }
        }

        ReadOnlyCollection<Equipment> r_EquipedEquipment;
        public ReadOnlyCollection<Equipment> EquipedEquipment
        {
            get { return r_EquipedEquipment; }
            private set
            {
                if (r_EquipedEquipment != value)
                {
                    r_EquipedEquipment = value;
                    OnPropertyChanged(nameof(EquipedEquipment));
                }
            }
        }

        #endregion

        internal Ship(RawShip rpRawData) : base(rpRawData)
        {
            Status = new ShipStatus(this);
            CombatAbility = new ShipCombatAbility(this);

            OnRawDataUpdated();
        }

        protected override void OnRawDataUpdated()
        {
            ShipInfo rInfo;
            if (!KanColleGame.Current.MasterInfo.Ships.TryGetValue(RawData.ShipID, out rInfo))
                Info = ShipInfo.Dummy;
            else
            {
                if (Info?.ID != RawData.ShipID)
                    r_EquipmentIDs = null;

                Info = rInfo;
                OnPropertyChanged(nameof(Info));
            }

            HP = new ClampedValue(RawData.HPMaximum, RawData.HPCurrent);
            Fuel = new ClampedValue(Info.MaxFuelConsumption, RawData.Fuel);
            Bullet = new ClampedValue(Info.MaxBulletConsumption, RawData.Bullet);

            Condition = RawData.Condition;

            var rPort = KanColleGame.Current.Port;

            if (KanColleGame.Current.Sortie != null && rPort.EvacuatedShipIDs.Contains(ID))
                State |= ShipState.Evacuated;
            else
                State &= ~ShipState.Evacuated;

            if (rPort.RepairDocks.Values.Any(r => r.Ship == this))
                State |= ShipState.Repairing;
            else
            {
                State &= ~ShipState.Repairing;

                var rShips = OwnerFleet?.AnchorageRepair.RepairingShips;
                UpdateAnchorageRepairStatus(rShips != null && rShips.Any(r => r.Item1 == this));
            }

            if (RawData.ModernizedStatus?.Length >= 5)
                Status.Update(Info, RawData);

            if (RawData.Equipment != null)
                UpdateSlots();

            CombatAbility.Update();

            OnPropertyChanged(nameof(Level));
            OnPropertyChanged(nameof(ExperienceToNextLevel));
        }

        void UpdateSlots()
        {
            var rUpdateList = false;

            if (Slots == null || Slots.Count != RawData.SlotCount)
                Slots = Enumerable.Range(0, RawData.SlotCount).Select((r, i) => new ShipSlot(null, RawData.PlaneCountInSlot[i], Info.PlaneCountInSlot[i])).ToList().AsReadOnly();

            if (r_EquipmentIDs == null || !r_EquipmentIDs.SequenceEqual(RawData.Equipment))
            {
                r_EquipmentIDs = RawData.Equipment;

                for (var i = 0; i < Slots.Count; i++)
                {
                    Equipment rEquipment;
                    var rID = r_EquipmentIDs[i];
                    if (rID == -1)
                        rEquipment = Equipment.Dummy;
                    else if (!KanColleGame.Current.Port.Equipment.TryGetValue(rID, out rEquipment))
                    {
                        rEquipment = new Equipment(new RawEquipment() { ID = rID, EquipmentID = -1 });
                        KanColleGame.Current.Port.Equipment.Add(rEquipment);
                    }

                    Slots[i].Equipment = rEquipment;
                }

                rUpdateList = true;
            }

            for (var i = 0; i < Slots.Count; i++)
                Slots[i].PlaneCount = RawData.PlaneCountInSlot[i];

            if (RawData.ExtraEquipment != 0)
            {
                if (ExtraSlot == null)
                    ExtraSlot = new ShipSlot(0, 0);

                if (r_ExtraEquipmentID != RawData.ExtraEquipment)
                {
                    r_ExtraEquipmentID = RawData.ExtraEquipment;
                    ExtraSlot.Equipment = r_ExtraEquipmentID == -1 ? Equipment.Dummy : KanColleGame.Current.Port.Equipment[r_ExtraEquipmentID];

                    rUpdateList = true;
                }
            }

            if (rUpdateList)
            {
                var rList = Slots.Where(r => r.HasEquipment).Select(r => r.Equipment);
                if (ExtraSlot != null && ExtraSlot.HasEquipment)
                    rList = rList.Concat(new[] { ExtraSlot.Equipment });

                EquipedEquipment = rList.ToArray().AsReadOnly();
                rUpdateList = false;
            }
        }

        internal void Repair(bool rpInstantRepair)
        {
            if (!rpInstantRepair)
                State |= ShipState.Repairing;
            else
            {
                State &= ~ShipState.Repairing;

                HP = HP.Update(HP.Maximum);
                if (Condition < 40)
                    Condition = 40;
            }
        }

        internal void UpdateEquipmentIDs(int[] rpEquipmentIDs)
        {
            RawData.Equipment = rpEquipmentIDs;
            UpdateSlots();
        }

        internal void InstallReinforcementExpansion()
        {
            r_ExtraEquipmentID = -1;
            ExtraSlot = new ShipSlot(Equipment.Dummy, 0, 0);
        }

        internal void UpdateAnchorageRepairStatus(bool rpRepairing)
        {
            if (rpRepairing)
            {
                State |= ShipState.RepairingInAnchorage;

                if (AnchorageRepairStatus == null)
                {
                    AnchorageRepairStatus = new ShipAnchorageRepairStatus(this);
                    OnPropertyChanged(nameof(AnchorageRepairStatus));
                }
            }
            else
            {
                State &= ~ShipState.RepairingInAnchorage;

                if (AnchorageRepairStatus != null)
                {
                    AnchorageRepairStatus.Dispose();
                    AnchorageRepairStatus = null;
                    OnPropertyChanged(nameof(AnchorageRepairStatus));
                }
            }
        }

        public override string ToString() => $"ID = {ID}, Name = \"{Info.Name}\", Type = \"{Info.Type.Name}\", Level = {Level}";
    }
}
