using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle
{
    public class BattleParticipantSnapshot : ModelBase
    {
        IParticipant r_Participant;
        public IParticipant Participant
        {
            get { return r_Participant; }
            internal set
            {
                r_Participant = value;

                var rFriendShip = value as FriendShip;
                if (rFriendShip != null)
                    IsEvacuated = (rFriendShip.Ship.State & ShipState.Evacuated) != 0;

                r_PreviousState = GetState(Current);
            }
        }

        public int Maximum { get; }
        public int Before { get; }
        int r_Current;
        public int Current
        {
            get { return r_Current; }
            internal set
            {
                r_Current = value;
                ProcessEmergencyRepair();
            }
        }

        public int ToHeavyDamage => r_Current - Maximum / 4;

        public int Damage => Before - Current;
        public int DamageGivenToOpponent { get; internal set; }
        public bool Inaccurate { get; internal set; }

        public bool IsEvacuated { get; internal set; }

        BattleParticipantState r_PreviousState;
        public BattleParticipantState State => GetState(Current);
        public bool IsStateChanged => IsEvacuated || State != r_PreviousState;

        internal BattleParticipantSnapshot(int rpMaximum, int rpCurrent)
        {
            Maximum = rpMaximum;
            Before = r_Current = rpCurrent;
        }

        BattleParticipantState GetState(int rpHP)
        {
            if (Current is -99999 && Maximum is -99999)
                return BattleParticipantState.NotParticipated;

            var rRatio = rpHP / (double)Maximum;

            if (rRatio <= 0.0) return BattleParticipantState.Sunk;
            else if (rRatio <= 0.25) return BattleParticipantState.HeavilyDamaged;
            else if (rRatio <= 0.5) return BattleParticipantState.ModeratelyDamaged;
            else if (rRatio <= 0.75) return BattleParticipantState.LightlyDamaged;
            else return BattleParticipantState.Healthy;
        }

        void ProcessEmergencyRepair()
        {
            var rParticipant = r_Participant as FriendShip;
            if (rParticipant == null || r_Current > 0 || BattleInfo.Current.IsPractice)
                return;

            Equipment rDamageControl = null;

            var rEquipmentInExtraSlot = rParticipant.ExtraSlot?.Equipment;
            if (rEquipmentInExtraSlot?.Info.Type == EquipmentType.DamageControl)
            {
                rDamageControl = rEquipmentInExtraSlot;
                rParticipant.ExtraSlot.Equipment = null;
            }

            if (rDamageControl == null)
                foreach (var slot in rParticipant.Slots)
                    if (slot.Equipment.Info.Type == EquipmentType.DamageControl)
                    {
                        rDamageControl = slot.Equipment;
                        slot.Equipment = null;
                    }

            if (rDamageControl != null)
            {
                switch (rDamageControl.Info.ID)
                {
                    case 42:
                        r_Current = (int)(Maximum * .2);
                        break;

                    case 43:
                        r_Current = Maximum;
                        break;
                }

                rParticipant.Ship.UpdateEquipedEquipment();
                rParticipant.IsDamageControlConsumed = true;
            }
        }

        public void Evacuate()
        {
            IsEvacuated = true;

            var rShip = ((FriendShip)Participant).Ship;
            rShip.State |= ShipState.Evacuated;

            OnPropertyChanged(nameof(IsEvacuated));
            OnPropertyChanged(nameof(IsStateChanged));
        }

        public override string ToString()
        {
            var rBuilder = StringBuilderCache.Acquire();

            if (Participant != null)
                rBuilder.Append(Participant.Info.TranslatedName).Append(" Lv.").Append(Participant.Level).Append(": ");

            rBuilder.Append(Before);
            if (Before != Current)
                rBuilder.Append("->").Append(Current);

            rBuilder.Append('/').Append(Maximum).Append(' ').Append(State);

            return rBuilder.GetStringAndRelease();
        }
    }
}
