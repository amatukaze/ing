using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Game.Models.Combat;

namespace Sakuno.ING.Game.Json.Combat
{
    partial class BattleJson
    {
        public class Torpedo : IRawBattlePhase
        {
#pragma warning disable IDE1006 // Naming Styles
            public int[] api_frai { set => value.AlignSet(ally, (r, v) => r.TargetIndex = v); }
            public int[] api_erai { set => value.AlignSet(enemy, (r, v) => r.TargetIndex = v); }
            public int[] api_fydam { set => value.AlignSet(ally, (r, v) => r.damage = v); }
            public int[] api_eydam { set => value.AlignSet(ally, (r, v) => r.damage = v); }
            public int[] api_fcl { set => value.AlignSet(ally, (r, v) => r.IsCritical = v == 2); }
            public int[] api_ecl { set => value.AlignSet(ally, (r, v) => r.IsCritical = v == 2); }
#pragma warning restore IDE1006 // Naming Styles

            private readonly List<Hit> ally = new List<Hit>();
            private readonly List<Hit> enemy = new List<Hit>();

            public IReadOnlyList<IRawAttack> Attacks
                => ally.Select((x, i) => new SingleAttack
                {
                    SourceIndex = i,
                    Hit = x
                })
                .Concat(enemy.Select((x, i) => new SingleAttack
                {
                    SourceIndex = i,
                    Hit = x,
                    IsEnemy = true
                }))
                .Where(x => x.Hit.TargetIndex >= 0)
                .ToArray();
        }

        public Torpedo api_opening_atack;
        public IRawBattlePhase OpendingTorpedoPhase => api_opening_atack;

        public Torpedo api_raigeki;
        public IRawBattlePhase ClosingTorpedoPhase => api_raigeki;
    }
}
