using System.Collections.Generic;
using System.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.OSS
{
    abstract class SinkTarget : Battle
    {
        public HashSet<int> TargetType { get; private set; }

        protected SinkTarget(int[] rpTargetType)
        {
            TargetType = new HashSet<int>(rpTargetType);
        }

        protected override void Process(ProgressInfo rpProgress, BattleInfo rpBattle, RawBattleResult rpResult)
        {
            rpProgress.Progress += rpBattle.CurrentStage.Enemy.Count(r => r.State == BattleParticipantState.Sunk && TargetType.Contains(r.Participant.Info.Type.ID));
        }
    }

    [Quest(218)]
    [Quest(213)]
    [Quest(212)]
    [Quest(221)]
    class SinkSupplyShip : SinkTarget
    {
        public SinkSupplyShip() : base(new[] { 15 }) { }
    }

    [Quest(211)]
    [Quest(220)]
    class SinkCarrier : SinkTarget
    {
        public SinkCarrier() : base(new[] { 7, 11 }) { }
    }

    [Quest(228)]
    [Quest(230)]
    class SinkSubmarine : SinkTarget
    {
        public SinkSubmarine() : base(new[] { 13 }) { }
    }
}
