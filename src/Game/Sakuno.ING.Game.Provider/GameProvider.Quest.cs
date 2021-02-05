using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Quests;
using System;
using System.Collections.Specialized;

namespace Sakuno.ING.Game
{
    public partial class GameProvider
    {
        public IObservable<RawQuest[]> QuestListUpdated { get; private set; }
        public IObservable<QuestId> QuestCompleted { get; private set; }

        private static QuestId ParseQuestCompleted(NameValueCollection request) =>
            (QuestId)request.GetInt("api_quest_id");
    }
}
