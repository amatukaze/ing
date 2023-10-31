using Sakuno.ING.Game.Events;

namespace Sakuno.ING.Game.Provider.Json;

public partial class RawShip
{
    bool IShipUpdated.IsLocked => api_locked > 0;
}
