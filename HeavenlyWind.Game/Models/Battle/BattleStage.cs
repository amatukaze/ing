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
        public IList<BattleParticipantSnapshot> EnemyMain { get; protected set; }
        public IList<BattleParticipantSnapshot> EnemyEscort { get; protected set; }

        internal BattleParticipantSnapshot[] FriendAndEnemy { get; private set; }

        internal protected BattleStage(BattleInfo rpOwner)
        {
            Owner = rpOwner;
        }

        internal void Process(ApiInfo rpInfo, BattleStage rpFirstStage = null)
        {
            var rData = rpInfo.Data as RawBattleBase;
            var rCombinedFleetData = rData as IRawCombinedFleet;

            FriendAndEnemy = new BattleParticipantSnapshot[rCombinedFleetData == null ? 12 : 24];

            for (var i = 1; i < rData.CurrentHPs.Length; i++)
                if (rData.MaximumHPs[i] != -1)
                    FriendAndEnemy[i - 1] = new BattleParticipantSnapshot(rData.MaximumHPs[i], rData.CurrentHPs[i]);

            FriendMain = FriendAndEnemy.Take(6).TakeWhile(r => r != null).ToArray();
            for (var i = 0; i < FriendMain.Count; i++)
                FriendMain[i].Participant = Owner.Participants.FriendMain[i];

            EnemyMain = FriendAndEnemy.Skip(6).TakeWhile(r => r != null).ToArray();
            for (var i = 0; i < EnemyMain.Count; i++)
                EnemyMain[i].Participant = Owner.Participants.EnemyMain[i];

            if (rCombinedFleetData != null)
            {
                BattleParticipantSnapshot[] rFriendAndEnemyEscort;

                if (rpFirstStage == null)
                    rFriendAndEnemyEscort = rCombinedFleetData.EscortFleetCurrentHPs.Zip(rCombinedFleetData.EscortFleetMaximumHPs,
                        (rpCurrent, rpMaximum) => rpMaximum != -1 ? new BattleParticipantSnapshot(rpMaximum, rpCurrent) : null).Skip(1).ToArray();
                else
                {
                    IEnumerable<BattleParticipantSnapshot> rFriendEscort = rpFirstStage.FriendEscort;
                    if (rFriendEscort == null)
                        rFriendEscort = Enumerable.Repeat<BattleParticipantSnapshot>(null, 6);

                    IEnumerable<BattleParticipantSnapshot> rEnemyEscort = rpFirstStage.EnemyEscort;
                    if (rEnemyEscort == null)
                        rEnemyEscort = Enumerable.Repeat<BattleParticipantSnapshot>(null, 6);

                    rFriendAndEnemyEscort = rFriendEscort.Concat(rEnemyEscort).Select(r => r != null ? new BattleParticipantSnapshot(r.Maximum, r.Current) : null).ToArray();
                }

                if (rFriendAndEnemyEscort[0] != null)
                {
                    FriendEscort = rFriendAndEnemyEscort.Take(6).ToArray();

                    for (var i = 0; i < FriendEscort.Count; i++)
                    {
                        FriendEscort[i].Participant = Owner.Participants.FriendEscort[i];
                        FriendAndEnemy[i + 12] = FriendEscort[i];
                    }
                }

                if (rFriendAndEnemyEscort.Length > 6)
                {
                    EnemyEscort = rFriendAndEnemyEscort.Skip(6).ToArray();

                    for (var i = 0; i < EnemyEscort.Count; i++)
                    {
                        EnemyEscort[i].Participant = Owner.Participants.EnemyEscort[i];
                        FriendAndEnemy[i + 18] = EnemyEscort[i];
                    }
                }
            }

            if (FriendEscort == null)
                Friend = FriendMain;
            else
                Friend = FriendMain.Concat(FriendEscort).ToArray();

            if (EnemyEscort == null)
                Enemy = EnemyMain;
            else
                Enemy = EnemyMain.Concat(EnemyEscort).ToArray();

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
