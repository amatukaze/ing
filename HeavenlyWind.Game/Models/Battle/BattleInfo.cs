using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle
{
    public class BattleInfo : ModelBase
    {
        static BattleInfo r_Current;

        public long ID { get; } = (long)DateTimeUtil.ToUnixTime(DateTimeOffset.Now);

        public bool IsInitialized { get; private set; }

        public BattleStage CurrentStage { get; private set; }
        public BattleStage First { get; private set; }
        public BattleStage Second { get; private set; }

        public Formation FriendFormation { get; private set; }
        public Formation EnemyFormation { get; private set; }
        public EngagementForm EngagementForm { get; private set; }

        public AerialCombat AerialCombat { get; } = new AerialCombat();

        public BattleResult Result { get; } = new BattleResult();

        static BattleInfo()
        {
            SessionService.Instance.Subscribe("api_port/port", _ => r_Current = null);

            var rFirstStages = new[]
            {
                "api_req_sortie/battle",
                "api_req_battle_midnight/sp_midnight",
                "api_req_sortie/airbattle",
                "api_req_combined_battle/airbattle",
                "api_req_combined_battle/battle",
                "api_req_combined_battle/battle_water",
                "api_req_combined_battle/sp_midnight",
            };
            SessionService.Instance.Subscribe(rFirstStages, r => r_Current?.ProcessFirstStage(r));

            var rSecondStages = new[]
            {
                "api_req_battle_midnight/battle",
                "api_req_combined_battle/midnight_battle",
            };
            SessionService.Instance.Subscribe(rSecondStages, r => r_Current?.ProcessSecondStage(r));
        }
        internal BattleInfo()
        {
            r_Current = this;
        }

        void ProcessFirstStage(ApiData rpData)
        {
            SetFormationAndEngagementForm(rpData);
        void SetFormationAndEngagementForm(ApiData rpData)
        {
            var rFormationRawData = rpData.Data as IRawFormationAndEngagementForm;

            FriendFormation = (Formation)rFormationRawData.FormationAndEngagementForm[0];
            EnemyFormation = (Formation)rFormationRawData.FormationAndEngagementForm[1];
            EngagementForm = (EngagementForm)rFormationRawData.FormationAndEngagementForm[2];
        }

        void ProcessSecondStage(ApiData rpData)
        {
        }
    }
}
