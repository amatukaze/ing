using Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Stages;
using System;
using System.Linq;

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

            var rEnemyTotalHPBefore = (double)rpFirst.Enemy.Sum(r => r.Before);
            var rFriendTotalHPBefore = (double)rpFirst.FriendMain.Sum(r => r.Before);

            if (rpFirst.FriendEscort != null)
                rFriendTotalHPBefore += rpFirst.FriendEscort.Sum(r => r.Before);

            var rEnemyFinalSnapshot = rpSecond == null ? rpFirst.Enemy : rpSecond.Enemy;
            var rEnemyTotalHPAfter = (double)rEnemyFinalSnapshot.Sum(r => Math.Max(r.Current, 0));

            var rFriendTotalHPAfter = (double)(rpSecond == null ? rpFirst.FriendMain : rpSecond.FriendMain ?? rpFirst.FriendMain).Sum(r => Math.Max(r.Current, 0));

            var rFriendEscortFinalSnapshot = rpSecond == null ? rpFirst.FriendEscort : rpSecond.FriendEscort ?? rpFirst.FriendEscort;
            if (rFriendEscortFinalSnapshot != null)
                rFriendTotalHPAfter += rFriendEscortFinalSnapshot.Sum(r => Math.Max(r.Current, 0));

            FriendDamageRate = (rFriendTotalHPBefore - rFriendTotalHPAfter) / rFriendTotalHPBefore * 100;
            EnemyDamageRate = (rEnemyTotalHPBefore - rEnemyTotalHPAfter) / rEnemyTotalHPBefore * 100;

            var rEnemySunkCount = rEnemyFinalSnapshot.Count(r => r.State == BattleParticipantState.Sunk);
            var rIsEnemyFlagshipSunk = rEnemyFinalSnapshot[0].State == BattleParticipantState.Sunk;

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
                if (EnemyDamageRate >= 100)
                    Rank = FriendDamageRate <= 0 ? BattleRank.SS : BattleRank.S;
                else if (rEnemySunkCount >= Math.Round(rEnemyFinalSnapshot.Count * 0.6))
                    Rank = BattleRank.A;
                else if (rIsEnemyFlagshipSunk || EnemyDamageRate > FriendDamageRate * 2.5)
                    Rank = BattleRank.B;
                else if (EnemyDamageRate > FriendDamageRate || EnemyDamageRate >= 50.0)
                    Rank = BattleRank.C;
                else
                    Rank = BattleRank.D;
            }

            OnPropertyChanged(nameof(FriendDamageRate));
            OnPropertyChanged(nameof(EnemyDamageRate));
            OnPropertyChanged(nameof(Rank));
        }
    }
}
