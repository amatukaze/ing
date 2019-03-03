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

            public AttackType MapType(int rawType)
            {
                switch (rawType)
                {
                    case RawAerialPhase.BombFlag:
                        return AttackType.AerialBomb;
                    case RawAerialPhase.TorpedoFlag:
                        return AttackType.AerialTorpedo;
                    case RawAerialPhase.BombFlag | RawAerialPhase.TorpedoFlag:
                        return AttackType.AerialBoth;
                    default:
                        return AttackType.Unknown;
                }
            }
        }

        public AerialPhase(int index, MasterDataRoot masterData, Side ally, Side enemy, RawAerialPhase raw, bool isJet = false)
            : base(Initialze(masterData, raw, new Builder(ally, enemy)))
        {
            Index = index;
            IsJet = isJet;
            FightingResult = raw.FightingResult;
            if (raw.AntiAirFire != null)
                AntiAirFire = new AntiAirFire(masterData, ally, raw.AntiAirFire);
            Ally = new AerialSide(masterData, raw.Ally, ally);
        }

        public int Index { get; }
        public bool IsJet { get; }
        public AirFightingResult? FightingResult { get; }
        public AntiAirFire AntiAirFire { get; }
        public AerialSide Ally { get; }
        public AerialSide Enemy { get; }
    }
}
