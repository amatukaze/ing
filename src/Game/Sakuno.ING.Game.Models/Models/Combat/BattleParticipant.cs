using Sakuno.ING.Game.Models.Knowledge;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public class BattleParticipant
    {
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
        public bool IsEscaped { get; }
        public bool Recovored { get; private set; }

        internal (int toHP, bool recover) DoDamage(int damage)
        {
            ToHP -= damage;
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
    }
}
