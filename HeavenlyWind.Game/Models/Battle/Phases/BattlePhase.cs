namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases
{
    public abstract class BattlePhase
    {
        public BattleStage Stage { get; }

        internal protected BattlePhase(BattleStage rpOwner)
        {
            Stage = rpOwner;
        }

        internal protected abstract void Process();
    }
    public abstract class BattlePhase<T> : BattlePhase
    {
        protected T RawData { get; }

        internal protected BattlePhase(BattleStage rpOwner, T rpRawData) : base(rpOwner)
        {
            RawData = rpRawData;
        }
    }
}
