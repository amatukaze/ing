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
            if (rStage1 == null)
                return;

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

        void ProcessStage2(AerialCombat rpInfo)
        {
            var rStage2 = RawData.Stage2;
            if (rStage2 == null)
                return;

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

            if (rStage2.AntiAirCutIn != null)
            {
                var rTriggerer = Stage.Friend[rStage2.AntiAirCutIn.TriggererIndex].Participant as FriendShip;
                if (rTriggerer != null)
                    rTriggerer.AntiAirCutIn = new AntiAirCutIn(rStage2.AntiAirCutIn);
            }
        }

        void ProcessStage3()
        {
            var rStage3 = RawData.Stage3;
            if (rStage3 == null)
                return;

            var rTotalEnemyDamages = 0;

            var rDamages = new int[28];
            for (var i = 0; i < 7; i++)
            {
                if (i < rStage3.FriendDamage.Length)
                    rDamages[i] = rStage3.FriendDamage[i];

                if (i < rStage3.EnemyDamage.Length)
                {
                    var rEnemyDamage = rStage3.EnemyDamage[i];
                    rDamages[i + 7] = rEnemyDamage;
                    rTotalEnemyDamages += rEnemyDamage;
                }
            }

            var rCount = 14;

            var rCombinedFleet = RawData.Stage3CombinedFleet;
            if (rCombinedFleet != null)
            {
                var rFriendCombinedFleetDamages = rCombinedFleet.FriendDamage;
                if (rFriendCombinedFleetDamages != null)
                {
                    for (var i = 0; i < rFriendCombinedFleetDamages.Length; i++)
                        rDamages[i + 14] = rFriendCombinedFleetDamages[i];

                    rCount += 7;
                }

                var rEnemyCombinedFleetDamages = rCombinedFleet.EnemyDamage;
                if (rEnemyCombinedFleetDamages != null)
                {
                    for (var i = 0; i < rEnemyCombinedFleetDamages.Length; i++)
                    {
                        var rDamage = rEnemyCombinedFleetDamages[i];
                        rDamages[i + 21] = rDamage;
                        rTotalEnemyDamages += rDamage;
                    }

                    rCount += 7;
                }
            }

            var rIsAllZero = true;

            for (var i = 0; i < rCount; i++)
                if (rDamages[i] != 0)
                {
                    rIsAllZero = false;
                    break;
                }

            if (rIsAllZero)
                return;

            var rParticipants = Stage.FriendAndEnemy;
            for (var i = 0; i < rCount; i++)
            {
                var rParticipant = rParticipants[i];
                if (rParticipant != null)
                    rParticipant.Current -= rDamages[i];
            }

            if (rTotalEnemyDamages == 0)
                return;

            var rFriendAttackers = RawData.Attackers[0];
            if (rFriendAttackers.Length == 1)
                rParticipants[rFriendAttackers[0] - 1].DamageGivenToOpponent += rTotalEnemyDamages;
            else if (rFriendAttackers.Length > 1)
            {
                var rFirepowers = new double[rFriendAttackers.Length];
                var rTotalFirepower = .0;

                for (var i = 0; i < rFriendAttackers.Length;i++)
                {
                    var rAttacker = rFriendAttackers[i];
                    var rShip = ((FriendShip)Stage.Friend[rAttacker - 1].Participant).Ship;

                    var rFirepower = .0;

                    foreach (var rSlot in rShip.Slots)
                    {
                        if (!rSlot.HasEquipment)
                            continue;

                        var rInfo = rSlot.Equipment.Info;
                        switch (rInfo.Type)
                        {
                            case EquipmentType.CarrierBasedDiveBomber:
                            case EquipmentType.SeaplaneBomber:
                            case EquipmentType.JetPoweredFighterBomber:
                                rFirepower += rInfo.DiveBomberAttack * Math.Sqrt(rSlot.PlaneCount) + 25.0;
                                break;

                            case EquipmentType.CarrierBasedTorpedoBomber:
                            case EquipmentType.JetPoweredAttackAircraft:
                                rFirepower += (rInfo.Torpedo * Math.Sqrt(rSlot.PlaneCount) + 25.0) * 1.15;
                                break;
                        }
                    }

                    rFirepowers[i] = rFirepower;
                    rTotalFirepower += rFirepower;
                }

                if (rTotalFirepower == 0)
                    return;

                var friendMainCount = Stage.FriendMain.Count;

                for (var i = 0; i < rFriendAttackers.Length; i++)
                {
                    if (rFirepowers[i] == 0)
                        continue;

                    var index = rFriendAttackers[i] - 1;

                    BattleParticipantSnapshot participant;

                    if (index < friendMainCount)
                        participant = Stage.FriendMain[index];
                    else
                        participant = Stage.FriendEscort[index - 6];

                    participant.DamageGivenToOpponent += (int)Math.Round(rTotalEnemyDamages * rFirepowers[i] / rTotalFirepower);
                    participant.Inaccurate = true;
                }
            }
        }
    }
}
