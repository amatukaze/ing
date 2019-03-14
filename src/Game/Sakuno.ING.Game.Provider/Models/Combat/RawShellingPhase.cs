using System;
using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public class RawShellingPhase : RawBattlePhase
    {
        public bool OldSchema { get; }

        public RawShellingPhase(BattleDetailJson.Shelling api) : base(SelectShelling(api))
        {
            OldSchema = api.api_at_eflag is null;
        }

        private static IEnumerable<RawAttack> SelectShelling(BattleDetailJson.Shelling api)
        {
            ReadOnlyMemory<int> attackList = api.api_at_list;
            ReadOnlyMemory<int> attackTypes = api.api_at_type ?? api.api_sp_list;
            if (attackList.Span[0] == -1)
            {
                attackList = attackList.Slice(1);
                attackTypes = attackTypes.Slice(1);
            }
            bool old = api.api_at_eflag is null; // Old schema before 17Q4

            for (int i = 0; i < attackList.Length; i++)
            {
                if (attackList.Span[i] <= 0) continue;

                (int idx, bool isEnemy) = old ?
                    ((attackList.Span[i] - 1) % 6, attackList.Span[i] >= 7) :
                    (attackList.Span[i] - 1, api.api_at_eflag[i]);
                var hits = new List<RawHit>(api.api_df_list.Length);
                for (int j = 0; j < api.api_df_list[i].Length; j++)
                    if (api.api_damage[i][j] >= 0)
                        hits.Add(new RawHit(old ?
                                (api.api_df_list[i][j] - 1) % 6 :
                                (api.api_df_list[i][j] - 1),
                            api.api_damage[i][j],
                            api.api_cl_list[i][j]));
                yield return new ComboAttack(idx, isEnemy, attackTypes.Span[i],
                    api.api_si_list.At(i)?.Where(x => x > 0).Select(x => (EquipmentInfoId)x).ToArray(), hits);
            }
        }
    }
}
