using Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System;
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

            FriendAndEnemy = new BattleParticipantSnapshot[rCombinedFleetData == null ? 14 : 28];

            FriendMain = new BattleParticipantSnapshot[rData.FriendCurrentHPs.Length];
            for (var i = 0; i < rData.FriendCurrentHPs.Length; i++)
            {
                FriendMain[i] = new BattleParticipantSnapshot(rData.FriendMaximumHPs[i], rData.FriendCurrentHPs[i])
                {
                    Participant = Owner.Participants.FriendMain[i],
                };

                FriendAndEnemy[i] = FriendMain[i];
            }

            EnemyMain = new BattleParticipantSnapshot[rData.EnemyCurrentHPs.Length];
            for (var i = 0; i < rData.EnemyCurrentHPs.Length; i++)
            {
                EnemyMain[i] = new BattleParticipantSnapshot(rData.EnemyMaximumHPs[i], rData.EnemyCurrentHPs[i])
                {
                    Participant = Owner.Participants.EnemyMain[i],
                };

                FriendAndEnemy[i + 7] = EnemyMain[i];
            }

            if (rCombinedFleetData != null)
            {
                if (rCombinedFleetData.FriendEscortCurrentHPs != null)
                {
                    FriendEscort = new BattleParticipantSnapshot[rCombinedFleetData.FriendEscortCurrentHPs.Length];
                    for (var i = 0; i < rData.FriendCurrentHPs.Length; i++)
                    {
                        FriendEscort[i] = new BattleParticipantSnapshot(rCombinedFleetData.FriendEscortMaximumHPs[i], rCombinedFleetData.FriendEscortCurrentHPs[i])
                        {
                            Participant = Owner.Participants.FriendEscort[i],
                        };

                        FriendAndEnemy[i + 14] = FriendEscort[i];
                    }
                }

                if (rCombinedFleetData.EnemyEscortCurrentHPs != null)
                {
                    EnemyEscort = new BattleParticipantSnapshot[rCombinedFleetData.EnemyEscortCurrentHPs.Length];
                    for (var i = 0; i < rData.FriendCurrentHPs.Length; i++)
                    {
                        EnemyEscort[i] = new BattleParticipantSnapshot(rCombinedFleetData.EnemyEscortMaximumHPs[i], rCombinedFleetData.EnemyEscortCurrentHPs[i])
                        {
                            Participant = Owner.Participants.EnemyEscort[i],
                        };

                        FriendAndEnemy[i + 21] = EnemyEscort[i];
                    }
                }

                //if ((rpFirstStage == null && rFriendAndEnemyEscort.Length > 6) || (rpFirstStage != null && rpFirstStage.EnemyEscort != null))
                //{
                //    EnemyEscort = rFriendAndEnemyEscort.Skip(6).TakeWhile(r => r != null).ToArray();

                //    for (var i = 0; i < EnemyEscort.Count; i++)
                //    {
                //        EnemyEscort[i].Participant = Owner.Participants.EnemyEscort[i];
                //        FriendAndEnemy[i + 18] = EnemyEscort[i];
                //    }
                //}
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
                try
                {
                    rPhase.Process();
                }
                catch (Exception e)
                {
                    Logger.Write(LoggingLevel.Error, e.Message);
                    ApiParserManager.HandleException(rpInfo.Session, e);
                }

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
        internal void Postprocess()
        {
            foreach (var rSnapshot in Friend)
            {
                var rParticipant = (FriendShip)rSnapshot.Participant;

                rParticipant.IsMVP = false;
                rParticipant.State = rSnapshot.State;
            }

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
