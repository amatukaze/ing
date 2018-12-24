using System;
using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Game.Models.Battle;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json.Battle
{
    partial class BattleJson
    {
        public class Shelling : IRawBattlePhase
        {
            public bool[] api_at_eflag;
            public int[] api_at_list;
            public int[] api_at_type;
            public int[] api_sp_list;
            public int[][] api_df_list;
            public EquipmentInfoId[][] api_si_list;
            public bool[][] api_cl_list;
            public double[][] api_damage;

            public IReadOnlyList<RawAttack> Attacks
                => api_at_eflag.Select((x, i) =>
                {
                    var cl = api_cl_list.ElementAtOrDefault(i);
                    var damage = api_damage.ElementAtOrDefault(i);
                    return new RawAttack
                    (
                        api_at_list.ElementAtOrNull(i),
                        api_at_eflag.ElementAtOrDefault(i),
                        (api_at_type ?? api_sp_list).ElementAtOrDefault(i),
                        api_si_list.ElementAtOrDefault(i)?.Where(e => e > 0).ToArray() ?? Array.Empty<EquipmentInfoId>(),
                        api_df_list.ElementAtOrDefault(i)?.Select((t, j) =>
                        {
                            var (d, p) = ParseDamage(damage.ElementAtOrDefault(i));
                            return new RawHit
                            (
                                t,
                                d,
                                cl.ElementAtOrDefault(j),
                                p
                            );
                        }).ToArray() ?? Array.Empty<RawHit>()
                    );
                }).ToArray() ?? Array.Empty<RawAttack>();
        }

        public Shelling api_hougeki1;
        public IRawBattlePhase SheelingPhase1 => api_hougeki1;

        public Shelling api_hougeki2;
        public IRawBattlePhase SheelingPhase2 => api_hougeki2;

        public Shelling api_hougeki3;
        public IRawBattlePhase SheelingPhase3 => api_hougeki3;

        public Shelling api_hougeki;
        public IRawBattlePhase NightPhase => api_hougeki;

        public Shelling api_n_hougeki1;
        public IRawBattlePhase NightPhase1 => api_n_hougeki1;

        public Shelling api_n_hougeki2;
        public IRawBattlePhase NightPhase2 => api_n_hougeki2;

    }
}
