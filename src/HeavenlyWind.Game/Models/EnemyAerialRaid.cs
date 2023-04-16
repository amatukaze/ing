using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using System;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class EnemyAerialRaid : ModelBase
    {
        public AerialCombatResult Result { get; }

        public int Amount { get; }

        public EnemyAerialRaid(JToken rpData)
        {
            var data = rpData["api_air_base_attack"];

            var stage1 = data["api_stage1"];
            if (stage1 != null)
                Result = (AerialCombatResult)stage1["api_disp_seiku"].ToObject<int>();

            var rStage3 = data["api_stage3"];
            if (rStage3 == null || rStage3.Type == JTokenType.Null)
                return;

            var rDamages = rStage3["api_fdam"].ToObject<int[]>().Sum();

            Amount = (int)Math.Round(rDamages * .9 + .1);
        }
    }
}
