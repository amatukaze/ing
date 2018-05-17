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
                if (r_IsMVP != value)
                {
                    r_IsMVP = value;
                    OnPropertyChanged();
                }
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
        bool r_IsDamageControlConsumed;
        public bool IsDamageControlConsumed
        {
            get { return r_IsDamageControlConsumed; }
            internal set
            {
                r_IsDamageControlConsumed = value;
                OnPropertyChanged(nameof(IsDamageControlConsumed));
            }
        }

        AntiAirCutIn r_AntiAirCutIn;
        public AntiAirCutIn AntiAirCutIn
        {
            get { return r_AntiAirCutIn; }
            internal set
            {
                if (r_AntiAirCutIn != value)
                {
                    r_AntiAirCutIn = value;
                    OnPropertyChanged();
                }
            }
        }

        BattleParticipantState? r_State;
        public BattleParticipantState? State
        {
            get { return r_State; }
            internal set
            {
                if (r_State != value)
                {
                    r_State = value;
                    OnPropertyChanged();
                }
            }
        }

        public FriendShip(Ship rpShip)
        {
            Ship = rpShip;
        }

        public void Reset()
        {
            IsMVP = false;
            AntiAirCutIn = null;
            State = null;
        }
    }
}
