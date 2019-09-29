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

            public BattleParticipant MapShip(int index, bool isEnemy)
                => (isEnemy ? enemy : ally).FindShip(index);
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
                => (isEnemy ? enemy : ally)[index];
            public AttackType MapType(int rawType) => MapTypeStatic(rawType);
        }

        public ShellingPhase(int index, MasterDataRoot masterData, Side ally, Side enemy, RawShellingPhase raw, bool useFleet2 = false)
            : base(raw.OldSchema
                  ? Initialze(masterData, raw, new OldBuilder(useFleet2 ? ally.Fleet2 : ally.Fleet, enemy.Fleet))
                  : Initialze(masterData, raw, new Builder(ally, enemy)), index)
        { }

        private static AttackType MapTypeStatic(int rawType) => rawType switch
        {
            0 => AttackType.DayShelling,
            2 => AttackType.DayDoubleShelling,
            3 => AttackType.DayCutInMS,
            4 => AttackType.DayCutInMR,
            5 => AttackType.DayCutInMAp,
            6 => AttackType.DayCutInMM,
            7 => AttackType.DayAerialCutIn,
            100 => AttackType.NelsonTouch,
            101 => AttackType.NagatoShoot,
            102 => AttackType.MutsuShoot,
            103 => AttackType.ColoradoShoot,
            _ => AttackType.Unknown
        };
    }

    public class OpeningAswPhase : ShellingPhase
    {
        public OpeningAswPhase(MasterDataRoot masterData, Side ally, Side enemy, RawShellingPhase raw)
            : base(0, masterData, ally, enemy, raw, true)
        {
        }
    }
}
