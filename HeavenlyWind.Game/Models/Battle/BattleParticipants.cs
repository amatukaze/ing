using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle
{
    public class BattleParticipants : ModelBase
    {
        public IList<IParticipant> FriendMain { get; internal set; }
        public IList<IParticipant> FriendEscort { get; internal set; }

        IList<IParticipant> r_Enemy;
        public IList<IParticipant> Enemy
        {
            get { return r_Enemy; }
            internal set
            {
                r_Enemy = value;
                OnPropertyChanged(nameof(Enemy));
            }
        }
    }
}
