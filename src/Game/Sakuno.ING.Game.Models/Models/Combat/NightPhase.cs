using System.Collections.Generic;

namespace Sakuno.ING.Game.Models.Combat
{
    public class NightPhase : BattlePhase
    {
        private readonly struct Builder : IBattlePhaseBuilder
        {
            private readonly IReadOnlyList<BattleParticipant> ally, enemy;

            public Builder(Side ally, Side enemy)
            {
                this.ally = ally.NightActiveFleet ?? ally.Fleet2 ?? ally.Fleet;
                this.enemy = enemy.NightActiveFleet ?? enemy.Fleet2 ?? enemy.Fleet;
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
            private readonly Side ally;
            private readonly Side enemy;

            public CombinedBuilder(Side ally, Side enemy)
            {
                this.ally = ally;
                this.enemy = enemy;
            }

            public BattleParticipant MapShip(int index, bool isEnemy)
                => (isEnemy ? enemy : ally).FindShip(index);
            public AttackType MapType(int rawType) => MapTypeStatic(rawType);
        }

        public NightPhase(int index, MasterDataRoot masterData, Side ally, Side enemy, RawShellingPhase raw, bool combined)
            : base(raw.OldSchema
                  ? Initialze(masterData, raw, new OldBuilder(ally.Fleet2 ?? ally.Fleet, enemy.Fleet))
                  : combined
                  ? Initialze(masterData, raw, new CombinedBuilder(ally, enemy))
                  : Initialze(masterData, raw, new Builder(ally, enemy)))
        {
            Index = index;
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
    }
}
