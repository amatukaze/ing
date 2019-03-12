using System.Collections.Generic;

namespace Sakuno.ING.Game.Models.Combat
{
    public class NightPhase : BattlePhase
    {
        private readonly struct Builder : IBattlePhaseBuilder
        {
            private readonly IReadOnlyList<BattleParticipant> ally, enemy;

            public Builder(IReadOnlyList<BattleParticipant> ally, IReadOnlyList<BattleParticipant> enemy)
            {
                this.ally = ally;
                this.enemy = enemy;
            }

            public BattleParticipant MapShip(int index, bool isEnemy)
                => (isEnemy ? enemy : ally)[index];
            public AttackType MapType(int rawType) => MapTypeStatic(rawType);
        }

        private static IReadOnlyList<BattleParticipant> SelectFleet(Side side, int? index)
        {
            switch (index)
            {
                case 1:
                    return side.Fleet;
                case 2:
                    return side.Fleet2;
                default:
                    return side.Fleet2 ?? side.Fleet;
            }
        }

        public NightPhase(MasterDataRoot masterData, Side ally, Side enemy, RawNightPhase raw)
            : base(Initialze(masterData, raw, new Builder(SelectFleet(ally, raw.Ally.ActiveFleet), SelectFleet(enemy, raw.Enemy.ActiveFleet))))
        {
            Ally = new NightEffects(masterData, SelectFleet(ally, raw.Ally.ActiveFleet), raw.Ally);
            Enemy = new NightEffects(masterData, SelectFleet(enemy, raw.Enemy.ActiveFleet), raw.Enemy);
        }

        protected NightPhase(int index, MasterDataRoot masterData, IReadOnlyList<BattleParticipant> ally, RawNightPhase raw, IEnumerable<Attack> attacks)
            : base(attacks, index)
        {
            Ally = new NightEffects(masterData, ally, raw.Ally);
            Enemy = new NightEffects(masterData, null, raw.Enemy);
        }

        protected static AttackType MapTypeStatic(int rawType) => rawType switch
        {
            0 => AttackType.NightShelling,
            1 => AttackType.NightDoubleShelling,
            2 => AttackType.NightCutInMT,
            3 => AttackType.NightCutInTT,
            4 => AttackType.NightCutInMMS,
            5 => AttackType.NightCutInMMM,
            6 => AttackType.NightAerialCutIn,
            7 => AttackType.NightDestroyerMTR,
            8 => AttackType.NightDestroyerTRP,
            100 => AttackType.NelsonTouch,
            101 => AttackType.NagatoShoot,
            102 => AttackType.MutsuShoot,
            _ => AttackType.Unknown
        };

        public NightEffects Ally { get; }
        public NightEffects Enemy { get; }
    }

    public class CombinedNightPhase : NightPhase
    {
        private readonly struct Builder : IBattlePhaseBuilder
        {
            private readonly IReadOnlyList<BattleParticipant> ally;
            private readonly Side enemy;

            public Builder(IReadOnlyList<BattleParticipant> ally, Side enemy)
            {
                this.ally = ally;
                this.enemy = enemy;
            }

            public BattleParticipant MapShip(int index, bool isEnemy)
                => isEnemy ? enemy.FindShip(index) : ally[index];
            public AttackType MapType(int rawType) => MapTypeStatic(rawType);
        }

        public CombinedNightPhase(int index, MasterDataRoot masterData, IReadOnlyList<BattleParticipant> ally, Side enemy, RawNightPhase raw)
            : base(index, masterData, ally, raw, Initialze(masterData, raw, new Builder(ally, enemy)))
        { }
    }

    public class NpcPhase : CombinedNightPhase
    {
        public NpcPhase(MasterDataRoot masterData, IReadOnlyList<BattleParticipant> npcFleet, Side enemy, RawNightPhase raw)
            : base(0, masterData, npcFleet, enemy, raw)
            => NpcFleet = npcFleet;

        public IReadOnlyList<BattleParticipant> NpcFleet { get; }
    }
}
