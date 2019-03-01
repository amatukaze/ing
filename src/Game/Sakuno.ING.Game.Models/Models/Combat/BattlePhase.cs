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
            => raw.Attacks.Select(x => new Attack((x.SourceIndex is int source) ? (x.EnemyAttacks ? builder.MapEnemyShip(source) : builder.MapAllyShip(source)) : null,
                     builder.MapType(x.Type), masterData.EquipmentInfos[x.EquipmentUsed],
                     x.Hits.Select(h => new Hit(x.EnemyAttacks ? builder.MapAllyShip(h.TargetIndex) : builder.MapEnemyShip(h.TargetIndex), h)))).ToArray();
    }
}
