using Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Stages
{
    abstract class CombinedFleetDay : Day
    {
        public ShellingPhase ShellingThirdRound { get; protected set; }

        internal protected CombinedFleetDay(BattleInfo rpOwner) : base(rpOwner) { }
    }
}
