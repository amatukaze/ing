using Sakuno.ING.Game.Models.Knowledge;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public partial class BattleParticipant : BindableObject
    {
        private bool loaded;
        public BattleParticipant(int oneBasedIndex, Ship ship)
        {
            OneBasedIndex = oneBasedIndex;
            Ship = ship;
            FromHP = ToHP = ship.HP;
        }

        public void Load(RawShipInBattle raw)
        {
            IsEscaped = raw.IsEscaped;
            if (!loaded)
                if (FromHP == default)
                    FromHP = ToHP = raw.HP;
            loaded = true;
        }

        public BattleParticipant(int oneBasedIndex, Ship ship, RawShipInBattle raw, bool isEnemy)
        {
            loaded = true;
            OneBasedIndex = oneBasedIndex;
            Ship = ship;
            ToHP = FromHP = raw.HP;
            IsEnemy = isEnemy;
            IsEscaped = raw.IsEscaped;
        }

        public int OneBasedIndex { get; }
        public Ship Ship { get; }
        public bool IsEnemy { get; }
        public ShipHP FromHP { get; private set; }
        public ShipHP ToHP { get; private set; }
        public bool IsSunk => ToHP.Current <= 0;
        public bool Recovored { get; private set; }
        public int DamageGiven { get; private set; }
        public int DamageReceived { get; private set; }
        public bool IsMvp { get; internal set; }

        internal (ShipHP toHP, bool recover) GetDamage(int damage)
        {
            ToHP -= damage;
            DamageReceived += damage;
            if (IsSunk && !IsEnemy)
            {
                EquipmentInfo damageControl = null;
                if (Ship.ExtraEquipment?.Equipment?.Type?.Id == KnownEquipmentType.DamageControl)
                    damageControl = Ship.ExtraEquipment.Equipment;
                else foreach (var slot in Ship.Equipment)
                        if (slot.Equipment?.Type?.Id == KnownEquipmentType.DamageControl)
                        {
                            damageControl = slot.Equipment;
                            break;
                        }

                if (damageControl != null)
                {
                    if (damageControl.Id == 42) //応急修理要員
                        ToHP = ((int)(ToHP.Max * 0.2), ToHP.Max);
                    else if (damageControl.Id == 43) //応急修理女神
                        ToHP = (ToHP.Max, ToHP.Max);
                    Recovored = true;
                    return (ToHP, true);
                }
            }
            return (ToHP, false);
        }

        internal void DoDamage(int damage) => DamageGiven += damage;
        internal void Notify()
        {
            using (EnterBatchNotifyScope())
            {
                NotifyPropertyChanged(nameof(FromHP));
                NotifyPropertyChanged(nameof(ToHP));
                NotifyPropertyChanged(nameof(IsSunk));
                NotifyPropertyChanged(nameof(Recovored));
                NotifyPropertyChanged(nameof(DamageGiven));
                NotifyPropertyChanged(nameof(DamageReceived));
                NotifyPropertyChanged(nameof(IsMvp));
            }
        }
    }
}
