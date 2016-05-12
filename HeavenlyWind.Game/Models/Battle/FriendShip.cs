using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle
{
    class FriendShip : ModelBase, IParticipant
    {
        public Ship Ship { get; }

        public ShipInfo Info => Ship.Info;
        public bool IsAbyssalShip => false;

        public int Level => Ship.Level;
        public IList<ShipSlot> Slots => Ship.Slots;
        public ShipSlot ExtraSlot => Ship.ExtraSlot;
        public IList<Equipment> EquipedEquipment => Ship.EquipedEquipment;

        public ShipCombatAbility CombatAbility => Ship.CombatAbility;

        bool r_IsMVP;
        public bool IsMVP
        {
            get { return r_IsMVP; }
            internal set
            {
                r_IsMVP = value;
                OnPropertyChanged(nameof(IsMVP));
            }
        }

        bool r_IsDamageControlVisible;
        public bool IsDamageControlVisible
        {
            get { return r_IsDamageControlVisible; }
            internal set
            {
                if (r_IsDamageControlVisible != value)
                {
                    r_IsDamageControlVisible = value;
                    OnPropertyChanged(nameof(IsDamageControlVisible));
                }
            }
        }

        public FriendShip(Ship rpShip)
        {
            Ship = rpShip;
        }
    }
}
