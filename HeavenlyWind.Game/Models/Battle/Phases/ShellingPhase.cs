using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases
{
    class ShellingPhase : BattlePhase<RawShellingPhase>
    {
        bool r_IsEscortFleet;

        internal ShellingPhase(BattleStage rpStage, RawShellingPhase rpRawData, bool rpIsEscortFleet = false) : base(rpStage, rpRawData)
        {
            r_IsEscortFleet = rpIsEscortFleet;
        }

        internal protected override void Process()
        {
            if (RawData == null)
                return;
        }
    }
}
