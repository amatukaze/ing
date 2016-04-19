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
            }
        }

        public int Maximum { get; }
        public int Before { get; }
        public int Current { get; internal set; }

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
            Before = Current = rpCurrent;

            r_PreviousState = GetState(rpCurrent);
        }

        BattleParticipantState GetState(int rpHP)
        {
            if (IsEvacuated)
                return BattleParticipantState.Evacuated;

            var rRatio = rpHP / (double)Maximum;

            if (Participant?.Info?.Speed == ShipSpeed.None)
                if (rRatio <= 0.0) return BattleParticipantState.Demolished;
                else if (rRatio <= 0.25) return BattleParticipantState.Destroyed;
                else if (rRatio <= 0.5) return BattleParticipantState.Damaged;
                else if (rRatio <= 0.75) return BattleParticipantState.Disordered;
                else return BattleParticipantState.Healthy;

            if (rRatio <= 0.0) return BattleParticipantState.Sunk;
            else if (rRatio <= 0.25) return BattleParticipantState.HeavilyDamaged;
            else if (rRatio <= 0.5) return BattleParticipantState.ModeratelyDamaged;
            else if (rRatio <= 0.75) return BattleParticipantState.LightlyDamaged;
            else return BattleParticipantState.Healthy;
        }

        public override string ToString()
        {
            var rBuilder = new StringBuilder(32);

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
