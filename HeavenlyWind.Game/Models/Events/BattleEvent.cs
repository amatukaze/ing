using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public enum BattleType { None, Normal, NightOnlyBattle, AerialCombat = 4 }

    public class BattleEvent : SortieEvent
    {
        public BattleType Type { get; }

        internal BattleEvent(RawMapExploration rpData) : base(rpData)
        {
        }
    }
}
