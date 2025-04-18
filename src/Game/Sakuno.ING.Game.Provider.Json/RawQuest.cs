namespace Sakuno.ING.Game.Provider.Json;

public partial class RawQuest
{
    QuestId IIdentifiable<QuestId>.Id => api_no;
}
