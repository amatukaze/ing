namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Stages
{
    class CombinedFleetNight : Night
    {
        public override BattleStageType Type => BattleStageType.Night;

        internal protected CombinedFleetNight(BattleInfo rpOwner) : base(rpOwner)
        {
        }
    }
}
