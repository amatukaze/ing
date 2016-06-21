using System.Linq;
using System.Text;

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
                    IsEvacuated = rFriendShip.Ship.State.HasFlag(ShipState.Evacuated);

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
        public bool IsStateChanged => State != r_PreviousState;

        internal BattleParticipantSnapshot(int rpMaximum, int rpCurrent)
        {
            Maximum = rpMaximum;
            Before = r_Current = rpCurrent;
        }

        BattleParticipantState GetState(int rpHP)
        {
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

            EquipmentInfo rDamageControl = null;

            var rEquipmentInExtraSlot = rParticipant.ExtraSlot?.Equipment?.Info;
            if (rEquipmentInExtraSlot?.Type == EquipmentType.DamageControl)
                rDamageControl = rEquipmentInExtraSlot;

            if (rDamageControl == null)
                rDamageControl = rParticipant.EquipedEquipment.FirstOrDefault(r => r.Info.Type == EquipmentType.DamageControl)?.Info;

            if (rDamageControl != null)
            {
                switch (rDamageControl.ID)
                {
                    case 42:
                        r_Current = (int)(Maximum * .2);
                        break;

                    case 43:
                        r_Current = Maximum;
                        break;
                }

                rParticipant.IsDamageControlConsumed = true;
            }
        }

        public override string ToString()
        {
            var rBuilder = new StringBuilder(32);

            if (Participant != null)
                rBuilder.Append($"{Participant.Info.TranslatedName} Lv.{Participant.Level}: ");

            rBuilder.Append(Before);
            if (Before != Current)
                rBuilder.Append("->").Append(Current);

            rBuilder.Append('/').Append(Maximum);

            rBuilder.Append(' ').Append(State);

            return rBuilder.ToString();
        }
    }
}
