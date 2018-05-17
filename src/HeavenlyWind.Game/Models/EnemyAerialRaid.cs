using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class EnemyAerialRaid : ModelBase
    {
        public int Amount { get; }

        public EnemyAerialRaid(JToken rpData)
        {
            var rStage3 = rpData["api_air_base_attack"]["api_stage3"];
            if (rStage3 == null || rStage3.Type == JTokenType.Null)
                return;

            var rDamages = rStage3["api_fdam"].ToObject<int[]>().Sum();

            Amount = (int)Math.Round(rDamages * .9 + .1);
        }
    }
}
