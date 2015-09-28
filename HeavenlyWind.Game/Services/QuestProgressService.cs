using Sakuno.KanColle.Amatsukaze.Game.Services.Quest;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Services
{
    public class QuestProgressService
    {
        public static QuestProgressService Instance { get; } = new QuestProgressService();

        public IDictionary<int, ProgressInfo> Progresses { get; private set; }

        QuestProgressService()
        {
        }

        public void Initialize()
        {
            Progresses = RecordService.Instance.QuestProgress.Reload();
        }
    }
}
