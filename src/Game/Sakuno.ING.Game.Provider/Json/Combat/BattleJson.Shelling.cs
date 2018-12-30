using System.Collections.Generic;
using Sakuno.ING.Game.Models.Combat;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json.Combat
{
    partial class BattleJson
    {
        public class Shelling : IRawBattlePhase
        {
#pragma warning disable IDE1006 // Naming Styles
            public bool[] api_at_eflag { set => value.AlignSet(attacks, (r, v) => r.IsEnemy = v); }
            public int[] api_at_list { set => value.AlignSet(attacks, (r, v) => r.SourceIndex = v); }
            public int[] api_at_type { set => value.AlignSet(attacks, (r, v) => r.Type = v); }
            public int[] api_sp_list { set => value.AlignSet(attacks, (r, v) => r.Type = v); }
            public int[][] api_df_list { set => value.AlignSet(attacks, (r, v) => v.AlignSet(r.hits, (h, i) => h.TargetIndex = i)); }
            public EquipmentInfoId[][] api_si_list { set => value.AlignSet(attacks, (r, v) => r.EquipmentUsed = v); }
            public int[][] api_cl_list { set => value.AlignSet(attacks, (r, v) => v.AlignSet(r.hits, (h, i) => h.IsCritical = i == 2)); }
            public double[][] api_damage { set => value.AlignSet(attacks, (r, v) => v.AlignSet(r.hits, (h, i) => h.damage = i)); }
#pragma warning restore IDE1006 // Naming Styles

            private readonly List<ComboAttack> attacks = new List<ComboAttack>();
            public IReadOnlyList<IRawAttack> Attacks => attacks;
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
