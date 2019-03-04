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

        private readonly struct OldBuilder : IBattlePhaseBuilder
        {
            private readonly IReadOnlyList<BattleParticipant> ally, enemy;

            public OldBuilder(IReadOnlyList<BattleParticipant> ally, IReadOnlyList<BattleParticipant> enemy)
            {
                this.ally = ally;
                this.enemy = enemy;
            }

            public BattleParticipant MapShip(int index, bool isEnemy)
                => index < 6 ? ally[index] : enemy[index - 6];
            public AttackType MapType(int rawType) => MapTypeStatic(rawType);
        }

        private readonly struct CombinedBuilder : IBattlePhaseBuilder
        {
            private readonly IReadOnlyList<BattleParticipant> ally;
            private readonly Side enemy;

            public CombinedBuilder(IReadOnlyList<BattleParticipant> ally, Side enemy)
            {
                this.ally = ally;
                this.enemy = enemy;
            }

            public BattleParticipant MapShip(int index, bool isEnemy)
                => isEnemy ? enemy.FindShip(index) : ally[index];
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

        public NightPhase(int index, MasterDataRoot masterData, Side ally, Side enemy, RawNightPhase raw, bool combined)
            : base(raw.OldSchema
                  ? Initialze(masterData, raw, new OldBuilder(ally.Fleet2 ?? ally.Fleet, enemy.Fleet))
                  : combined
                  ? Initialze(masterData, raw, new CombinedBuilder(ally.Fleet, enemy))
                  : Initialze(masterData, raw, new Builder(SelectFleet(ally, raw.Ally.ActiveFleet), SelectFleet(enemy, raw.Enemy.ActiveFleet))))
        {
            Index = index;
            Ally = new NightEffects(masterData, SelectFleet(ally, raw.Ally.ActiveFleet), raw.Ally);
            Enemy = new NightEffects(masterData, SelectFleet(enemy, raw.Enemy.ActiveFleet), raw.Enemy);
        }

        public NightPhase(MasterDataRoot masterData, IReadOnlyList<BattleParticipant> npc, Side enemy, RawNightPhase raw)
            : base(Initialze(masterData, raw, new CombinedBuilder(npc, enemy)))
        {
            Index = -1;
        }

        private static AttackType MapTypeStatic(int rawType) => rawType switch
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

        public int Index { get; }
        public NightEffects Ally { get; }
        public NightEffects Enemy { get; }
    }
}
