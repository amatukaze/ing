using System.Collections.Generic;

namespace Sakuno.ING.Game.Models.Battle
{
    public readonly struct RawAttack
    {
        public RawAttack(bool isEnemy, int type, IReadOnlyList<RawHit> hits)
        {
            IsEnemy = isEnemy;
            Type = type;
            Hits = hits;
        }

        public bool IsEnemy { get; }
        public int Type { get; }
        public IReadOnlyList<RawHit> Hits { get; }
    }
}
