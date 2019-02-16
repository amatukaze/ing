using System.Collections.Generic;
using Sakuno.ING.Game.Json;

namespace Sakuno.ING.Game.Models.Combat
{
    public class RawSupportPhase : RawBattlePhase
    {
        public RawSupportPhase(BattleDetailJson.Support.ShellingSupport api) : base(SelectSupport(api))
        {
        }

        private static IEnumerable<RawAttack> SelectSupport(BattleDetailJson.Support.ShellingSupport api)
        {
            for (int i = 0; i < api.api_damage.Length; i++)
            {
                if (api.api_damage[i] == 0) continue;
                yield return new SingleAttack(null, false, 0, null, new RawHit(i, api.api_damage[i], api.api_cl_list[i]));
            }
        }
    }
}
