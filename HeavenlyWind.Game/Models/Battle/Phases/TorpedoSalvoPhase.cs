using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases
{
    class TorpedoSalvoPhase : BattlePhase<RawTorpedoSalvoPhase>
    {
        bool r_IsEscortFleet;

        internal TorpedoSalvoPhase(BattleStage rpStage, RawTorpedoSalvoPhase rpRawData, bool rpIsEscortFleet = false) : base(rpStage, rpRawData)
        {
            r_IsEscortFleet = rpIsEscortFleet;
        }

        protected internal override void Process()
        {
            if (RawData == null)
                return;
        }
    }
}
