using System.Collections.Specialized;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game
{
    public partial class GameProvider
    {
        public event TimedMessageHandler<QuestPageUpdate> QuestUpdated;
        public event TimedMessageHandler<QuestId> QuestCompleted;

        private static QuestPageUpdate ParseQuestPage(QuestPageJson response)
            => new QuestPageUpdate
            (
                totalCount: response.api_count,
                anyCompleted: response.api_completed_kind,
                pageCount: response.api_page_count,
                pageId: response.api_disp_page,
                quests: response.api_list,
                activeCount: response.api_exec_count
            );

        private static QuestId ParseQuestComplete(NameValueCollection request)
            => (QuestId)request.GetInt("api_quest_id");
    }
}
