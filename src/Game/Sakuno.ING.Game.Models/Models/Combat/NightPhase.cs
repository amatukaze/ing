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

        private static AttackType MapTypeStatic(int rawType)
        {
            switch (rawType)
            {
                case 0:
                    return AttackType.NightShelling;
                case 1:
                    return AttackType.NightDoubleShelling;
                case 2:
                    return AttackType.NightCutInMT;
                case 3:
                    return AttackType.NightCutInTT;
                case 4:
                    return AttackType.NightCutInMMS;
                case 5:
                    return AttackType.NightCutInMMM;
                case 6:
                    return AttackType.NightAerialCutIn;
                case 7:
                    return AttackType.NightDestroyerMTR;
                case 8:
                    return AttackType.NightDestroyerTRP;
                case 100:
                    return AttackType.NelsonTouch;
                case 101:
                    return AttackType.NagatoShoot;
                case 102:
                    return AttackType.MutsuShoot;
                default:
                    return AttackType.Unknown;
            }
        }

        public int Index { get; }
        public NightEffects Ally { get; }
        public NightEffects Enemy { get; }
    }
}
