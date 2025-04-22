using Sakuno.ING.Game.Events;

namespace Sakuno.ING.Game.Provider.Json;

public partial class RawShip
{
    int IShipUpdated.TotalExperience => api_exp[0];
    int IShipUpdated.ExperienceToNextLevel => api_exp[1];

    ShipHP IShipUpdated.HP => new(api_nowhp, api_maxhp);

    int IShipUpdated.Firepower => api_karyoku[0];
    int IShipUpdated.Torpedo => api_raisou[0];
    int IShipUpdated.AntiAir => api_taiku[0];
    int IShipUpdated.Armor => api_soukou[0];
    int IShipUpdated.Evasion => api_kaihi[0];
    int IShipUpdated.AntiSubmarine => api_taisen[0];
    int IShipUpdated.LineOfSight => api_sakuteiki[0];
    int IShipUpdated.Luck => api_lucky[0];

    bool IShipUpdated.IsLocked => api_locked > 0;
}
