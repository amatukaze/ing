using Sakuno.ING.Game.Provider.Json;

namespace Sakuno.ING.Game.Provider;

public sealed partial class GameProvider
{
    [Api("api_get_member/questlist")]
    private void HandleQuests(QuestListJson response) =>
        _partialQuestsUpdated.OnNext(response.api_list);
}
