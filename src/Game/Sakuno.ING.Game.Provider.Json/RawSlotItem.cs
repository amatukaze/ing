using Sakuno.ING.Game.Events;

namespace Sakuno.ING.Game.Provider.Json;

public partial class RawSlotItem
{
    bool ISlotItemUpdated.IsLocked => api_locked > 0;
}
