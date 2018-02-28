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
                BattleParticipantSnapshot participant;

                var index = rStage2.AntiAirCutIn.TriggererIndex;
                var friendMainCount = Math.Max(Stage.FriendMain.Count, 6);

                if (index < friendMainCount)
                    participant = Stage.FriendMain[index];
                else
                    participant = Stage.FriendEscort[index - friendMainCount];

                if (participant.Participant is FriendShip triggerer)
                    triggerer.AntiAirCutIn = new AntiAirCutIn(rStage2.AntiAirCutIn);
            }
        }

        void ProcessStage3()
        {
            var rStage3 = RawData.Stage3;
            if (rStage3 == null)
                return;

            var totalEnemyDamage = 0;

            var friendDamage = rStage3.FriendDamage;
            if (friendDamage != null)
                for (var i = 0; i < Stage.FriendMain.Count; i++)
                    Stage.FriendMain[i].Current -= friendDamage[i];

            var enemyDamage = rStage3.EnemyDamage;
            if (enemyDamage != null)
                for (var i = 0; i < Stage.EnemyMain.Count; i++)
                {
                    var damage = rStage3.EnemyDamage[i];

                    Stage.EnemyMain[i].Current -= damage;
                    totalEnemyDamage += damage;
                }

            var combinedFleet = RawData.Stage3CombinedFleet;
            if (combinedFleet != null)
            {
                var friendCombinedFleetDamages = combinedFleet.FriendDamage;
                if (friendCombinedFleetDamages != null)
                    for (var i = 0; i < Stage.FriendEscort.Count; i++)
                        Stage.FriendEscort[i].Current -= friendCombinedFleetDamages[i];

                var enemyCombinedFleetDamages = combinedFleet.EnemyDamage;
                if (enemyCombinedFleetDamages != null)
                    for (var i = 0; i < Stage.EnemyEscort.Count; i++)
                    {
                        var damage = enemyCombinedFleetDamages[i];

                        Stage.EnemyEscort[i].Current -= damage;
                        totalEnemyDamage += damage;
                    }
            }

            if (totalEnemyDamage == 0)
                return;

            var friendMainCount = Math.Max(Stage.FriendMain.Count, 6);

            var friendAttackers = RawData.Attackers[0];
            if (friendAttackers.Length == 1)
            {
                var index = friendAttackers[0] - 1;

                BattleParticipantSnapshot participant = null;

                if (index < Stage.FriendMain.Count)
                    participant = Stage.FriendMain[index];
                else if (index >= friendMainCount)
                    participant = Stage.FriendEscort[index - friendMainCount];

                if (participant != null)
                    participant.DamageGivenToOpponent += totalEnemyDamage;
            }
            else if (friendAttackers.Length > 1)
            {
                var rFirepowers = new double[friendAttackers.Length];
                var rTotalFirepower = .0;

                for (var i = 0; i < friendAttackers.Length; i++)
                {
                    var rAttacker = friendAttackers[i];
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

                for (var i = 0; i < friendAttackers.Length; i++)
                {
                    if (rFirepowers[i] == 0)
                        continue;

                    var index = friendAttackers[i] - 1;

                    BattleParticipantSnapshot participant;

                    if (index < friendMainCount)
                        participant = Stage.FriendMain[index];
                    else
                        participant = Stage.FriendEscort[index - friendMainCount];

                    participant.DamageGivenToOpponent += (int)Math.Round(totalEnemyDamage * rFirepowers[i] / rTotalFirepower);
                    participant.Inaccurate = true;
                }
            }
        }
    }
}
