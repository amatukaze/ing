using System.Collections.Generic;

namespace Sakuno.ING.Game.Models.Combat
{
    public class ShellingPhase : BattlePhase
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

            public BattleParticipant MapAllyShip(int index) => ally.FindShip(index);
            public BattleParticipant MapEnemyShip(int index) => enemy.FindShip(index);
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

            public BattleParticipant MapAllyShip(int index)
                => index < 6 ? ally[index] : enemy[index - 6];
            public BattleParticipant MapEnemyShip(int index) => MapAllyShip(index);
            public AttackType MapType(int rawType) => MapTypeStatic(rawType);
        }

        public ShellingPhase(int index, MasterDataRoot masterData, Side ally, Side enemy, RawShellingPhase raw, bool useFleet2 = false)
            : base(raw.OldSchema
                  ? Initialze(masterData, raw, new OldBuilder(useFleet2 ? ally.Fleet2 : ally.Fleet, enemy.Fleet))
                  : Initialze(masterData, raw, new Builder(ally, enemy)))
            => Index = index;

        public int Index { get; }

        private static AttackType MapTypeStatic(int rawType)
        {
            switch (rawType)
            {
                case 0:
                    return AttackType.DayShelling;
                case 2:
                    return AttackType.DayDoubleShelling;
                case 3:
                    return AttackType.DayCutInMS;
                case 4:
                    return AttackType.DayCutInMR;
                case 5:
                    return AttackType.DayCutInMAp;
                case 6:
                    return AttackType.DayCutInMM;
                case 7:
                    return AttackType.DayAerialCutIn;
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
    }
}
