using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Combat;

namespace Sakuno.ING.Game.Json.Combat
{
    partial class BattleJson
    {
        public SupportFireType? api_support_flag;
        public SupportFireType? api_n_support_flag;
        public SupportFireType? SupportFireType => api_support_flag ?? api_n_support_flag;

        public class AerialSupport : Aerial
        {
            public ShipId?[] api_ship_id;
        }

        public class Support : IRawBattlePhase
        {
            public ShipId?[] api_ship_id;
#pragma warning disable IDE1006 // Naming Styles
            public int[] api_cl_list { set => value.AlignSet(hits, (r, v) => r.critical = v == 2); }
            public double[] api_damage { set => value.AlignSet(hits, (r, v) => r.damage = v); }
#pragma warning restore IDE1006 // Naming Styles
            public readonly List<PartialHit> hits = new List<PartialHit>();

            public IReadOnlyList<IRawAttack> Attacks
                => hits.Select((x, i) => new SingleAttack
                {
                    Hit = new Hit
                    {
                        damage = x.damage,
                        IsCritical = x.critical,
                        TargetIndex = i
                    }
                })
                .Where(x => x.Hit.damage > 0)
                .ToArray();
        }

        public class SupportInfo
        {
            public AerialSupport api_support_airatack;
            public Support api_support_hourai;
        }
        public SupportInfo api_support_info;
        public SupportInfo api_n_support_info;

        public IReadOnlyList<ShipId?> SupportFleet
            => (api_support_info ?? api_n_support_info)?.api_support_airatack?.api_ship_id
            ?? (api_support_info ?? api_n_support_info)?.api_support_hourai?.api_ship_id;

        public IRawAerialPhase AerialSupportPhase => (api_support_info ?? api_n_support_info)?.api_support_airatack;
        public IRawBattlePhase SupportPhase => (api_support_info ?? api_n_support_info)?.api_support_hourai;

        public class Friendly : IRawNpcPhase
        {
            public int[] api_flare_pos;
            public Shelling api_hougeki;

            public int? NpcFlareIndex => FindFlarePosition(api_flare_pos, 0);
            public int? EnemyFlareIndex => FindFlarePosition(api_flare_pos, 1);
            public IReadOnlyList<IRawAttack> Attacks => api_hougeki?.Attacks;
        }

        public Friendly api_friendly_battle;
        public IRawNpcPhase NpcPhase => api_friendly_battle;
    }
}
