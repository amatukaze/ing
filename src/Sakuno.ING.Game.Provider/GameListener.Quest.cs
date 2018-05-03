using System.Collections.Specialized;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game
{
    partial class GameListener
    {
        public readonly IProducer<ITimedMessage<QuestPageUpdate>> QuestUpdated;
        public readonly IProducer<ITimedMessage<QuestComplete>> QuestCompleted;

        private static QuestPageUpdate ParseQuestPage(NameValueCollection request, QuestPageJson response)
            => new QuestPageUpdate
            {
                TotalCount = response.api_count,
                AnyCompleted = response.api_completed_kind,
                PageCount = response.api_page_count,
                PageId = response.api_disp_page,
                Quests = response.api_list,
                ActiveCount = response.api_exec_count
            };

        private static QuestComplete ParseQuestComplete(NameValueCollection request)
            => new QuestComplete(request.GetInt("api_quest_id"));
    }
}
