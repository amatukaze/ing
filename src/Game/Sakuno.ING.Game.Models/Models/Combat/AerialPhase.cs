namespace Sakuno.ING.Game.Models.Combat
{
    public class AerialPhase : BattlePhase
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

            public AttackType MapType(int rawType) => rawType switch
            {
                RawAerialPhase.BombFlag => AttackType.AerialBomb,
                RawAerialPhase.TorpedoFlag => AttackType.AerialTorpedo,
                RawAerialPhase.BombFlag | RawAerialPhase.TorpedoFlag => AttackType.AerialBoth,
                _ => AttackType.Unknown
            };
        }

        public AerialPhase(int index, MasterDataRoot masterData, Side ally, Side enemy, RawAerialPhase raw)
            : base(Initialze(masterData, raw, new Builder(ally, enemy)), index)
        {
            FightingResult = raw.FightingResult;
            if (raw.AntiAirFire != null)
                AntiAirFire = new AntiAirFire(masterData, ally, raw.AntiAirFire);
            Ally = new AerialSide(masterData, raw.Ally, ally);
            Enemy = new AerialSide(masterData, raw.Enemy, enemy);
        }

        public AirFightingResult? FightingResult { get; }
        public AntiAirFire AntiAirFire { get; }
        public AerialSide Ally { get; }
        public AerialSide Enemy { get; }
    }

    public class JetPhase : AerialPhase
    {
        public JetPhase(MasterDataRoot masterData, Side ally, Side enemy, RawAerialPhase raw, bool isLandBase)
            : base(0, masterData, ally, enemy, raw)
            => IsLandBase = isLandBase;

        public bool IsLandBase { get; }
    }
}
