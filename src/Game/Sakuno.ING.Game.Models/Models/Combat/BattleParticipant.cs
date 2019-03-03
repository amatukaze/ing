using Sakuno.ING.Game.Models.Knowledge;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public partial class BattleParticipant : BindableObject
    {
        public BattleParticipant(Ship ship)
        {
            Ship = ship;
            MaxHP = ship.HP.Current;
            FromHP = ToHP = ship.HP.Current;
        }

        public void Load(RawShipInBattle raw) => IsEscaped = raw.IsEscaped;

        public BattleParticipant(Ship ship, RawShipInBattle raw, bool isEnemy)
        {
            Ship = ship;
            FromHP = raw.HP.Current;
            MaxHP = raw.HP.Max;
            ToHP = FromHP;
            IsEnemy = isEnemy;
            IsEscaped = raw.IsEscaped;
        }

        public Ship Ship { get; }
        public bool IsEnemy { get; }
        public int FromHP { get; }
        public int ToHP { get; private set; }
        public int MaxHP { get; }
        public bool Recovored { get; private set; }
        public int DamageGiven { get; private set; }
        public int DamageReceived { get; private set; }
        public bool IsMvp { get; internal set; }

        internal (int toHP, bool recover) GetDamage(int damage)
        {
            ToHP -= damage;
            DamageReceived += damage;
            if (ToHP <= 0 && !IsEnemy)
            {
                EquipmentInfo damageControl = null;
                if (Ship.ExtraEquipment?.Equipment?.Type.Id == KnownEquipmentType.DamageControl)
                    damageControl = Ship.ExtraEquipment.Equipment;
                else foreach (var slot in Ship.Equipment)
                        if (slot.Equipment?.Type.Id == KnownEquipmentType.DamageControl)
                        {
                            damageControl = slot.Equipment;
                            break;
                        }

                if (damageControl != null)
                {
                    if (damageControl.Id == 42) //応急修理要員
                        ToHP = (int)(MaxHP * 0.2);
                    else if (damageControl.Id == 43) //応急修理女神
                        ToHP = MaxHP;
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
                NotifyPropertyChanged(nameof(ToHP));
                NotifyPropertyChanged(nameof(Recovored));
                NotifyPropertyChanged(nameof(DamageGiven));
                NotifyPropertyChanged(nameof(DamageReceived));
                NotifyPropertyChanged(nameof(IsMvp));
            }
        }
    }
}
