using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Quests;
using System.Collections.Specialized;
using System.Reactive.Subjects;

namespace Sakuno.ING.Game
{
    public partial class GameProvider
    {
        private readonly Subject<RawQuest[]> _questListUpdated = new();
        private readonly Subject<QuestId> _questCompleted = new();

        [Api("api_get_member/questlist")]
        private void HandleQuestListUpdated(QuestListJson response) =>
            _questListUpdated.OnNext(response.api_list);

        [Api("api_req_quest/clearitemget")]
        private void HandleQuestCompleted(NameValueCollection request) =>
            _questCompleted.OnNext((QuestId)request.GetInt("api_quest_id"));
    }
}
