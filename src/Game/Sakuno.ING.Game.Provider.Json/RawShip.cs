using Sakuno.ING.Game.Events;

namespace Sakuno.ING.Game.Provider.Json;

public partial class RawShip
{
    int IShipUpdated.TotalExperience => api_exp[0];
    int IShipUpdated.ExperienceToNextLevel => api_exp[1];

    ShipHP IShipUpdated.HP => new(api_nowhp, api_maxhp);

    bool IShipUpdated.IsLocked => api_locked > 0;
}
