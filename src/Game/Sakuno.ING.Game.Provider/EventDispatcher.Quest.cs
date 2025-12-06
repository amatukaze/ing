using Sakuno.ING.Game.Provider.Json;

namespace Sakuno.ING.Game.Provider;

partial class EventDispatcher
{
    [Api("api_get_member/questlist")]
    private void HandleQuests(QuestListJson response) =>
        _partialQuestsUpdated.OnNext(response.api_list);
}
