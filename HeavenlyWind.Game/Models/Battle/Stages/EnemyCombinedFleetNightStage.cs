using Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Stages
{
    class EnemyCombinedFleetNightStage : CombinedFleetNight
    {
        internal protected EnemyCombinedFleetNightStage(BattleInfo rpOwner, ApiInfo rpInfo) : base(rpOwner)
        {
            var rRawData = rpInfo.Data as RawEnemyCombinedFleetNight;

            Shelling = new ShellingPhase(this, rRawData.Shelling) { ParticipantingFleet = rRawData.ParticipatingFleet };
        }
    }
}
