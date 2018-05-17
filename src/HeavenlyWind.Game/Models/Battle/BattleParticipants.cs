using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle
{
    public class BattleParticipants : ModelBase
    {
        public IList<IParticipant> FriendMain { get; internal set; }
        public IList<IParticipant> FriendEscort { get; internal set; }

        IList<IParticipant> r_EnemyMain;
        public IList<IParticipant> EnemyMain
        {
            get { return r_EnemyMain; }
            internal set
            {
                r_EnemyMain = value;
                OnPropertyChanged(nameof(EnemyMain));
            }
        }

        IList<IParticipant> r_EnemyEscort;
        public IList<IParticipant> EnemyEscort
        {
            get { return r_EnemyEscort; }
            internal set
            {
                r_EnemyEscort = value;
                OnPropertyChanged(nameof(EnemyEscort));
            }
        }
    }
}
