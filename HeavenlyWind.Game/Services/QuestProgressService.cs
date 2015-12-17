using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using Sakuno.KanColle.Amatsukaze.Game.Services.Quest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Services
{
    public class QuestProgressService
    {
        const string Data = @"Data\Quests.json";

        public static QuestProgressService Instance { get; } = new QuestProgressService();

        public IDictionary<int, ProgressInfo> Progresses { get; private set; }

        internal Dictionary<int, QuestInfo> Infos { get; set; }

        QuestProgressService()
        {
            SessionService.Instance.Subscribe("api_get_member/basic", _ => Progresses = RecordService.Instance.QuestProgress.Reload());

            SessionService.Instance.Subscribe("api_get_member/questlist", r => ProcessQuestList(r.Data as RawQuestList));
        }

        public void Initialize()
        {
            var rDataFile = new FileInfo(Data);
            if (!rDataFile.Exists)
                Infos = new Dictionary<int, QuestInfo>();
            else
                using (var rReader = new JsonTextReader(rDataFile.OpenText()))
                {
                    var rData = JArray.Load(rReader);

                    Infos = rData.Select(r => new QuestInfo(r)).ToDictionary(r => r.ID);
                }
        }

        void ProcessQuestList(RawQuestList rpData)
        {
            if (rpData == null)
                return;

            foreach (var rQuest in rpData.Quests)
            {
                if (rQuest.State == QuestState.None)
                    continue;

                var rID = rQuest.ID;

                QuestInfo rInfo;
                if (!Infos.TryGetValue(rID, out rInfo))
                    continue;

                ProgressInfo rProgressInfo;
                if (!Progresses.TryGetValue(rID, out rProgressInfo))
                {
                    var rTotal = rInfo.Total;
                    var rProgress = 0;

                    if (rQuest.State == QuestState.Completed)
                        rProgress = rTotal;
                    else
                        switch (rQuest.Progress)
                        {
                            case QuestProgress.Progress50: rProgress = (int)Math.Ceiling(rTotal * 0.5); break;
                            case QuestProgress.Progress80: rProgress = (int)Math.Ceiling(rTotal * 0.8); break;
                        }

                    Progresses.Add(rID, rProgressInfo = new ProgressInfo(rID, QuestState.Executing, rProgress));

                    RecordService.Instance.QuestProgress.InsertRecord(rProgressInfo);
                }

                KanColleGame.Current.Port.Quests[rID].RealtimeProgress = rProgressInfo;
            }
        }
    }
}
