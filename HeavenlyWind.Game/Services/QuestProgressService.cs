using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Services.Quest;
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
    }
}
