using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases
{
    class AerialCombatPhase : BattlePhase<RawAerialCombatPhase>
    {
        public PhaseRound Round { get; }

        internal AerialCombatPhase(BattleStage rpStage, RawAerialCombatPhase rpRawData, PhaseRound rpRound = PhaseRound.First) : base(rpStage, rpRawData)
        {
            Round = rpRound;
        }

        internal protected override void Process()
        {
            if (RawData == null)
                return;

            var rInfo = Stage.Owner.AerialCombat;

            ProcessStage1(rInfo);
            ProcessStage2(rInfo);
            ProcessStage3();
        }

        void ProcessStage1(AerialCombat rpInfo)
        {
            var rStage1 = RawData.Stage1;
            if (rStage1 != null)
            {
                rpInfo.Result = rStage1.Result;

                if (Round == PhaseRound.First)
                    rpInfo.Stage1 = new AerialCombat.Stage()
                    {
                        FriendPlaneCount = rStage1.FriendPlaneCount,
                        FriendPlaneRemaningCount = rStage1.FriendPlaneCount - rStage1.FriendPlaneLostCount,

                        EnemyPlaneCount = rStage1.EnemyPlaneCount,
                        EnemyPlaneRemaningCount = rStage1.EnemyPlaneCount - rStage1.EnemyPlaneLostCount,
                    };
                else
                {
                    rpInfo.Stage1.FriendPlaneRemaningCount -= rStage1.FriendPlaneLostCount;
                    rpInfo.Stage1.EnemyPlaneRemaningCount -= rStage1.EnemyPlaneLostCount;
                }
            }
        }

        void ProcessStage2(AerialCombat rpInfo)
        {
            var rStage2 = RawData.Stage2;
            if (rStage2 != null)
            {
                if (Round == PhaseRound.First)
                    rpInfo.Stage2 = new AerialCombat.Stage()
                    {
                        FriendPlaneCount = rStage2.FriendPlaneCount,
                        FriendPlaneRemaningCount = rStage2.FriendPlaneCount - rStage2.FriendPlaneLostCount,

                        EnemyPlaneCount = rStage2.EnemyPlaneCount,
                        EnemyPlaneRemaningCount = rStage2.EnemyPlaneCount - rStage2.EnemyPlaneLostCount,
                    };
                else
                {
                    rpInfo.Stage2.FriendPlaneRemaningCount -= rStage2.FriendPlaneLostCount;
                    rpInfo.Stage2.EnemyPlaneRemaningCount -= rStage2.EnemyPlaneLostCount;
                }
            }
        }

        void ProcessStage3()
        {
            var rStage3 = RawData.Stage3;
            if (rStage3 == null)
                return;

            var rParticipants = Stage.FriendAndEnemy;

            var rFriendMainDamages = rStage3.FriendDamage.Skip(1);
            var rEnemyMainDamages = rStage3.EnemyDamage.Skip(1);
            var rDamages = Enumerable.Concat(rFriendMainDamages, rEnemyMainDamages).ToArray();

            IEnumerable<int> rEnemyEscortDamages = null;
            if (RawData.Stage3CombinedFleet != null)
            {
                var rFriendEscortDamages = RawData.Stage3CombinedFleet.FriendDamage.Skip(1);
                var rEscortDamages = Enumerable.Concat(rDamages, rFriendEscortDamages);

                if (RawData.Stage3CombinedFleet.EnemyDamage != null)
                {
                    rEnemyEscortDamages = RawData.Stage3CombinedFleet.EnemyDamage.Skip(1);

                    rEscortDamages = rEscortDamages.Concat(rEnemyEscortDamages);
                }

                rDamages = rEscortDamages.ToArray();
            }

            var rEnemyDamages = rEnemyMainDamages;
            if (rEnemyEscortDamages != null)
                rEnemyDamages = rEnemyMainDamages.Concat(rEnemyEscortDamages).ToArray();

            if (rDamages.All(r => r == 0))
                return;

            for (var i = 0; i < rDamages.Length; i++)
            {
                var rParticipant = rParticipants[i];
                if (rParticipant != null)
                    rParticipant.Current -= rDamages[i];
            }

            if (rEnemyDamages.All(r => r == 0))
                return;

            var rFriendAttackers = RawData.Attackers[0];
            if (rFriendAttackers.Length == 1 && rFriendAttackers[0] != -1)
                rParticipants[rFriendAttackers[0] - 1].DamageGivenToOpponent += rEnemyDamages.Sum();
            else if (rFriendAttackers.Length > 1)
            {
                var rFirepowers = rFriendAttackers.Select(r =>
                {
                    var rShip = ((FriendShip)Stage.Friend[r - 1].Participant).Ship;

                    return rShip.Slots.Where(rpSlot => rpSlot.HasEquipment).Sum(rpSlot =>
                    {
                        var rEquipmentInfo = rpSlot.Equipment.Info;
                        switch (rEquipmentInfo.Type)
                        {
                            case EquipmentType.CarrierBasedDiveBomber:
                            case EquipmentType.SeaplaneBomber:
                                return rEquipmentInfo.DiveBomberAttack * Math.Sqrt(rpSlot.PlaneCount) + 25;

                            case EquipmentType.CarrierBasedTorpedoBomber:
                                return 1.15 * (rEquipmentInfo.Torpedo * Math.Sqrt(rpSlot.PlaneCount) + 25);

                            default: return 0;
                        }
                    });
                }).ToArray();

                var rTotalDamages = rEnemyDamages.Sum();
                var rTotalFirepowers = rFirepowers.Sum();

                if (rTotalDamages == 0)
                    return;

                for (var i = 0; i < rFriendAttackers.Length; i++)
                {
                    var rParticipant = rParticipants[rFriendAttackers[i] - 1];
                    rParticipant.DamageGivenToOpponent += (int)Math.Round(rTotalDamages * rFirepowers[i] / rTotalFirepowers);
                    rParticipant.Inaccurate = true;
                }
            }
        }
    }
}
