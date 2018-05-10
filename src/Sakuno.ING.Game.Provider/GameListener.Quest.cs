using System.Collections.Specialized;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game
{
    partial class GameListener
    {
        private readonly ITimedMessageProvider<QuestPageUpdate> questUpdated;
        public event TimedMessageHandler<QuestPageUpdate> QuestUpdated
        {
            add => questUpdated.Received += value;
            remove => questUpdated.Received -= value;
        }

        private readonly ITimedMessageProvider<int> questCompleted;
        public event TimedMessageHandler<int> QuestCompleted
        {
            add => questCompleted.Received += value;
            remove => questCompleted.Received -= value;
        }
        
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

        private static int ParseQuestComplete(NameValueCollection request)
            => request.GetInt("api_quest_id");
    }
}
