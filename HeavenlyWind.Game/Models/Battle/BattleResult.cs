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
            var rIsEnemyFlagshipSunk = (rpSecond == null ? rpFirst.Enemy : rpSecond.Enemy ?? rpFirst.Enemy)[0].State == BattleParticipantState.Sunk;

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

            OnPropertyChanged(nameof(FriendDamageRate));
            OnPropertyChanged(nameof(EnemyDamageRate));
            OnPropertyChanged(nameof(Rank));
        }
    }
}
