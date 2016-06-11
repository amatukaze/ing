using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System;
using System.Collections.Specialized;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.OSS
{
    [Quest(214)]
    class OperationA : Battle
    {
        static BitVector32.Section[] r_Sections;

        static OperationA()
        {
            r_Sections = new BitVector32.Section[4];
            r_Sections[0] = BitVector32.CreateSection(36);
            r_Sections[1] = BitVector32.CreateSection(6, r_Sections[0]);
            r_Sections[2] = BitVector32.CreateSection(24, r_Sections[1]);
            r_Sections[3] = BitVector32.CreateSection(12, r_Sections[2]);
        }

        public override void Register(QuestInfo rpQuest)
        {
            base.Register(rpQuest);

            SessionService.Instance.Subscribe("api_req_map/start", r =>
            {
                ProgressInfo rProgressInfo;
                if (!QuestProgressService.Instance.Progresses.TryGetValue(rpQuest.ID, out rProgressInfo) || rProgressInfo.State != QuestState.Executing)
                    return;

                var rBits = new BitVector32(rProgressInfo.Progress);
                rBits[r_Sections[0]] = Math.Min(rBits[r_Sections[0]] + 1, 36);
                rProgressInfo.Progress = rBits.Data;

                UpdatePercentage(rProgressInfo, rBits);
            });
        }

        protected override void Process(ProgressInfo rpProgress, BattleInfo rpBattle, RawBattleResult rpResult)
        {
            var rBits = new BitVector32(rpProgress.Progress);

            if (rpResult.Rank >= BattleRank.S)
                rBits[r_Sections[1]] = Math.Min(rBits[r_Sections[1]] + 1, 6);
            if (rpBattle.IsBossBattle)
                rBits[r_Sections[2]] = Math.Min(rBits[r_Sections[2]] + 1, 24);
            if (rpBattle.IsBossBattle && rpResult.Rank >= BattleRank.B)
                rBits[r_Sections[3]] = Math.Min(rBits[r_Sections[3]] + 1, 12);

            rpProgress.Progress = rBits.Data;
            UpdatePercentage(rpProgress, rBits);
        }

        void UpdatePercentage(ProgressInfo rpProgress, BitVector32 rpBits)
        {
            var rPercent = 0.0;
            rPercent += Math.Min(rpBits[r_Sections[0]] / (double)36, 1.0);
            rPercent += Math.Min(rpBits[r_Sections[1]] / (double)6, 1.0);
            rPercent += Math.Min(rpBits[r_Sections[2]] / (double)24, 1.0);
            rPercent += Math.Min(rpBits[r_Sections[3]] / (double)12, 1.0);

            rpProgress.Percentage = rPercent;
        }
    }
}
