using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Game.Json.Combat;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public class RawShellingPhase : RawBattlePhase
    {
        public bool OldSchema { get; }

        public RawShellingPhase(BattleApi.Shelling api) : base(SelectShelling(api))
        {
            OldSchema = api.api_at_eflag == null;
        }

        private static IEnumerable<RawAttack> SelectShelling(BattleApi.Shelling api)
        {
            for (int i = 0; i < api.api_at_list.Length; i++)
            {
                if (api.api_at_list[i] <= 0) continue;

                int idx;
                bool isEnemy;
                if (api.api_at_eflag != null)
                {
                    isEnemy = api.api_at_eflag[i] != 0;
                    idx = api.api_at_list[i] - 1;
                }
                else // Old schema before 17Q4
                {
                    isEnemy = api.api_at_list[i] >= 6;
                    idx = (api.api_at_list[i] - 1) % 6;
                }
                var hits = new List<RawHit>(api.api_df_list.Length);
                for (int j = 0; j < api.api_df_list[i].Length; i++)
                    if (api.api_damage[i][j] >= 0)
                        hits.Add(new RawHit(api.api_df_list[i][j], api.api_damage[i][j], api.api_cl_list[i][j]));
                yield return new ComboAttack(idx, isEnemy, api.api_at_type.ElementAtOrDefault(i) + api.api_sp_list.ElementAtOrDefault(i),
                    api.api_si_list.ElementAtOrDefault(i)?.Select(x => (EquipmentInfoId)x).ToArray(), hits);
            }
        }
    }
}
