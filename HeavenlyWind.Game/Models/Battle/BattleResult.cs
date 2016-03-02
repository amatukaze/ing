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

        internal void Update(BattleStage rpStage)
        {
            var rEnemyTotalHPBefore = (double)rpStage.Enemy.Sum(r => r.Before);
            var rFriendTotalHPBefore = (double)rpStage.FriendMain.Sum(r => r.Before);

            if (rpStage.FriendEscort != null)
                rFriendTotalHPBefore += rpStage.FriendEscort.Sum(r => r.Before);

            var rEnemyTotalHPAfter = (double)rpStage.Enemy.Sum(r => Math.Max(r.Current, 0));
            var rFriendTotalHPAfter = (double)rpStage.FriendMain.Sum(r => Math.Max(r.Current, 0));

            var rFriendEscortFinalSnapshot = rpStage.FriendEscort;
            if (rFriendEscortFinalSnapshot != null)
                rFriendTotalHPAfter += rFriendEscortFinalSnapshot.Sum(r => Math.Max(r.Current, 0));

            FriendDamageRate = (rFriendTotalHPBefore - rFriendTotalHPAfter) / rFriendTotalHPBefore * 100;
            EnemyDamageRate = (rEnemyTotalHPBefore - rEnemyTotalHPAfter) / rEnemyTotalHPBefore * 100;

            var rEnemySunkCount = rpStage.Enemy.Count(r => r.State == BattleParticipantState.Sunk);
            var rIsEnemyFlagshipSunk = rpStage.Enemy[0].State == BattleParticipantState.Sunk;

            if (rpStage is AerialAttackStage)
            {
                if (FriendDamageRate == .0)
                    Rank = BattleRank.SS;
                else if (FriendDamageRate < .1)
                    Rank = BattleRank.A;
                else if (FriendDamageRate < .2)
                    Rank = BattleRank.B;
                else if (FriendDamageRate < .5)
                    Rank = BattleRank.C;
                else
                    Rank = BattleRank.D;
            }
            else
            {
                if (EnemyDamageRate >= 100)
                    Rank = FriendDamageRate <= 0 ? BattleRank.SS : BattleRank.S;
                else if (rEnemySunkCount >= Math.Round(rpStage.Enemy.Count * 0.6))
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
