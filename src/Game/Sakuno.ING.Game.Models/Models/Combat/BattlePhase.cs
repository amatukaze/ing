using System.Collections.Generic;
using System.Linq;

namespace Sakuno.ING.Game.Models.Combat
{
    public abstract class BattlePhase
    {
        public IReadOnlyList<Attack> Attacks { get; }
        protected BattlePhase(IEnumerable<Attack> attacks) => Attacks = attacks.ToArray();

        protected static IEnumerable<Attack> Initialze<TBuilder>(MasterDataRoot masterData, RawBattlePhase raw, TBuilder builder)
            where TBuilder : IBattlePhaseBuilder
            => from attack in raw.Attacks
               let source = attack.SourceIndex is int s ? builder.MapShip(s, attack.EnemyAttacks) : null
               select new Attack(source,
                   builder.MapType(attack.Type),
                   masterData.EquipmentInfos[attack.EquipmentUsed],
                   from hit in attack.Hits
                   select new Hit(source, builder.MapShip(hit.TargetIndex, !attack.EnemyAttacks), hit));
    }
}
