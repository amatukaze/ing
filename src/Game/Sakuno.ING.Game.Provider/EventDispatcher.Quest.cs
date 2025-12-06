using Sakuno.ING.Game.Provider.Json;

namespace Sakuno.ING.Game.Provider;

partial class EventDispatcher
{
    [Api("api_get_member/questlist")]
    private void HandleQuests(QuestListJson response) =>
        _provider.OnPartialQuestsUpdated(response.api_list);
}
