using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class Ship : RawDataWrapper<RawShip>, IID, ICombatAbility
    {
        static Port r_Port = KanColleGame.Current.Port;

        public const int LevelToMarriage = 99;

        public int ID => RawData.ID;
        public ShipInfo Info { get; private set; }
        public int SortNumber => RawData.SortNumber;

        public int Level => RawData.Level;
        public bool IsMarried => Level > LevelToMarriage;

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

                    if (r_Condition >= 50)
                        ConditionType = ShipConditionType.HighMorale;
                    else if (r_Condition >= 40)
                        ConditionType = ShipConditionType.Normal;
                    else if (r_Condition >= 30)
                        ConditionType = ShipConditionType.SightlyTired;
                    else if (r_Condition >= 20)
                        ConditionType = ShipConditionType.ModerateTired;
                    else
                        ConditionType = ShipConditionType.SeriouslyTired;
                }
            }
        }

        ShipConditionType r_ConditionType;
        public ShipConditionType ConditionType
        {
            get { return r_ConditionType; }
            private set
            {
                if (r_ConditionType != value)
                {
                    r_ConditionType = value;
                    OnPropertyChanged(nameof(ConditionType));
                }
            }
        }

        bool r_IsLocked;
        public bool IsLocked
        {
            get { return r_IsLocked; }
            internal set
            {
                if (r_IsLocked != value)
                {
                    r_IsLocked = value;
                    OnPropertyChanged(nameof(IsLocked));
                }
            }
        }

        public ShipLocking LockingTag => ShipLockingService.Instance?.GetLocking(RawData.LockingTag);

        public int Experience => RawData.Experience[0];
        public int ExperienceToNextLevel => RawData.Experience[1];
        public int ExperienceToMarriage => ExperienceTable.GetShipExperienceToLevel(LevelToMarriage, Experience);
        public int ExperienceToMaxLevel => ExperienceTable.GetShipExperienceToLevel(ExperienceTable.Ship.Count, Experience);

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

        public ClampedValue HP { get; }

        ShipDamageState r_DamageState;
        public ShipDamageState DamageState
        {
            get { return r_DamageState; }
            private set
            {
                if (r_DamageState != value)
                {
                    r_DamageState = value;
                    OnPropertyChanged(nameof(DamageState));
                }
            }
        }

        public ClampedValue Fuel { get; }
        public ClampedValue Bullet { get; }

        ShipSpeed r_Speed;
        public ShipSpeed Speed
        {
            get { return r_Speed; }
            internal set
            {
                if (r_Speed != value)
                {
                    r_Speed = value;
                    OnPropertyChanged(nameof(Speed));
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

        public bool CanParticipantInAnchorageRepair =>
            r_DamageState != ShipDamageState.FullyHealthy && r_DamageState < ShipDamageState.ModeratelyDamaged &&
            !r_Port.RepairDocks.Values.Any(r => r.Ship == this) &&
            r_Port.Materials.Fuel >= RepairFuelConsumption && r_Port.Materials.Steel >= RepairSteelConsumption;

        ShipAnchorageRepairStatus r_AnchorageRepairStatus;
        public ShipAnchorageRepairStatus AnchorageRepairStatus
        {
            get { return r_AnchorageRepairStatus; }
            private set
            {
                if (r_AnchorageRepairStatus != value)
                {
                    r_AnchorageRepairStatus = value;
                    OnPropertyChanged(nameof(AnchorageRepairStatus));
                }
            }
        }

        #region Equipment

        int[] r_EquipmentIDs;
        IList<ShipSlot> r_Slots;
        public IList<ShipSlot> Slots
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

        IList<Equipment> r_EquipedEquipment;
        public IList<Equipment> EquipedEquipment
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
            HP = new ShipHP(this, 1, 1);
            Fuel = new ClampedValue(1, 1);
            Bullet = new ClampedValue(1, 1);

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
                {
                    r_EquipmentIDs = null;
                    r_Slots = null;
                }

                Info = rInfo;
                OnPropertyChanged(nameof(Info));
            }

            HP.Set(RawData.HPMaximum, RawData.HPCurrent);
            Fuel.Set(Info.MaxFuelConsumption, RawData.Fuel);
            Bullet.Set(Info.MaxBulletConsumption, RawData.Bullet);

            Speed = RawData.Speed;

            Condition = RawData.Condition;

            if (KanColleGame.Current.Sortie != null && r_Port.EvacuatedShipIDs.Contains(ID))
                State |= ShipState.Evacuated;
            else
                State &= ~ShipState.Evacuated;

            if (r_Port.RepairDocks.Values.Any(r => r.Ship == this))
                State |= ShipState.Repairing;
            else
            {
                State &= ~ShipState.Repairing;

                UpdateAnchorageRepairStatus(OwnerFleet?.AnchorageRepair?.IsBeingAnchorageRepair(this));
            }

            if (RawData.ModernizedStatus?.Length >= 5)
                Status.Update(Info, RawData);

            if (RawData.Equipment != null)
                UpdateSlots();

            CombatAbility.Update();

            IsLocked = RawData.IsLocked;

            OnPropertyChanged(nameof(Level));
            OnPropertyChanged(nameof(ExperienceToNextLevel));
            OnPropertyChanged(nameof(Status));
        }

        void UpdateSlots()
        {
            var rUpdateList = false;

            if (Slots == null || Slots.Count != RawData.SlotCount)
                Slots = Enumerable.Range(0, RawData.SlotCount).Select(r => new ShipSlot(Info.PlaneCountInSlot[r], RawData.PlaneCountInSlot[r])).ToArray();

            if (r_EquipmentIDs == null || !r_EquipmentIDs.SequenceEqual(RawData.Equipment))
            {
                r_EquipmentIDs = RawData.Equipment;

                for (var i = 0; i < Slots.Count; i++)
                {
                    Equipment rEquipment;
                    var rID = r_EquipmentIDs[i];
                    if (rID == -1)
                        rEquipment = Equipment.Dummy;
                    else if (!r_Port.Equipment.TryGetValue(rID, out rEquipment))
                    {
                        rEquipment = new Equipment(new RawEquipment() { ID = rID, EquipmentID = -1 });
                        r_Port.AddEquipment(rEquipment);
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
                    ExtraSlot.Equipment = r_ExtraEquipmentID == -1 ? Equipment.Dummy : r_Port.Equipment[r_ExtraEquipmentID];

                    rUpdateList = true;
                }
            }

            if (rUpdateList)
            {
                var rList = Slots.Where(r => r.HasEquipment).Select(r => r.Equipment);
                if (ExtraSlot != null && ExtraSlot.HasEquipment)
                    rList = rList.Concat(new[] { ExtraSlot.Equipment });

                EquipedEquipment = rList.ToArray();
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

                HP.Current = HP.Maximum;

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

        internal void UpdateAnchorageRepairStatus(bool? rpRepairing)
        {
            if (rpRepairing.GetValueOrDefault())
            {
                State |= ShipState.RepairingInAnchorage;

                if (AnchorageRepairStatus == null)
                    AnchorageRepairStatus = new ShipAnchorageRepairStatus(this);
            }
            else
            {
                State &= ~ShipState.RepairingInAnchorage;

                AnchorageRepairStatus?.Dispose();
                AnchorageRepairStatus = null;
            }
        }

        public override string ToString() => $"ID = {ID}, Name = \"{Info.Name}\", Type = \"{Info.Type.Name}\", Level = {Level}";

        class ShipHP : ClampedValue
        {
            Ship r_Owner;

            public ShipHP(Ship rpOwner, int rpMaximum, int rpCurrent) : base(rpMaximum, rpCurrent)
            {
                r_Owner = rpOwner;
            }

            public override void Set(int rpMaximum, int rpCurrent)
            {
                base.Set(rpMaximum, rpCurrent);

                var rRate = Current / (double)Maximum;

                if (rRate <= .25)
                {
                    r_Owner.State |= ShipState.HeavilyDamaged;
                    r_Owner.DamageState = ShipDamageState.HeavilyDamaged;
                }
                else
                {
                    r_Owner.State &= ~ShipState.HeavilyDamaged;

                    if (rRate <= .5)
                        r_Owner.DamageState = ShipDamageState.ModeratelyDamaged;
                    else if (rRate <= .75)
                        r_Owner.DamageState = ShipDamageState.LightlyDamaged;
                    else if (rRate < 1.0)
                        r_Owner.DamageState = ShipDamageState.Healthy;
                    else
                        r_Owner.DamageState = ShipDamageState.FullyHealthy;
                }
            }
        }
    }
}
