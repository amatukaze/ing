using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class Ship : RawDataWrapper<RawShip>, IID
    {
        public static Ship Dummy { get; } = new Ship(new RawShip() { ShipID = -1 });

        public int ID => RawData.ID;
        public ShipInfo Info { get; private set; }
        public int SortNumber => RawData.SortNumber;

        public int Level => RawData.Level;
        
        public int Condition => RawData.Condition;
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

        public int LockingTag => RawData.LockingTag;

        public int Experience => RawData.Experience[0];
        public int ExperienceToNextLevel => RawData.Experience[1];
        public int ExperienceToMarriage => ExperienceTable.GetShipExperienceToLevel(99, Experience);
        public int ExperienceToMaxLevel => ExperienceTable.GetShipExperienceToLevel(150, Experience);

        public TimeSpan? RepairTime => RawData.RepairTime == 0 ? (TimeSpan?)null : TimeSpan.FromMilliseconds(RawData.RepairTime);

        public Fleet OwnerFleet { get; internal set; }

        ClampedValue r_HP;
        public ClampedValue HP
        {
            get { return r_HP; }
            private set
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

        #region Equipments

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

        ReadOnlyCollection<Equipment> r_Equipments;
        public ReadOnlyCollection<Equipment> Equipments
        {
            get { return r_Equipments; }
            private set
            {
                if (r_Equipments != value)
                {
                    r_Equipments = value;
                    OnPropertyChanged(nameof(Equipments));
                }
            }
        }

        #endregion

        internal Ship(RawShip rpRawData) : base(rpRawData)
        {
            Status = new ShipStatus(this);
            OnRawDataUpdated();
        }

        protected override void OnRawDataUpdated()
        {
            ShipInfo rInfo;
            if (KanColleGame.Current.MasterInfo.Ships.TryGetValue(RawData.ShipID, out rInfo))
                Info = rInfo;
            else
                Info = ShipInfo.Dummy;

            HP = new ClampedValue(RawData.HPMaximum, RawData.HPCurrent);
            Fuel = new ClampedValue(Info.MaxFuelConsumption, RawData.Fuel);
            Bullet = new ClampedValue(Info.MaxBulletConsumption, RawData.Bullet);

            if (RawData.ModernizedStatus?.Length >= 5)
                Status.Update(Info, RawData);

            if (RawData.Equipments != null)
                UpdateSlots();

            OnPropertyChanged(nameof(Level));
            OnPropertyChanged(nameof(ExperienceToNextLevel));
            OnPropertyChanged(nameof(Condition));
        }

        void UpdateSlots()
        {
            var rUpdateList = false;

            if (r_EquipmentIDs == null || !r_EquipmentIDs.SequenceEqual(RawData.Equipments))
            {
                r_EquipmentIDs = RawData.Equipments;
                Slots = RawData.Equipments.Take(RawData.EquipmentCount)
                    .Zip(RawData.PlaneCountInSlot.Zip(Info.PlaneCountInSlot, (rpCount, rpMaxCount) => new { Count = rpCount, MaxCount = rpMaxCount }),
                        (rpID, rpPlane) => 
                        {
                            Equipment rEquipment;
                            if (!KanColleGame.Current.Port.Equipments.TryGetValue(rpID, out rEquipment))
                                KanColleGame.Current.Port.Equipments.Add(rEquipment = new Equipment(new RawEquipment() { ID = rpID, EquipmentID = -1 }));
                            else if (rpID == -1)
                                rEquipment = Equipment.Dummy;

                            return new ShipSlot(rEquipment, rpPlane.MaxCount, rpPlane.Count);
                        }).ToArray().AsReadOnly();

                rUpdateList = true;
            }
            for (var i = 0; i < Slots.Count; i++)
                Slots[i].PlaneCount = RawData.PlaneCountInSlot[i];

            if (RawData.ExtraEquipment != 0 && r_ExtraEquipmentID != RawData.ExtraEquipment)
            {
                r_ExtraEquipmentID = RawData.ExtraEquipment;
                ExtraSlot = new ShipSlot(r_ExtraEquipmentID != -1 ? KanColleGame.Current.Port.Equipments[RawData.ExtraEquipment] : Equipment.Dummy, 0, 0);

                rUpdateList = true;
            }

            if (rUpdateList)
            {
                var rList = Slots.Where(r => r.HasEquipment).Select(r => r.Equipment);
                if (ExtraSlot != null)
                    rList = rList.Concat(new[] { ExtraSlot.Equipment });

                Equipments = rList.ToArray().AsReadOnly();
                rUpdateList = false;
            }
        }

        public override string ToString() => $"ID = {ID}, Name = \"{Info.Name}\", Type = \"{Info.Type.Name}\", Level = {Level}";
    }
}
