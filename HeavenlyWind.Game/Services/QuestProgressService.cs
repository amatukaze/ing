using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.Collections;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Services.Quest;
using Sakuno.KanColle.Amatsukaze.Game.Services.Quest.OSS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using QuestClass = Sakuno.KanColle.Amatsukaze.Game.Models.Quest;

namespace Sakuno.KanColle.Amatsukaze.Game.Services
{
    public class QuestProgressService
    {
        public static QuestProgressService Instance { get; } = new QuestProgressService();

        public IDictionary<int, ProgressInfo> Progresses { get; private set; }

        internal IDictionary<int, QuestInfo> Infos { get; set; }
        ManualResetEventSlim r_InitializationLock = new ManualResetEventSlim(false);

        DateTimeOffset r_LastProcessTime;

        QuestProgressService()
        {
        }

        public void Initialize()
        {
            ApiService.SubscribeOnceOnlyOnBeforeProcessStarted("api_get_member/require_info", delegate
            {
                byte[] rContent;
                if (!DataStore.TryGet("quest", out rContent))
                    Infos = new ListDictionary<int, QuestInfo>();
                else
                {
                    var rReader = new JsonTextReader(new StreamReader(new MemoryStream(rContent)));
                    var rData = JArray.Load(rReader);

                    Infos = rData.Select(r => new QuestInfo(r)).ToDictionary(r => r.ID);
                }

                if (r_InitializationLock != null)
                {
                    r_InitializationLock.Set();
                    r_InitializationLock.Dispose();
                    r_InitializationLock = null;
                }
            });

            ApiService.Subscribe("api_get_member/require_info", delegate
            {
                if (r_InitializationLock != null)
                    r_InitializationLock.Wait();

                Progresses = RecordService.Instance.QuestProgress.Reload();
            });

            ApiService.Subscribe("api_get_member/questlist", r => ProcessQuestList(r.Data as RawQuestList));
            ApiService.Subscribe("api_req_quest/clearitemget", r => Progresses.Remove(int.Parse(r.Parameters["api_quest_id"])));
        }

        void ProcessQuestList(RawQuestList rpData)
        {
            if (rpData != null && rpData.Quests != null)
                foreach (var rRawQuest in rpData.Quests)
                {
                    ProgressInfo rProgressInfo;
                    if (Progresses.TryGetValue(rRawQuest.ID, out rProgressInfo))
                        rProgressInfo.ResetType = rRawQuest.Type;
                }

            var rQuests = KanColleGame.Current.Port.Quests.Table;
            if (GetResetTime(QuestType.Daily) > r_LastProcessTime)
            {
                var rResetTimes = new DateTimeOffset[5 + 1];

                rResetTimes[0] = rResetTimes[4] = DateTimeOffset.MinValue;
                rResetTimes[1] = rResetTimes[5] = GetResetTime(QuestType.Daily);
                rResetTimes[2] = GetResetTime(QuestType.Weekly);
                rResetTimes[3] = GetResetTime(QuestType.Monthly);

                var rOutdatedProgresses = Progresses.Values.Where(r => rResetTimes[(int)(!r.Quest.IsDailyReset ? r.ResetType : QuestType.Daily)] > r.UpdateTime).ToArray();
                foreach (var rProgressInfo in rOutdatedProgresses)
                {
                    var rID = rProgressInfo.Quest.ID;

                    rQuests.Remove(rID);
                    Progresses.Remove(rID);
                    RecordService.Instance.QuestProgress.DeleteRecord(rID);
                }

                var rOutdatedQuests = rQuests.Values.Where(r => rResetTimes[(int)r.Type] > r.CreationTime).ToArray();
                foreach (var rQuest in rOutdatedQuests)
                    rQuests.Remove(rQuest);
            }

            if (rpData == null || rpData.Quests == null)
                return;

            foreach (var rRawQuest in rpData.Quests)
            {
                var rID = rRawQuest.ID;

                QuestInfo rInfo;
                ProgressInfo rProgressInfo = null;
                if (!Infos.TryGetValue(rID, out rInfo))
                    Progresses.TryGetValue(rID, out rProgressInfo);
                else
                {
                    var rTotal = rInfo.Total;
                    if (rTotal > 0)
                    {
                        int rProgress;
                        if (Progresses.TryGetValue(rID, out rProgressInfo) && rQuests.ContainsKey(rID))
                        {
                            rProgress = rProgressInfo.Progress;

                            if (rRawQuest.State == QuestState.Completed)
                                rProgress = rTotal;
                            else if (rID != 214)
                                switch (rRawQuest.Progress)
                                {
                                    case QuestProgress.Progress50: rProgress = Math.Max(rProgress, (int)Math.Ceiling(rTotal * 0.5) - rInfo.StartFrom); break;
                                    case QuestProgress.Progress80: rProgress = Math.Max(rProgress, (int)Math.Ceiling(rTotal * 0.8) - rInfo.StartFrom); break;
                                }

                            rProgressInfo.Progress = rProgress;
                            rProgressInfo.State = rRawQuest.State;
                        }
                        else
                        {
                            rProgress = 0;

                            if (rRawQuest.State == QuestState.Completed)
                                rProgress = rTotal;
                            else if (rID != 214)
                                switch (rRawQuest.Progress)
                                {
                                    case QuestProgress.Progress50: rProgress = (int)Math.Ceiling(rTotal * 0.5) - rInfo.StartFrom; break;
                                    case QuestProgress.Progress80: rProgress = (int)Math.Ceiling(rTotal * 0.8) - rInfo.StartFrom; break;
                                }

                            Progresses.Add(rID, rProgressInfo = new ProgressInfo(rID, rRawQuest.Type, rRawQuest.State, rProgress));
                        }

                        if (rID == 214)
                        {
                            OSSQuestProgressRule rOSSRule;
                            if (OSSQuestProgressRule.Maps.TryGetValue(214, out rOSSRule))
                                ((OperationA)rOSSRule).UpdatePercentage(rProgressInfo);
                        }

                        if (rRawQuest.State == QuestState.Active)
                            RecordService.Instance.QuestProgress.InsertRecord(rRawQuest, rProgress);
                    }
                }

                QuestClass rQuest;
                if (!rQuests.TryGetValue(rID, out rQuest))
                    rQuests.Add(rQuest = new QuestClass(rRawQuest));

                rQuest.RealtimeProgress = rProgressInfo;
                rQuest.Extra = rInfo;
            }

            r_LastProcessTime = DateTimeOffset.Now.ToOffset(TimeSpan.FromHours(4.0));
        }

        internal static DateTimeOffset GetResetTime(QuestType rpType)
        {
            var rNow = DateTimeOffset.Now.ToOffset(TimeSpan.FromHours(4.0));

            switch (rpType)
            {
                case QuestType.Daily:
                case QuestType.Special:
                    return rNow.DateAsOffset();

                case QuestType.Weekly:
                    return rNow.LastMonday();

                case QuestType.Monthly:
                    return rNow.StartOfLastMonth();

                default:
                    return DateTimeOffset.MinValue;
            }
        }
    }
}
