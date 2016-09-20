using Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle
{
    public abstract class BattleStage : ModelBase
    {
        public BattleInfo Owner { get; }

        public abstract BattleStageType Type { get; }

        public abstract IList<BattlePhase> Phases { get; }

        public IList<BattleParticipantSnapshot> Friend { get; protected set; }
        public IList<BattleParticipantSnapshot> FriendMain { get; protected set; }
        public IList<BattleParticipantSnapshot> FriendEscort { get; protected set; }

        public IList<BattleParticipantSnapshot> Enemy { get; protected set; }

        internal BattleParticipantSnapshot[] FriendAndEnemy { get; private set; }

        internal protected BattleStage(BattleInfo rpOwner)
        {
            Owner = rpOwner;
        }

        internal void Process(ApiInfo rpInfo)
        {
            var rData = rpInfo.Data as RawBattleBase;
            var rCombinedFleetData = rData as IRawCombinedFleet;

            FriendAndEnemy = new BattleParticipantSnapshot[rCombinedFleetData == null ? 12 : 18];

            for (var i = 1; i < rData.CurrentHPs.Length; i++)
                if (rData.MaximumHPs[i] != -1)
                    FriendAndEnemy[i - 1] = new BattleParticipantSnapshot(rData.MaximumHPs[i], rData.CurrentHPs[i]);

            FriendMain = FriendAndEnemy.Take(6).TakeWhile(r => r != null).ToArray();
            for (var i = 0; i < FriendMain.Count; i++)
                FriendMain[i].Participant = Owner.Participants.FriendMain[i];

            Enemy = FriendAndEnemy.Skip(6).TakeWhile(r => r != null).ToArray();
            for (var i = 0; i < Enemy.Count; i++)
                Enemy[i].Participant = Owner.Participants.Enemy[i];

            if (rCombinedFleetData != null)
            {
                FriendEscort = rCombinedFleetData.EscortFleetCurrentHPs.Zip(rCombinedFleetData.EscortFleetMaximumHPs,
                    (rpCurrent, rpMaximum) => rpMaximum != -1 ? new BattleParticipantSnapshot(rpMaximum, rpCurrent) : null).Skip(1).ToArray();

                for (var i = 0; i < FriendEscort.Count; i++)
                {
                    FriendEscort[i].Participant = Owner.Participants.FriendEscort[i];
                    FriendAndEnemy[i + 12] = FriendEscort[i];
                }
            }

            if (FriendEscort == null)
                Friend = FriendMain;
            else
                Friend = FriendMain.Concat(FriendEscort).ToArray();

            foreach (var rPhase in Phases)
                rPhase.Process();

            if (!Owner.IsPractice)
                foreach (var rSnapshot in Friend)
                {
                    var rParticipant = (FriendShip)rSnapshot.Participant;
                    if (rSnapshot.State == BattleParticipantState.HeavilyDamaged && rParticipant.EquipedEquipment.Any(r => r.Info.Type == EquipmentType.DamageControl))
                    {
                        rParticipant.IsDamageControlConsumed = false;
                        rParticipant.IsDamageControlVisible = true;
                    }
                }
        }
        internal void ProcessMVP()
        {
            foreach (var rSnapshot in Friend)
                ((FriendShip)rSnapshot.Participant).IsMVP = false;

            var rMaxDamage = FriendMain.Max(r => r.DamageGivenToOpponent);
            ((FriendShip)FriendMain.First(r => r.DamageGivenToOpponent == rMaxDamage).Participant).IsMVP = true;

            if (FriendEscort != null)
            {
                rMaxDamage = FriendEscort.Max(r => r.DamageGivenToOpponent);
                ((FriendShip)FriendEscort.First(r => r.DamageGivenToOpponent == rMaxDamage).Participant).IsMVP = true;
            }
        }
    }
}
