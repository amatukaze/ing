namespace Sakuno.ING.Game.Models.Combat
{
    public class TorpedoPhase : BattlePhase
    {
        private readonly struct Builder : IBattlePhaseBuilder
        {
            private readonly Side ally;
            private readonly Side enemy;

            public Builder(Side ally, Side enemy)
            {
                this.ally = ally;
                this.enemy = enemy;
            }

            public BattleParticipant MapShip(int index, bool isEnemy)
                => (isEnemy ? enemy : ally).FindShip(index);
            public AttackType MapType(int rawType) => AttackType.DayTorpedo;
        }

        public TorpedoPhase(MasterDataRoot masterData, Side ally, Side enemy, RawTorpedoPhase raw, bool isOpening)
            : base(Initialze(masterData, raw, new Builder(ally, enemy)))
            => IsOpening = isOpening;

        public bool IsOpening { get; }
    }
}
