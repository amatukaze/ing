using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Game.Json;

namespace Sakuno.ING.Game.Models.Combat
{
    public class RawTorpedoPhase : RawBattlePhase
    {
        public RawTorpedoPhase(BattleDetailJson.Torpedo api)
            : base(SelectTorpedo(api.api_frai, api.api_fydam, api.api_fcl, false)
                  .Concat(SelectTorpedo(api.api_erai, api.api_eydam, api.api_ecl, true)))
        {
        }

        private static IEnumerable<RawAttack> SelectTorpedo(int[] targets, decimal[] damages, int[] criticals, bool enemyAttacks)
        {
            if (targets == null) yield break;
            int index = 0;
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] <= 0) continue;
                yield return new SingleAttack(index, enemyAttacks, 0, null, new RawHit(targets[i] - 1, damages[i], criticals[i]));
                index++;
            }
        }
    }
}
