using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Services.Quest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Services
{
    public class QuestProgressService
    {
        public const string DataFilename = @"Data\Quests.json";
        static TimeSpan Offset = TimeSpan.FromHours(4.0);

        public static QuestProgressService Instance { get; } = new QuestProgressService();

        public IDictionary<int, ProgressInfo> Progresses { get; private set; }

        internal Dictionary<int, QuestInfo> Infos { get; set; }

        DateTimeOffset r_LastProcessTime;

        QuestProgressService()
        {
        }

        public void Initialize()
        {
            SessionService.Instance.SubscribeOnce("api_start2", delegate
            {
                var rDataFile = new FileInfo(DataFilename);
                if (!rDataFile.Exists)
                    Infos = new Dictionary<int, QuestInfo>();
                else
                    using (var rReader = new JsonTextReader(rDataFile.OpenText()))
                    {
                        var rData = JArray.Load(rReader);

                        Infos = rData.Select(r => new QuestInfo(r)).ToDictionary(r => r.ID);
                    }
            });

            SessionService.Instance.Subscribe("api_get_member/require_info", _ => Progresses = RecordService.Instance.QuestProgress.Reload());

            SessionService.Instance.Subscribe("api_get_member/questlist", r => ProcessQuestList(r.Data as RawQuestList));
            SessionService.Instance.Subscribe("api_req_quest/clearitemget", r => Progresses.Remove(int.Parse(r.Parameters["api_quest_id"])));
        }

        void ProcessQuestList(RawQuestList rpData)
        {
            if (GetResetTime(QuestType.Daily) > r_LastProcessTime)
            {
                var rQuests = KanColleGame.Current.Port.Quests.Table;
                var rOutdatedProgresses = Progresses.Values.Where(r => GetResetTime(!r.Quest.IsDailyReset ? r.ResetType : QuestType.Daily) > r.UpdateTime).ToArray();
                foreach (var rProgressInfo in rOutdatedProgresses)
                {
                    var rID = rProgressInfo.Quest.ID;

                    rQuests.Remove(rID);
                    Progresses.Remove(rID);
                }
                var rOutdatedQuests = rQuests.Values.Where(r => GetResetTime(r.Type) > r.CreationTime).ToArray();
                foreach (var rQuest in rOutdatedQuests)
                    rQuests.Remove(rQuest);
            }

            if (rpData == null)
                return;

            foreach (var rQuest in rpData.Quests)
            {
                var rID = rQuest.ID;

                QuestInfo rInfo;
                if (!Infos.TryGetValue(rID, out rInfo))
                    continue;

                var rTotal = rInfo.Total;
                int rProgress;

                ProgressInfo rProgressInfo;
                if (Progresses.TryGetValue(rID, out rProgressInfo))
                {
                    rProgress = rProgressInfo.Progress;

                    if (rQuest.State == QuestState.Completed)
                        rProgress = rTotal;
                    else
                        switch (rQuest.Progress)
                        {
                            case QuestProgress.Progress50: rProgress = Math.Max(rProgress, (int)Math.Ceiling(rTotal * 0.5) - rInfo.StartFrom); break;
                            case QuestProgress.Progress80: rProgress = Math.Max(rProgress, (int)Math.Ceiling(rTotal * 0.8) - rInfo.StartFrom); break;
                        }

                    rProgressInfo.Progress = rProgress;
                    rProgressInfo.State = rQuest.State;
                }
                else
                {
                    rProgress = 0;

                    if (rQuest.State == QuestState.Completed)
                        rProgress = rTotal;
                    else
                        switch (rQuest.Progress)
                        {
                            case QuestProgress.Progress50: rProgress = (int)Math.Ceiling(rTotal * 0.5) - rInfo.StartFrom; break;
                            case QuestProgress.Progress80: rProgress = (int)Math.Ceiling(rTotal * 0.8) - rInfo.StartFrom; break;
                        }

                    Progresses.Add(rID, rProgressInfo = new ProgressInfo(rID, rQuest.Type, rQuest.State, rProgress));
                }

                if (rQuest.State == QuestState.Executing)
                    RecordService.Instance.QuestProgress.InsertRecord(rQuest, rProgress);

                KanColleGame.Current.Port.Quests[rID].RealtimeProgress = rProgressInfo;
            }

            r_LastProcessTime = DateTimeOffset.Now.ToOffset(Offset);
        }

        internal static DateTimeOffset GetResetTime(QuestType rpType)
        {
            var rCurrentTime = DateTimeOffset.Now.ToOffset(Offset);
            var rResult = DateTimeOffset.MinValue;

            switch (rpType)
            {
                case QuestType.Daily:
                case QuestType.Special1:
                case QuestType.Special2:
                    rResult = new DateTimeOffset(rCurrentTime.Date, Offset);
                    break;

                case QuestType.Weekly:
                    var rDelta = rCurrentTime.DayOfWeek - DayOfWeek.Monday;
                    if (rDelta < 0)
                        rDelta += 7;

                    rResult = new DateTimeOffset(rCurrentTime.AddDays(-rDelta).Date, Offset);
                    break;

                case QuestType.Monthly:
                    rResult = new DateTimeOffset(rCurrentTime.Year, rCurrentTime.Month, 1, 0, 0, 0, Offset);
                    break;
            }

            return rResult;
        }
    }
}
