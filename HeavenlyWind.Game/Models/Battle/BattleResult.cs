using Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Stages;
using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle
{
    public class BattleResult : ModelBase
    {
        public double FriendDamageRate { get; private set; }
        public double EnemyDamageRate { get; private set; }

        public BattleRank Rank { get; private set; }

        internal void Update(BattleStage rpFirst, BattleStage rpSecond)
        {
            var rCurrentStage = rpSecond ?? rpFirst;

            var rFriendInitialTotalHP = 0;
            var rFriendTotalHP = 0;
            var rFriendCount = rpFirst.Friend.Count;
            var rFriendSunkCount = 0;

            for (var i = 0; i < rFriendCount; i++)
            {
                if (rpFirst.Friend[i].IsEvacuated)
                    continue;

                if (rCurrentStage.Friend[i].Current <= 0)
                    rFriendSunkCount++;

                rFriendInitialTotalHP += rpFirst.Friend[i].Before;
                rFriendTotalHP += Math.Max(rCurrentStage.Friend[i].Current, 0);
            }
            var rEnemyInitialTotalHP = 0;
            var rEnemyTotalHP = 0;
            var rEnemyCount = rpFirst.Enemy.Count;
            var rEnemySunkCount = 0;
            var rIsEnemyFlagshipSunk = rCurrentStage.Enemy[0].Current <= 0;

            for (var i = 0; i < rEnemyCount; i++)
            {
                if (rCurrentStage.Enemy[i].Current <= 0)
                    rEnemySunkCount++;

                rEnemyInitialTotalHP += rpFirst.Enemy[i].Before;
                rEnemyTotalHP += Math.Max(rCurrentStage.Enemy[i].Current, 0);
            }

            FriendDamageRate = (rFriendInitialTotalHP - rFriendTotalHP) / (double)rFriendInitialTotalHP * 100.0;
            EnemyDamageRate = (rEnemyInitialTotalHP - rEnemyTotalHP) / (double)rEnemyInitialTotalHP * 100.0;

            if (rCurrentStage is AerialAttackStage)
            {
                if (FriendDamageRate == .0)
                    Rank = BattleRank.SS;
                else if (FriendDamageRate < 10.0)
                    Rank = BattleRank.A;
                else if (FriendDamageRate < 20.0)
                    Rank = BattleRank.B;
                else if (FriendDamageRate < 50.0)
                    Rank = BattleRank.C;
                else if (FriendDamageRate < 80.0)
                    Rank = BattleRank.D;
                else
                    Rank = BattleRank.E;
            }
            else
            {
                if (rFriendSunkCount == 0 && rEnemySunkCount == rEnemyCount)
                    Rank = FriendDamageRate <= .0 ? BattleRank.SS : BattleRank.S;
                else if (rFriendSunkCount == 0 && rEnemySunkCount >= (int)(rEnemyCount * .7))
                    Rank = BattleRank.A;
                else if ((rFriendSunkCount < rEnemySunkCount && rIsEnemyFlagshipSunk) || EnemyDamageRate > FriendDamageRate * 2.5)
                    Rank = BattleRank.B;
                else if (EnemyDamageRate > FriendDamageRate * .9)
                    Rank = BattleRank.C;
                else if (rFriendCount == 1 && rCurrentStage.Friend[0].State == BattleParticipantState.HeavilyDamaged)
                    Rank = BattleRank.D;
                else if (rFriendCount > 1 && rFriendSunkCount == rFriendCount - 1)
                    Rank = BattleRank.E;
                else
                    Rank = BattleRank.D;
            }

            OnPropertyChanged(string.Empty);
        }
    }
}
