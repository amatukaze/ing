using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public enum BattleType { None, Normal, NightOnly, DayAfterNight, AerialCombat }

    public class BattleEvent : SortieEvent
    {
        public BattleType Type { get; }

        public BattleInfo Battle { get; }

        internal BattleEvent(RawMapExploration rpData) : base(rpData)
        {
            Battle = new BattleInfo();
        }
    }
}
