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
            var rDamages = rpData["api_air_base_attack"]["api_stage3"]["api_fdam"].ToObject<int[]>().Skip(1).Sum();

            Amount = (int)Math.Round(rDamages * .9 + .1);
        }
    }
}
