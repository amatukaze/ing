using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Game.Json;

namespace Sakuno.ING.Game.Models.Combat
{
    public class RawTorpedoPhase : RawBattlePhase
    {
        public RawTorpedoPhase(BattleDetailJson.Torpedo api, bool oldSchema)
            : base(SelectTorpedo(api.api_frai, api.api_fydam, api.api_fcl, false, oldSchema)
                  .Concat(SelectTorpedo(api.api_erai, api.api_eydam, api.api_ecl, true, oldSchema)))
        {
        }

        private static IEnumerable<RawAttack> SelectTorpedo(int[] targets, decimal[] damages, int[] criticals, bool enemyAttacks, bool oldSchema)
        {
            if (targets == null) yield break;
            int baseIndex = oldSchema ? 1 : 0;
            for (int i = 0; i + baseIndex < targets.Length; i++)
            {
                int index = i + baseIndex;
                if (targets[index] - baseIndex < 0) continue;
                yield return new SingleAttack(i, enemyAttacks, 0, null, new RawHit(targets[index] - baseIndex, damages[index], criticals[index]));
            }
        }
    }
}
